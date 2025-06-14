using SledzSpecke.Application.Abstractions;
using SledzSpecke.Application.Exceptions;
using SledzSpecke.Core.Exceptions;
using SledzSpecke.Core.Repositories;
using SledzSpecke.Core.ValueObjects;

namespace SledzSpecke.Application.Commands.Handlers;

public class UpdateInternshipHandler : ICommandHandler<UpdateInternship>
{
    private readonly IInternshipRepository _internshipRepository;
    private readonly IUserContextService _userContextService;
    private readonly IUserRepository _userRepository;
    private readonly ISpecializationValidationService _validationService;
    private readonly ISpecializationRepository _specializationRepository;

    public UpdateInternshipHandler(
        IInternshipRepository internshipRepository,
        IUserContextService userContextService,
        IUserRepository userRepository,
        ISpecializationValidationService validationService,
        ISpecializationRepository specializationRepository)
    {
        _internshipRepository = internshipRepository;
        _userContextService = userContextService;
        _userRepository = userRepository;
        _validationService = validationService;
        _specializationRepository = specializationRepository;
    }

    public async Task HandleAsync(UpdateInternship command)
    {
        var internship = await _internshipRepository.GetByIdAsync(new InternshipId(command.InternshipId));
        if (internship == null)
        {
            throw new NotFoundException($"Internship with ID {command.InternshipId} not found.");
        }

        // Verify user has access to this internship
        var userId = _userContextService.GetUserId();
        var user = await _userRepository.GetByIdAsync(new UserId(userId));
        if (user == null || user.SpecializationId != internship.SpecializationId)
        {
            throw new UnauthorizedException("You are not authorized to update this internship.");
        }

        // Check if internship can be modified
        if (internship.IsApproved)
        {
            throw new BusinessRuleException("Cannot update an approved internship.");
        }

        // Update institution name if provided
        if (!string.IsNullOrWhiteSpace(command.InstitutionName) && command.InstitutionName != internship.InstitutionName)
        {
            internship.UpdateInstitution(command.InstitutionName, internship.DepartmentName);
        }

        // Update department name if provided
        if (!string.IsNullOrWhiteSpace(command.DepartmentName) && command.DepartmentName != internship.DepartmentName)
        {
            internship.UpdateInstitution(internship.InstitutionName, command.DepartmentName);
        }

        // Update supervisor if provided
        if (!string.IsNullOrWhiteSpace(command.SupervisorName))
        {
            internship.SetSupervisor(command.SupervisorName);
        }

        // Update dates if provided
        if (command.StartDate.HasValue || command.EndDate.HasValue)
        {
            var startDate = command.StartDate ?? internship.StartDate;
            var endDate = command.EndDate ?? internship.EndDate;
            
            internship.UpdateDates(startDate, endDate);
        }

        // Update module assignment if provided
        if (command.ModuleId.HasValue)
        {
            internship.AssignToModule(new ModuleId(command.ModuleId.Value));
        }

        // Validate the updated internship
        var specialization = await _specializationRepository.GetByIdAsync(internship.SpecializationId);
        if (specialization != null)
        {
            var validationResult = await _validationService.ValidateInternshipAsync(internship, specialization.Id.Value);
            if (!validationResult.IsValid)
            {
                throw new ValidationException($"Internship validation failed: {string.Join(", ", validationResult.Errors)}");
            }
        }

        await _internshipRepository.UpdateAsync(internship);
    }
}
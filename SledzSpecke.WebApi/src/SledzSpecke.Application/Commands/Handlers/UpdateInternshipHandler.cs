using SledzSpecke.Application.Abstractions;
using SledzSpecke.Core.Abstractions;
using SledzSpecke.Core.Exceptions;
using SledzSpecke.Core.Repositories;
using SledzSpecke.Core.ValueObjects;

namespace SledzSpecke.Application.Commands.Handlers;

public sealed class UpdateInternshipHandlerRefactored : IResultCommandHandler<UpdateInternship>
{
    private readonly IInternshipRepository _internshipRepository;
    private readonly IUserContextService _userContextService;
    private readonly IUserRepository _userRepository;
    private readonly ISpecializationValidationService _validationService;
    private readonly ISpecializationRepository _specializationRepository;
    private readonly IUnitOfWork _unitOfWork;

    public UpdateInternshipHandlerRefactored(
        IInternshipRepository internshipRepository,
        IUserContextService userContextService,
        IUserRepository userRepository,
        ISpecializationValidationService validationService,
        ISpecializationRepository specializationRepository,
        IUnitOfWork unitOfWork)
    {
        _internshipRepository = internshipRepository;
        _userContextService = userContextService;
        _userRepository = userRepository;
        _validationService = validationService;
        _specializationRepository = specializationRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result> HandleAsync(UpdateInternship command)
    {
        try
        {
            var internshipId = new InternshipId(command.InternshipId);
            var internship = await _internshipRepository.GetByIdAsync(internshipId);
            if (internship == null)
            {
                return Result.Failure($"Internship with ID {command.InternshipId} not found.");
            }

            // Verify user has access to this internship
            var userId = _userContextService.GetUserId();
            var user = await _userRepository.GetByIdAsync(new UserId(userId));
            if (user == null || user.SpecializationId.Value != internship.SpecializationId.Value)
            {
                return Result.Failure("You are not authorized to update this internship.");
            }

            // Check if internship can be modified
            if (internship.IsApproved)
            {
                return Result.Failure("Cannot update an approved internship.");
            }

            // Update institution name if provided
            if (!string.IsNullOrWhiteSpace(command.InstitutionName) && command.InstitutionName != internship.InstitutionName)
            {
                var institutionName = new InstitutionName(command.InstitutionName);
                internship.UpdateInstitution(institutionName, internship.DepartmentName);
            }

            // Update department name if provided
            if (!string.IsNullOrWhiteSpace(command.DepartmentName) && command.DepartmentName != internship.DepartmentName)
            {
                var departmentName = new DepartmentName(command.DepartmentName);
                internship.UpdateInstitution(internship.InstitutionName, departmentName);
            }

            // Update supervisor if provided
            if (!string.IsNullOrWhiteSpace(command.SupervisorName))
            {
                var supervisorName = new PersonName(command.SupervisorName);
                internship.SetSupervisor(supervisorName);
            }

            // Update dates if provided
            if (command.StartDate.HasValue || command.EndDate.HasValue)
            {
                var startDate = command.StartDate ?? internship.StartDate;
                var endDate = command.EndDate ?? internship.EndDate;

                var updateResult = internship.UpdateDates(startDate, endDate);
                if (!updateResult.IsSuccess)
                {
                    return Result.Failure(updateResult.Error, updateResult.ErrorCode);
                }
            }

            // Update module assignment if provided
            if (command.ModuleId.HasValue)
            {
                var moduleId = new ModuleId(command.ModuleId.Value);
                internship.AssignToModule(moduleId);
            }

            // Validate the updated internship
            var specialization = await _specializationRepository.GetByIdAsync(internship.SpecializationId);
            if (specialization != null)
            {
                var validationResult = await _validationService.ValidateInternshipAsync(internship, specialization.Id.Value);
                if (!validationResult.IsValid)
                {
                    return Result.Failure($"Internship validation failed: {string.Join(", ", validationResult.Errors)}");
                }
            }

            await _internshipRepository.UpdateAsync(internship);
            await _unitOfWork.SaveChangesAsync();
            
            return Result.Success();
        }
        catch (Exception ex) when (ex is CustomException)
        {
            return Result.Failure(ex.Message);
        }
        catch (Exception)
        {
            return Result.Failure("An error occurred while updating the internship.");
        }
    }
}
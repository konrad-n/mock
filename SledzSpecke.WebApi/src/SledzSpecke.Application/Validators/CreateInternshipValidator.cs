using FluentValidation;
using SledzSpecke.Application.Commands.Internships;
using SledzSpecke.Core.Repositories;
using SledzSpecke.Core.ValueObjects;
using SledzSpecke.Api.Extensions;

namespace SledzSpecke.Application.Validators;

/// <summary>
/// Validator for CreateInternship command with module progression rules
/// </summary>
public class CreateInternshipValidator : AbstractValidator<CreateInternship>
{
    private readonly ISpecializationRepository _specializationRepository;
    private readonly IInternshipRepository _internshipRepository;

    public CreateInternshipValidator(
        ISpecializationRepository specializationRepository,
        IInternshipRepository internshipRepository)
    {
        _specializationRepository = specializationRepository;
        _internshipRepository = internshipRepository;

        RuleFor(x => x.SpecializationId)
            .GreaterThan(0)
            .WithMessage("Specialization ID must be a positive number")
            .WithErrorCode(ErrorCodes.VALIDATION_ERROR)
            .MustAsync(SpecializationExists)
            .WithMessage("Specialization not found")
            .WithErrorCode(ErrorCodes.NOT_FOUND)
            .MustAsync(SpecializationIsActive)
            .WithMessage("Specialization is not active")
            .WithErrorCode(ErrorCodes.SPECIALIZATION_NOT_ACTIVE);

        RuleFor(x => x.Department)
            .NotEmpty()
            .WithMessage("Department is required")
            .MaximumLength(255)
            .WithMessage("Department cannot exceed 255 characters")
            .WithErrorCode(ErrorCodes.VALIDATION_ERROR);

        RuleFor(x => x.Hospital)
            .NotEmpty()
            .WithMessage("Hospital is required")
            .MaximumLength(255)
            .WithMessage("Hospital cannot exceed 255 characters")
            .WithErrorCode(ErrorCodes.VALIDATION_ERROR);

        RuleFor(x => x.StartDate)
            .NotEmpty()
            .WithMessage("Start date is required")
            .GreaterThan(DateTime.UtcNow.AddYears(-1))
            .WithMessage("Start date cannot be more than 1 year in the past")
            .WithErrorCode(ErrorCodes.VALIDATION_ERROR);

        RuleFor(x => x.EndDate)
            .NotEmpty()
            .WithMessage("End date is required")
            .GreaterThan(x => x.StartDate)
            .WithMessage("End date must be after start date")
            .WithErrorCode(ErrorCodes.VALIDATION_ERROR);

        RuleFor(x => x)
            .Must(HaveValidDuration)
            .WithMessage("Internship duration must be between 1 week and 1 year")
            .WithErrorCode(ErrorCodes.VALIDATION_ERROR);

        RuleFor(x => x.SupervisorName)
            .NotEmpty()
            .WithMessage("Supervisor name is required")
            .MaximumLength(255)
            .WithMessage("Supervisor name cannot exceed 255 characters")
            .Matches(@"^[a-zA-ZąćęłńóśźżĄĆĘŁŃÓŚŹŻ\s\-'.,]+$")
            .WithMessage("Supervisor name contains invalid characters")
            .WithErrorCode(ErrorCodes.VALIDATION_ERROR);

        RuleFor(x => x.SupervisorEmail)
            .EmailAddress()
            .When(x => !string.IsNullOrEmpty(x.SupervisorEmail))
            .WithMessage("Invalid supervisor email format")
            .WithErrorCode(ErrorCodes.VALIDATION_ERROR);

        RuleFor(x => x.SupervisorPhone)
            .Matches(@"^(\+48)?[ -]?[0-9]{3}[ -]?[0-9]{3}[ -]?[0-9]{3}$")
            .When(x => !string.IsNullOrEmpty(x.SupervisorPhone))
            .WithMessage("Invalid Polish phone number format")
            .WithErrorCode(ErrorCodes.VALIDATION_ERROR);

        RuleFor(x => x.ModuleId)
            .GreaterThan(0)
            .WithMessage("Module ID must be a positive number")
            .WithErrorCode(ErrorCodes.VALIDATION_ERROR)
            .MustAsync(FollowModuleProgression)
            .WithMessage("Cannot create internship for this module - previous modules must be completed first")
            .WithErrorCode(ErrorCodes.INVALID_MODULE_PROGRESSION);

        RuleFor(x => x.Notes)
            .MaximumLength(2000)
            .When(x => !string.IsNullOrEmpty(x.Notes))
            .WithMessage("Notes cannot exceed 2000 characters")
            .WithErrorCode(ErrorCodes.VALIDATION_ERROR);

        RuleFor(x => new { x.SpecializationId, x.StartDate, x.EndDate })
            .MustAsync(async (data, cancellation) => 
                await NotOverlapWithExistingInternships(data.SpecializationId, data.StartDate, data.EndDate))
            .WithMessage("This internship overlaps with an existing internship")
            .WithErrorCode(ErrorCodes.CONFLICT);
    }

    private async Task<bool> SpecializationExists(int specializationId, CancellationToken cancellationToken)
    {
        var specialization = await _specializationRepository.GetByIdAsync(new SpecializationId(specializationId));
        return specialization != null;
    }

    private async Task<bool> SpecializationIsActive(int specializationId, CancellationToken cancellationToken)
    {
        var specialization = await _specializationRepository.GetByIdAsync(new SpecializationId(specializationId));
        return specialization?.IsActive ?? false;
    }

    private async Task<bool> FollowModuleProgression(CreateInternship command, int moduleId, CancellationToken cancellationToken)
    {
        // Basic modules (1-3) can be done in any order
        if (moduleId <= 3)
            return true;

        // Specialistic modules (4+) require basic modules to be completed
        var internships = await _internshipRepository.GetBySpecializationIdAsync(new SpecializationId(command.SpecializationId));
        var completedModules = internships
            .Where(i => i.IsCompleted && i.ModuleId.HasValue)
            .Select(i => i.ModuleId!.Value)
            .ToHashSet();

        // Check if all basic modules (1-3) are completed
        for (int i = 1; i <= 3; i++)
        {
            if (!completedModules.Contains(new ModuleId(i)))
                return false;
        }

        // For modules 5+, check if previous specialistic module is completed
        if (moduleId > 4)
        {
            if (!completedModules.Contains(new ModuleId(moduleId - 1)))
                return false;
        }

        return true;
    }

    private bool HaveValidDuration(CreateInternship command)
    {
        var duration = command.EndDate - command.StartDate;
        return duration >= TimeSpan.FromDays(7) && duration <= TimeSpan.FromDays(365);
    }

    private async Task<bool> NotOverlapWithExistingInternships(int specializationId, DateTime startDate, DateTime endDate)
    {
        var internships = await _internshipRepository.GetBySpecializationIdAsync(new SpecializationId(specializationId));
        
        return !internships.Any(i => 
            (startDate >= i.StartDate && startDate <= i.EndDate) ||
            (endDate >= i.StartDate && endDate <= i.EndDate) ||
            (startDate <= i.StartDate && endDate >= i.EndDate));
    }
}
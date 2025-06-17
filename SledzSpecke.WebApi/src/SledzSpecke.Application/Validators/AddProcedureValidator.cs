using FluentValidation;
using SledzSpecke.Application.Commands.Procedures;
using SledzSpecke.Core.Repositories;
using SledzSpecke.Core.ValueObjects;
using SledzSpecke.Api.Extensions;

namespace SledzSpecke.Application.Validators;

/// <summary>
/// Validator for AddProcedure command with duplicate prevention
/// </summary>
public class AddProcedureValidator : AbstractValidator<AddProcedure>
{
    private readonly IProcedureRepository _procedureRepository;
    private readonly IInternshipRepository _internshipRepository;

    public AddProcedureValidator(
        IProcedureRepository procedureRepository,
        IInternshipRepository internshipRepository)
    {
        _procedureRepository = procedureRepository;
        _internshipRepository = internshipRepository;

        RuleFor(x => x.InternshipId)
            .GreaterThan(0)
            .WithMessage("Internship ID must be a positive number")
            .WithErrorCode(ErrorCodes.VALIDATION_ERROR)
            .MustAsync(InternshipExists)
            .WithMessage("Internship not found")
            .WithErrorCode(ErrorCodes.NOT_FOUND);

        RuleFor(x => x.ProcedureName)
            .NotEmpty()
            .WithMessage("Procedure name is required")
            .MaximumLength(500)
            .WithMessage("Procedure name cannot exceed 500 characters")
            .WithErrorCode(ErrorCodes.VALIDATION_ERROR);

        RuleFor(x => x.Date)
            .NotEmpty()
            .WithMessage("Procedure date is required")
            .WithErrorCode(ErrorCodes.VALIDATION_ERROR)
            .MustAsync(BeWithinInternshipPeriod)
            .WithMessage("Procedure date must be within the internship period")
            .WithErrorCode(ErrorCodes.BUSINESS_RULE_VIOLATION);

        RuleFor(x => x.Count)
            .GreaterThan(0)
            .WithMessage("Procedure count must be greater than 0")
            .LessThanOrEqualTo(100)
            .WithMessage("Procedure count cannot exceed 100")
            .WithErrorCode(ErrorCodes.VALIDATION_ERROR);

        RuleFor(x => x.ModuleId)
            .GreaterThan(0)
            .When(x => x.ModuleId.HasValue)
            .WithMessage("Module ID must be a positive number if provided")
            .WithErrorCode(ErrorCodes.VALIDATION_ERROR);

        RuleFor(x => x.Notes)
            .MaximumLength(2000)
            .When(x => !string.IsNullOrEmpty(x.Notes))
            .WithMessage("Notes cannot exceed 2000 characters")
            .WithErrorCode(ErrorCodes.VALIDATION_ERROR);

        RuleFor(x => x.Supervisor)
            .MaximumLength(255)
            .When(x => !string.IsNullOrEmpty(x.Supervisor))
            .WithMessage("Supervisor name cannot exceed 255 characters")
            .WithErrorCode(ErrorCodes.VALIDATION_ERROR);

        RuleFor(x => x.Department)
            .MaximumLength(255)
            .When(x => !string.IsNullOrEmpty(x.Department))
            .WithMessage("Department name cannot exceed 255 characters")
            .WithErrorCode(ErrorCodes.VALIDATION_ERROR);

        RuleFor(x => new { x.InternshipId, x.ProcedureName, x.Date })
            .MustAsync(async (data, cancellation) => 
                await NotBeDuplicateProcedure(data.InternshipId, data.ProcedureName, data.Date))
            .WithMessage("A procedure with this name already exists on the same date")
            .WithErrorCode(ErrorCodes.DUPLICATE_PROCEDURE);

        RuleFor(x => x.ProcedureType)
            .IsInEnum()
            .When(x => x.ProcedureType.HasValue)
            .WithMessage("Invalid procedure type")
            .WithErrorCode(ErrorCodes.VALIDATION_ERROR);

        RuleFor(x => x.Complexity)
            .IsInEnum()
            .When(x => x.Complexity.HasValue)
            .WithMessage("Invalid complexity level")
            .WithErrorCode(ErrorCodes.VALIDATION_ERROR);
    }

    private async Task<bool> InternshipExists(int internshipId, CancellationToken cancellationToken)
    {
        var internship = await _internshipRepository.GetByIdAsync(new InternshipId(internshipId));
        return internship != null;
    }

    private async Task<bool> BeWithinInternshipPeriod(AddProcedure command, DateTime date, CancellationToken cancellationToken)
    {
        var internship = await _internshipRepository.GetByIdAsync(new InternshipId(command.InternshipId));
        if (internship == null)
            return false;

        return date >= internship.StartDate && date <= internship.EndDate;
    }

    private async Task<bool> NotBeDuplicateProcedure(int internshipId, string procedureName, DateTime date)
    {
        var existingProcedures = await _procedureRepository.GetByInternshipIdAsync(new InternshipId(internshipId));
        
        // Check if there's already a procedure with the same name on the same date
        return !existingProcedures.Any(p => 
            p.ProcedureName.Value.Equals(procedureName, StringComparison.OrdinalIgnoreCase) && 
            p.Date.Date == date.Date);
    }
}
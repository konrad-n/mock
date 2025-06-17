using FluentValidation;
using SledzSpecke.Application.Commands.MedicalShifts;
using SledzSpecke.Core.Repositories;
using SledzSpecke.Core.ValueObjects;
using SledzSpecke.Api.Extensions;

namespace SledzSpecke.Application.Validators;

/// <summary>
/// Validator for AddMedicalShift command with complex business rules
/// </summary>
public class AddMedicalShiftValidator : AbstractValidator<AddMedicalShift>
{
    private readonly IInternshipRepository _internshipRepository;
    private readonly IMedicalShiftRepository _shiftRepository;

    public AddMedicalShiftValidator(
        IInternshipRepository internshipRepository,
        IMedicalShiftRepository shiftRepository)
    {
        _internshipRepository = internshipRepository;
        _shiftRepository = shiftRepository;

        RuleFor(x => x.InternshipId)
            .GreaterThan(0)
            .WithMessage("Internship ID must be a positive number")
            .WithErrorCode(ErrorCodes.VALIDATION_ERROR);

        RuleFor(x => x.Date)
            .NotEmpty()
            .WithMessage("Shift date is required")
            .WithErrorCode(ErrorCodes.VALIDATION_ERROR)
            .MustAsync(BeWithinInternshipPeriod)
            .WithMessage("Shift date must be within the internship period")
            .WithErrorCode(ErrorCodes.SHIFT_OUTSIDE_INTERNSHIP);

        RuleFor(x => x.Hours)
            .InclusiveBetween(1, 24)
            .WithMessage("Shift hours must be between 1 and 24")
            .WithErrorCode(ErrorCodes.VALIDATION_ERROR);

        RuleFor(x => new { x.InternshipId, x.Date, x.Hours })
            .MustAsync(async (shift, cancellation) => 
                await NotExceedWeeklyLimit(shift.InternshipId, shift.Date, shift.Hours))
            .WithMessage("Adding this shift would exceed the 48-hour weekly limit")
            .WithErrorCode(ErrorCodes.WEEKLY_HOURS_EXCEEDED);

        RuleFor(x => x.Department)
            .NotEmpty()
            .WithMessage("Department is required")
            .MaximumLength(255)
            .WithMessage("Department name cannot exceed 255 characters")
            .WithErrorCode(ErrorCodes.VALIDATION_ERROR);

        RuleFor(x => x.Hospital)
            .NotEmpty()
            .WithMessage("Hospital is required")
            .MaximumLength(255)
            .WithMessage("Hospital name cannot exceed 255 characters")
            .WithErrorCode(ErrorCodes.VALIDATION_ERROR);

        RuleFor(x => x.ModuleId)
            .GreaterThan(0)
            .When(x => x.ModuleId.HasValue)
            .WithMessage("Module ID must be a positive number if provided")
            .WithErrorCode(ErrorCodes.VALIDATION_ERROR);

        RuleFor(x => x.StartTime)
            .Must(BeValidTime)
            .When(x => !string.IsNullOrEmpty(x.StartTime))
            .WithMessage("Start time must be in HH:mm format")
            .WithErrorCode(ErrorCodes.VALIDATION_ERROR);

        RuleFor(x => x.EndTime)
            .Must(BeValidTime)
            .When(x => !string.IsNullOrEmpty(x.EndTime))
            .WithMessage("End time must be in HH:mm format")
            .WithErrorCode(ErrorCodes.VALIDATION_ERROR);

        When(x => !string.IsNullOrEmpty(x.StartTime) && !string.IsNullOrEmpty(x.EndTime), () =>
        {
            RuleFor(x => x)
                .Must(HaveValidTimeRange)
                .WithMessage("End time must be after start time")
                .WithErrorCode(ErrorCodes.VALIDATION_ERROR);
        });

        RuleFor(x => x.SupervisorName)
            .MaximumLength(255)
            .When(x => !string.IsNullOrEmpty(x.SupervisorName))
            .WithMessage("Supervisor name cannot exceed 255 characters")
            .WithErrorCode(ErrorCodes.VALIDATION_ERROR);

        RuleFor(x => x.Description)
            .MaximumLength(2000)
            .When(x => !string.IsNullOrEmpty(x.Description))
            .WithMessage("Description cannot exceed 2000 characters")
            .WithErrorCode(ErrorCodes.VALIDATION_ERROR);
    }

    private async Task<bool> BeWithinInternshipPeriod(AddMedicalShift command, DateTime date, CancellationToken cancellationToken)
    {
        var internship = await _internshipRepository.GetByIdAsync(new InternshipId(command.InternshipId));
        if (internship == null)
            return false;

        return date >= internship.StartDate && date <= internship.EndDate;
    }

    private async Task<bool> NotExceedWeeklyLimit(int internshipId, DateTime date, int hours)
    {
        // Calculate the week boundaries (Monday to Sunday)
        var dayOfWeek = date.DayOfWeek == DayOfWeek.Sunday ? 7 : (int)date.DayOfWeek;
        var weekStart = date.AddDays(1 - dayOfWeek);
        var weekEnd = weekStart.AddDays(6);

        // Get all shifts for this week
        var shifts = await _shiftRepository.GetByInternshipIdAndDateRangeAsync(
            new InternshipId(internshipId), 
            weekStart, 
            weekEnd);

        var totalHours = shifts.Sum(s => s.Hours) + hours;
        return totalHours <= 48;
    }

    private bool BeValidTime(string? time)
    {
        if (string.IsNullOrEmpty(time))
            return true;

        if (time.Length != 5 || time[2] != ':')
            return false;

        if (!int.TryParse(time.Substring(0, 2), out var hours) || hours < 0 || hours > 23)
            return false;

        if (!int.TryParse(time.Substring(3, 2), out var minutes) || minutes < 0 || minutes > 59)
            return false;

        return true;
    }

    private bool HaveValidTimeRange(AddMedicalShift command)
    {
        if (string.IsNullOrEmpty(command.StartTime) || string.IsNullOrEmpty(command.EndTime))
            return true;

        var startParts = command.StartTime.Split(':');
        var endParts = command.EndTime.Split(':');

        var startHours = int.Parse(startParts[0]);
        var startMinutes = int.Parse(startParts[1]);
        var endHours = int.Parse(endParts[0]);
        var endMinutes = int.Parse(endParts[1]);

        var startTotalMinutes = startHours * 60 + startMinutes;
        var endTotalMinutes = endHours * 60 + endMinutes;

        // Allow shifts that cross midnight
        if (endTotalMinutes < startTotalMinutes)
        {
            // Shift crosses midnight, so it's valid
            return true;
        }

        return endTotalMinutes > startTotalMinutes;
    }
}
using FluentValidation;
using SledzSpecke.Application.Commands;
using SledzSpecke.Core.Entities;
using SledzSpecke.Core.Repositories;
using SledzSpecke.Core.ValueObjects;

namespace SledzSpecke.Application.Validators;

/// <summary>
/// Validator for AddMedicalShift command with comprehensive business rules
/// </summary>
public class AddMedicalShiftValidator : AbstractValidator<AddMedicalShift>
{
    private const int MONTHLY_HOURS_MINIMUM = 160;
    private const int WEEKLY_HOURS_MAXIMUM = 48;
    private const int DAILY_HOURS_MAXIMUM = 24;
    
    public AddMedicalShiftValidator(IInternshipRepository internshipRepository)
    {
        RuleFor(x => x.InternshipId)
            .GreaterThan(0)
            .WithMessage("Internship ID must be a positive number")
            .WithErrorCode("INVALID_INTERNSHIP_ID");
            
        RuleFor(x => x.Date)
            .NotEmpty()
            .WithMessage("Shift date is required")
            .LessThanOrEqualTo(DateTime.UtcNow.Date.AddDays(1))
            .WithMessage("Shift date cannot be in the future")
            .WithErrorCode("FUTURE_DATE_NOT_ALLOWED")
            .GreaterThan(DateTime.UtcNow.AddMonths(-6))
            .WithMessage("Shift date cannot be older than 6 months")
            .WithErrorCode("DATE_TOO_OLD");
            
        RuleFor(x => x.Hours)
            .InclusiveBetween(0, 23)
            .WithMessage("Hours must be between 0 and 23")
            .WithErrorCode("INVALID_HOURS");
            
        RuleFor(x => x.Minutes)
            .InclusiveBetween(0, 59)
            .WithMessage("Minutes must be between 0 and 59")
            .WithErrorCode("INVALID_MINUTES")
            .When(x => x.Hours < 24);
            
        RuleFor(x => x)
            .Must(x => x.Hours > 0 || x.Minutes > 0)
            .WithMessage("Shift duration must be greater than 0")
            .WithErrorCode("ZERO_DURATION")
            .Must(x => x.Hours * 60 + x.Minutes <= DAILY_HOURS_MAXIMUM * 60)
            .WithMessage($"Shift duration cannot exceed {DAILY_HOURS_MAXIMUM} hours")
            .WithErrorCode("EXCEEDS_DAILY_MAXIMUM");
            
        RuleFor(x => x.Location)
            .NotEmpty()
            .WithMessage("Location is required")
            .MaximumLength(200)
            .WithMessage("Location cannot exceed 200 characters")
            .Matches(@"^[a-zA-ZąćęłńóśźżĄĆĘŁŃÓŚŹŻ\s\-\.,0-9]+$")
            .WithMessage("Location contains invalid characters")
            .WithErrorCode("INVALID_LOCATION_FORMAT");
            
        RuleFor(x => x.Year)
            .InclusiveBetween(1, 6)
            .WithMessage("Year must be between 1 and 6")
            .WithErrorCode("INVALID_YEAR");
            
        // Async validation for business rules
        RuleFor(x => x)
            .MustAsync(async (command, cancellation) =>
            {
                var internship = await internshipRepository.GetByIdAsync(command.InternshipId);
                return internship != null && internship.Status == InternshipStatus.InProgress;
            })
            .WithMessage("Internship not found or is not active")
            .WithErrorCode("INTERNSHIP_NOT_ACTIVE");
            
        RuleFor(x => x)
            .MustAsync(async (command, cancellation) =>
            {
                var internship = await internshipRepository.GetByIdAsync(command.InternshipId);
                if (internship == null) return true; // Will be caught by previous rule
                
                return command.Date >= internship.StartDate && 
                       command.Date <= internship.EndDate;
            })
            .WithMessage("Shift date must be within internship period")
            .WithErrorCode("SHIFT_OUTSIDE_INTERNSHIP");
            
        // Weekend shift warning (not an error, just sets severity)
        RuleFor(x => x.Date)
            .Must(date => date.DayOfWeek != DayOfWeek.Saturday && date.DayOfWeek != DayOfWeek.Sunday)
            .WithMessage("Weekend shifts require supervisor approval")
            .WithSeverity(Severity.Warning);
            
        // Night shift detection
        RuleFor(x => x)
            .Must(x => !IsNightShift(x.Hours))
            .WithMessage("Night shifts (22:00-06:00) require additional documentation")
            .WithSeverity(Severity.Warning);
    }
    
    private bool IsNightShift(int startHour)
    {
        return startHour >= 22 || startHour < 6;
    }
}
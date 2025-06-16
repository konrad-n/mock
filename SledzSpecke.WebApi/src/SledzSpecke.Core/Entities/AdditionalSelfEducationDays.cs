using SledzSpecke.Core.Abstractions;
using SledzSpecke.Core.ValueObjects;

namespace SledzSpecke.Core.Entities;

/// <summary>
/// Represents additional self-education days taken during medical specialization.
/// Maximum 6 days per year allowed as per SMK regulations.
/// </summary>
public sealed class AdditionalSelfEducationDays
{
    private const int MaxDaysPerYear = 6;
    
    public int Id { get; private set; }
    public ModuleId ModuleId { get; private set; }
    public InternshipId InternshipId { get; private set; }
    public DateTime StartDate { get; private set; }
    public DateTime EndDate { get; private set; }
    public int NumberOfDays { get; private set; }
    public string Purpose { get; private set; }
    public string EventName { get; private set; }
    public string? Location { get; private set; }
    public string? Organizer { get; private set; }
    public bool IsApproved { get; private set; }
    public DateTime CreatedAt { get; private set; }
    public DateTime UpdatedAt { get; private set; }

    private AdditionalSelfEducationDays() { } // EF Core

    private AdditionalSelfEducationDays(
        ModuleId moduleId, 
        InternshipId internshipId,
        DateTime startDate,
        DateTime endDate,
        string purpose,
        string eventName)
    {
        ModuleId = moduleId;
        InternshipId = internshipId;
        StartDate = startDate.Date;
        EndDate = endDate.Date;
        NumberOfDays = CalculateDays(startDate, endDate);
        Purpose = purpose;
        EventName = eventName;
        IsApproved = false;
        CreatedAt = DateTime.UtcNow;
        UpdatedAt = DateTime.UtcNow;
    }

    public static Result<AdditionalSelfEducationDays> Create(
        ModuleId moduleId,
        InternshipId internshipId,
        DateTime startDate,
        DateTime endDate,
        string purpose,
        string eventName)
    {
        // Validate inputs
        if (moduleId == null)
            return Result<AdditionalSelfEducationDays>.Failure("Module ID is required", "MODULE_ID_REQUIRED");
            
        if (internshipId == null)
            return Result<AdditionalSelfEducationDays>.Failure("Internship ID is required", "INTERNSHIP_ID_REQUIRED");
            
        if (string.IsNullOrWhiteSpace(purpose))
            return Result<AdditionalSelfEducationDays>.Failure("Purpose is required", "PURPOSE_REQUIRED");
            
        if (string.IsNullOrWhiteSpace(eventName))
            return Result<AdditionalSelfEducationDays>.Failure("Event name is required", "EVENT_NAME_REQUIRED");

        // Validate dates
        if (startDate > endDate)
            return Result<AdditionalSelfEducationDays>.Failure("Start date cannot be after end date", "INVALID_DATE_RANGE");

        // Calculate days
        var days = CalculateDays(startDate, endDate);
        if (days > MaxDaysPerYear)
            return Result<AdditionalSelfEducationDays>.Failure(
                $"Additional education cannot exceed {MaxDaysPerYear} days", 
                "EXCEEDED_DAYS_LIMIT");

        if (days <= 0)
            return Result<AdditionalSelfEducationDays>.Failure(
                "Duration must be at least 1 day", 
                "INVALID_DURATION");

        var instance = new AdditionalSelfEducationDays(
            moduleId, 
            internshipId, 
            startDate, 
            endDate, 
            purpose, 
            eventName);

        return Result<AdditionalSelfEducationDays>.Success(instance);
    }

    public Result UpdateDetails(string purpose, string eventName, string? location, string? organizer)
    {
        if (IsApproved)
            return Result.Failure("Cannot modify approved self-education days", "ALREADY_APPROVED");

        if (string.IsNullOrWhiteSpace(purpose))
            return Result.Failure("Purpose is required", "PURPOSE_REQUIRED");
            
        if (string.IsNullOrWhiteSpace(eventName))
            return Result.Failure("Event name is required", "EVENT_NAME_REQUIRED");

        Purpose = purpose;
        EventName = eventName;
        Location = location;
        Organizer = organizer;
        UpdatedAt = DateTime.UtcNow;
        
        return Result.Success();
    }

    public Result UpdateDates(DateTime startDate, DateTime endDate)
    {
        if (IsApproved)
            return Result.Failure("Cannot modify approved self-education days", "ALREADY_APPROVED");

        if (startDate > endDate)
            return Result.Failure("Start date cannot be after end date", "INVALID_DATE_RANGE");

        var days = CalculateDays(startDate, endDate);
        if (days > MaxDaysPerYear)
            return Result.Failure(
                $"Additional education cannot exceed {MaxDaysPerYear} days", 
                "EXCEEDED_DAYS_LIMIT");

        if (days <= 0)
            return Result.Failure("Duration must be at least 1 day", "INVALID_DURATION");

        StartDate = startDate.Date;
        EndDate = endDate.Date;
        NumberOfDays = days;
        UpdatedAt = DateTime.UtcNow;

        return Result.Success();
    }

    public Result Approve()
    {
        if (IsApproved)
            return Result.Failure("Self-education days already approved", "ALREADY_APPROVED");

        IsApproved = true;
        UpdatedAt = DateTime.UtcNow;

        return Result.Success();
    }

    public Result Reject()
    {
        if (!IsApproved)
            return Result.Failure("Self-education days not approved", "NOT_APPROVED");

        IsApproved = false;
        UpdatedAt = DateTime.UtcNow;

        return Result.Success();
    }

    private static int CalculateDays(DateTime startDate, DateTime endDate)
    {
        return (endDate.Date - startDate.Date).Days + 1;
    }

}
using SledzSpecke.Core.Abstractions;
using SledzSpecke.Core.Entities;
using SledzSpecke.Core.ValueObjects;

namespace SledzSpecke.Core.DomainServices;

/// <summary>
/// Service for managing educational course requirements and tracking for both Old and New SMK systems
/// </summary>
public interface ICourseRequirementService
{
    /// <summary>
    /// Gets course requirements for a specific specialization and year/module
    /// </summary>
    Task<Result<CourseRequirements>> GetRequirementsAsync(
        SpecializationId specializationId,
        SmkVersion smkVersion,
        int? year = null,
        ModuleId? moduleId = null);
    
    /// <summary>
    /// Tracks course participation and completion
    /// </summary>
    Task<Result> RecordCourseParticipationAsync(
        UserId userId,
        CourseId courseId,
        DateTime participationDate,
        int hoursCompleted,
        bool isPassed);
    
    /// <summary>
    /// Gets user's course progress for a specialization
    /// </summary>
    Task<Result<CourseProgress>> GetUserCourseProgressAsync(
        UserId userId,
        SpecializationId specializationId);
    
    /// <summary>
    /// Validates if user meets course requirements for year/module completion
    /// </summary>
    Task<Result<CourseCompletionStatus>> ValidateCourseCompletionAsync(
        UserId userId,
        SpecializationId specializationId,
        int? year = null,
        ModuleId? moduleId = null);
    
    /// <summary>
    /// Gets upcoming required courses for a user
    /// </summary>
    Task<Result<IEnumerable<RequiredCourse>>> GetUpcomingRequiredCoursesAsync(
        UserId userId,
        SpecializationId specializationId);
}

public class CourseRequirements
{
    public SpecializationId SpecializationId { get; set; }
    public SmkVersion SmkVersion { get; set; }
    public int? Year { get; set; } // For Old SMK
    public ModuleId? ModuleId { get; set; } // For New SMK
    public List<CourseRequirement> RequiredCourses { get; set; } = new();
    public int TotalRequiredHours { get; set; }
    public int MinimumPassingCourses { get; set; }
}

public class CourseRequirement
{
    public CourseId CourseId { get; set; }
    public string CourseName { get; set; } = string.Empty;
    public string CourseCode { get; set; } = string.Empty;
    public int RequiredHours { get; set; }
    public bool IsMandatory { get; set; }
    public CourseType Type { get; set; }
    public string Description { get; set; } = string.Empty;
}

public class CourseProgress
{
    public UserId UserId { get; set; }
    public SpecializationId SpecializationId { get; set; }
    public List<CompletedCourse> CompletedCourses { get; set; } = new();
    public int TotalHoursCompleted { get; set; }
    public int TotalCoursesCompleted { get; set; }
    public int TotalCoursesPassed { get; set; }
    public double CompletionPercentage { get; set; }
    public List<CourseRequirement> RemainingRequiredCourses { get; set; } = new();
}

public class CompletedCourse
{
    public CourseId CourseId { get; set; }
    public string CourseName { get; set; } = string.Empty;
    public DateTime CompletionDate { get; set; }
    public int HoursCompleted { get; set; }
    public bool IsPassed { get; set; }
    public string? CertificateNumber { get; set; }
}

public class CourseCompletionStatus
{
    public bool MeetsAllRequirements { get; set; }
    public List<string> UnmetRequirements { get; set; } = new();
    public int RequiredHours { get; set; }
    public int CompletedHours { get; set; }
    public int RequiredCourses { get; set; }
    public int CompletedCourses { get; set; }
    public List<CourseRequirement> MissingMandatoryCourses { get; set; } = new();
}

public class RequiredCourse
{
    public CourseRequirement Requirement { get; set; } = null!;
    public DateTime? NextAvailableDate { get; set; }
    public string? Location { get; set; }
    public int Priority { get; set; } // 1 = highest priority
    public string RecommendedTiming { get; set; } = string.Empty; // e.g., "Before end of Year 2"
}

public enum CourseType
{
    Theoretical,
    Practical,
    Workshop,
    Seminar,
    Conference,
    OnlineTraining
}


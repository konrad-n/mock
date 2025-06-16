using SledzSpecke.Core.Abstractions;
using SledzSpecke.Core.Entities;
using SledzSpecke.Core.ValueObjects;
using System.Text.RegularExpressions;

namespace SledzSpecke.Core.DomainServices;

/// <summary>
/// Service responsible for validating CMKP (Centrum Medyczne Kształcenia Podyplomowego) certificates
/// and course requirements according to SMK standards
/// </summary>
public sealed class CmkpValidationService : ICmkpValidationService
{
    // CMKP certificate number format: CMKP/[YEAR]/[NUMBER]
    private static readonly Regex CertificateNumberRegex = new(@"^CMKP\/\d{4}\/\d+$", RegexOptions.Compiled);
    
    // Allowed course types that require CMKP certification
    private static readonly HashSet<ValueObjects.CourseType> CmkpRequiredCourseTypes = new()
    {
        ValueObjects.CourseType.Specialization,
        ValueObjects.CourseType.Improvement,
        ValueObjects.CourseType.Certification
    };

    /// <summary>
    /// Validates CMKP certificate number format
    /// </summary>
    public Result<bool> ValidateCertificateNumber(string? certificateNumber)
    {
        if (string.IsNullOrWhiteSpace(certificateNumber))
        {
            return Result<bool>.Failure("CMKP certificate number is required", "CERTIFICATE_NUMBER_REQUIRED");
        }

        if (!CertificateNumberRegex.IsMatch(certificateNumber))
        {
            return Result<bool>.Failure(
                "Invalid CMKP certificate number format. Expected format: CMKP/YYYY/NUMBER", 
                "INVALID_CERTIFICATE_FORMAT");
        }

        // Extract year from certificate number
        var parts = certificateNumber.Split('/');
        if (parts.Length != 3)
        {
            return Result<bool>.Failure("Invalid certificate number structure", "INVALID_CERTIFICATE_STRUCTURE");
        }

        if (!int.TryParse(parts[1], out var year))
        {
            return Result<bool>.Failure("Invalid year in certificate number", "INVALID_CERTIFICATE_YEAR");
        }

        // Certificate year should be reasonable (not future, not too old)
        var currentYear = DateTime.UtcNow.Year;
        if (year > currentYear)
        {
            return Result<bool>.Failure("Certificate year cannot be in the future", "FUTURE_CERTIFICATE_YEAR");
        }

        if (year < currentYear - 10)
        {
            return Result<bool>.Failure("Certificate is too old (more than 10 years)", "EXPIRED_CERTIFICATE");
        }

        return Result<bool>.Success(true);
    }

    /// <summary>
    /// Validates if a course requires CMKP certification
    /// </summary>
    public bool RequiresCmkpCertification(ValueObjects.CourseType courseType)
    {
        return CmkpRequiredCourseTypes.Contains(courseType);
    }

    /// <summary>
    /// Validates course completion with CMKP requirements
    /// </summary>
    public Result<CourseValidationResult> ValidateCourse(Course course)
    {
        var result = new CourseValidationResult
        {
            CourseId = course.Id,
            CourseName = course.CourseName ?? string.Empty,
            CourseType = course.CourseType,
            IsValid = true
        };

        // Check if CMKP certification is required
        if (RequiresCmkpCertification(course.CourseType))
        {
            result.RequiresCmkpCertificate = true;

            // Validate certificate number
            if (string.IsNullOrWhiteSpace(course.CmkpCertificateNumber))
            {
                result.IsValid = false;
                result.ValidationErrors.Add("CMKP certificate number is required for this course type");
            }
            else
            {
                var certificateValidation = ValidateCertificateNumber(course.CmkpCertificateNumber);
                if (certificateValidation.IsFailure)
                {
                    result.IsValid = false;
                    result.ValidationErrors.Add(certificateValidation.Error);
                }
            }

            // Check if verified
            if (!course.IsVerifiedByCmkp)
            {
                result.IsValid = false;
                result.ValidationErrors.Add("Course must be verified by CMKP");
            }

            // Check verification date
            if (course.IsVerifiedByCmkp && !course.CmkpVerificationDate.HasValue)
            {
                result.IsValid = false;
                result.ValidationErrors.Add("CMKP verification date is required when course is verified");
            }
        }

        // Validate course duration
        if (course.DurationDays <= 0)
        {
            result.IsValid = false;
            result.ValidationErrors.Add("Course duration must be at least 1 day");
        }

        if (course.DurationHours <= 0)
        {
            result.IsValid = false;
            result.ValidationErrors.Add("Course duration must be at least 1 hour");
        }

        // Validate dates
        if (course.StartDate > course.EndDate)
        {
            result.IsValid = false;
            result.ValidationErrors.Add("Course start date cannot be after end date");
        }

        // Validate completion date is within course dates
        if (course.CompletionDate < course.StartDate || course.CompletionDate > course.EndDate)
        {
            result.IsValid = false;
            result.ValidationErrors.Add("Completion date must be within course start and end dates");
        }

        // Check if course dates are within module dates
        if (course.Module != null)
        {
            if (course.StartDate < course.Module.StartDate || course.EndDate > course.Module.EndDate)
            {
                result.IsValid = false;
                result.ValidationErrors.Add("Course dates must be within module dates");
            }
        }

        return Result<CourseValidationResult>.Success(result);
    }

    /// <summary>
    /// Validates all courses in a module for CMKP compliance
    /// </summary>
    public async Task<Result<ModuleCmkpValidation>> ValidateModuleCourses(
        Module module, 
        IEnumerable<Course> courses)
    {
        var validation = new ModuleCmkpValidation
        {
            ModuleId = module.Id,
            ModuleName = module.Name,
            IsValid = true
        };

        var coursesList = courses.ToList();
        validation.TotalCourses = coursesList.Count;

        foreach (var course in coursesList)
        {
            var courseValidation = ValidateCourse(course);
            if (courseValidation.IsSuccess && courseValidation.Value.IsValid)
            {
                validation.ValidCourses++;
                if (courseValidation.Value.RequiresCmkpCertificate)
                {
                    validation.CmkpCertifiedCourses++;
                }
            }
            else
            {
                validation.IsValid = false;
                if (courseValidation.IsSuccess)
                {
                    validation.InvalidCourses.Add(new InvalidCourseInfo
                    {
                        CourseId = course.Id,
                        CourseName = course.CourseName ?? string.Empty,
                        Errors = courseValidation.Value.ValidationErrors
                    });
                }
            }
        }

        // Check if required courses are present based on module type
        var requiredCourseCount = module.TotalCourses;
        if (validation.ValidCourses < requiredCourseCount)
        {
            validation.IsValid = false;
            validation.ValidationErrors.Add($"Module requires {requiredCourseCount} courses, but only {validation.ValidCourses} valid courses found");
        }

        return await Task.FromResult(Result<ModuleCmkpValidation>.Success(validation));
    }

    /// <summary>
    /// Generates a CMKP certificate number for testing purposes
    /// Note: In production, this would interface with the actual CMKP system
    /// </summary>
    public string GenerateMockCertificateNumber()
    {
        var year = DateTime.UtcNow.Year;
        var number = Random.Shared.Next(1000, 9999);
        return $"CMKP/{year}/{number}";
    }
}

/// <summary>
/// Interface for CMKP validation operations
/// </summary>
public interface ICmkpValidationService
{
    Result<bool> ValidateCertificateNumber(string? certificateNumber);
    bool RequiresCmkpCertification(ValueObjects.CourseType courseType);
    Result<CourseValidationResult> ValidateCourse(Course course);
    Task<Result<ModuleCmkpValidation>> ValidateModuleCourses(Module module, IEnumerable<Course> courses);
    string GenerateMockCertificateNumber();
}

/// <summary>
/// Result of course validation
/// </summary>
public class CourseValidationResult
{
    public CourseId CourseId { get; set; } = null!;
    public string CourseName { get; set; } = string.Empty;
    public ValueObjects.CourseType CourseType { get; set; }
    public bool IsValid { get; set; }
    public bool RequiresCmkpCertificate { get; set; }
    public List<string> ValidationErrors { get; set; } = new();
}

/// <summary>
/// Result of module CMKP validation
/// </summary>
public class ModuleCmkpValidation
{
    public ModuleId ModuleId { get; set; } = null!;
    public string ModuleName { get; set; } = string.Empty;
    public bool IsValid { get; set; }
    public int TotalCourses { get; set; }
    public int ValidCourses { get; set; }
    public int CmkpCertifiedCourses { get; set; }
    public List<InvalidCourseInfo> InvalidCourses { get; set; } = new();
    public List<string> ValidationErrors { get; set; } = new();
}

/// <summary>
/// Information about an invalid course
/// </summary>
public class InvalidCourseInfo
{
    public CourseId CourseId { get; set; } = null!;
    public string CourseName { get; set; } = string.Empty;
    public List<string> Errors { get; set; } = new();
}
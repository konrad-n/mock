using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using SledzSpecke.Application.Export.DTO;
using SledzSpecke.Core.DomainServices;

namespace SledzSpecke.Infrastructure.Export;

public sealed class SmkValidator : ISmkValidator
{
    private readonly ILogger<SmkValidator> _logger;
    private static readonly Regex DateFormatRegex = new(@"^\d{2}\.\d{2}\.\d{4}$");
    private static readonly Regex TimeFormatRegex = new(@"^\d{2}:\d{2}$");

    public SmkValidator(ILogger<SmkValidator> logger)
    {
        _logger = logger;
    }

    public Task<ValidationReport> ValidateAsync(SpecializationExportDto data)
    {
        _logger.LogInformation("Validating specialization export data for SMK compliance");

        var errors = new List<ValidationError>();
        var warnings = new List<ValidationWarning>();

        // Validate basic info
        ValidateBasicInfo(data.BasicInfo, errors, warnings);

        // Validate internships
        ValidateInternships(data.Internships, errors, warnings);

        // Validate courses
        ValidateCourses(data.Courses, errors, warnings);

        // Validate medical shifts
        ValidateMedicalShifts(data.MedicalShifts, errors, warnings);

        // Validate procedures
        ValidateProcedures(data.Procedures, data.BasicInfo.SmkVersion, errors, warnings);

        // Validate self-education days
        ValidateSelfEducationDays(data.AdditionalSelfEducationDays, errors, warnings);

        // Cross-validation
        PerformCrossValidation(data, errors, warnings);

        var report = new ValidationReport
        {
            IsValid = !errors.Any(),
            Errors = errors,
            Warnings = warnings,
            ValidationDate = DateTime.UtcNow,
            SmkVersion = data.BasicInfo.SmkVersion
        };

        _logger.LogInformation("Validation completed: {ErrorCount} errors, {WarningCount} warnings", 
            errors.Count, warnings.Count);

        return Task.FromResult(report);
    }

    private void ValidateBasicInfo(BasicInfoExportDto basicInfo, List<ValidationError> errors, List<ValidationWarning> warnings)
    {
        // Required fields
        if (string.IsNullOrWhiteSpace(basicInfo.Pesel))
        {
            errors.Add(new ValidationError
            {
                Field = "PESEL",
                Message = "PESEL is required",
                ErrorCode = "PESEL_REQUIRED",
                Severity = ValidationSeverity.Critical
            });
        }
        else if (basicInfo.Pesel.Length != 11)
        {
            errors.Add(new ValidationError
            {
                Field = "PESEL",
                Message = "PESEL must be 11 digits",
                ErrorCode = "PESEL_INVALID_LENGTH",
                Severity = ValidationSeverity.Critical
            });
        }

        if (string.IsNullOrWhiteSpace(basicInfo.PwzNumber))
        {
            errors.Add(new ValidationError
            {
                Field = "PWZ",
                Message = "PWZ number is required",
                ErrorCode = "PWZ_REQUIRED",
                Severity = ValidationSeverity.Critical
            });
        }
        else if (basicInfo.PwzNumber.Length != 7)
        {
            errors.Add(new ValidationError
            {
                Field = "PWZ",
                Message = "PWZ must be 7 digits",
                ErrorCode = "PWZ_INVALID_LENGTH",
                Severity = ValidationSeverity.Critical
            });
        }

        if (string.IsNullOrWhiteSpace(basicInfo.FirstName))
        {
            errors.Add(new ValidationError
            {
                Field = "FirstName",
                Message = "First name is required",
                ErrorCode = "FIRSTNAME_REQUIRED",
                Severity = ValidationSeverity.Critical
            });
        }

        if (string.IsNullOrWhiteSpace(basicInfo.LastName))
        {
            errors.Add(new ValidationError
            {
                Field = "LastName",
                Message = "Last name is required",
                ErrorCode = "LASTNAME_REQUIRED",
                Severity = ValidationSeverity.Critical
            });
        }

        // SMK version validation
        if (basicInfo.SmkVersion != "old" && basicInfo.SmkVersion != "new")
        {
            errors.Add(new ValidationError
            {
                Field = "SmkVersion",
                Message = "SMK version must be 'old' or 'new'",
                ErrorCode = "SMK_VERSION_INVALID",
                Severity = ValidationSeverity.Critical
            });
        }

        // Date format validation
        if (!string.IsNullOrEmpty(basicInfo.SpecializationStartDate) && !DateFormatRegex.IsMatch(basicInfo.SpecializationStartDate))
        {
            errors.Add(new ValidationError
            {
                Field = "SpecializationStartDate",
                Message = "Date must be in DD.MM.YYYY format",
                ErrorCode = "DATE_FORMAT_INVALID",
                Severity = ValidationSeverity.Error
            });
        }

        // Optional fields warnings
        if (string.IsNullOrWhiteSpace(basicInfo.CorrespondenceAddress))
        {
            warnings.Add(new ValidationWarning
            {
                Field = "CorrespondenceAddress",
                Message = "Correspondence address is recommended",
                WarningCode = "ADDRESS_MISSING"
            });
        }
    }

    private void ValidateInternships(List<InternshipExportDto> internships, List<ValidationError> errors, List<ValidationWarning> warnings)
    {
        if (!internships.Any())
        {
            warnings.Add(new ValidationWarning
            {
                Field = "Internships",
                Message = "No internships found",
                WarningCode = "NO_INTERNSHIPS"
            });
        }

        foreach (var internship in internships)
        {
            if (string.IsNullOrWhiteSpace(internship.InternshipName))
            {
                errors.Add(new ValidationError
                {
                    Field = "InternshipName",
                    Message = $"Internship name is required",
                    ErrorCode = "INTERNSHIP_NAME_REQUIRED",
                    Severity = ValidationSeverity.Error
                });
            }

            if (!DateFormatRegex.IsMatch(internship.StartDate))
            {
                errors.Add(new ValidationError
                {
                    Field = "Internship.StartDate",
                    Message = $"Internship '{internship.InternshipName}' start date must be in DD.MM.YYYY format",
                    ErrorCode = "DATE_FORMAT_INVALID",
                    Severity = ValidationSeverity.Error
                });
            }

            if (!DateFormatRegex.IsMatch(internship.EndDate))
            {
                errors.Add(new ValidationError
                {
                    Field = "Internship.EndDate",
                    Message = $"Internship '{internship.InternshipName}' end date must be in DD.MM.YYYY format",
                    ErrorCode = "DATE_FORMAT_INVALID",
                    Severity = ValidationSeverity.Error
                });
            }

            if (internship.DurationDays <= 0)
            {
                errors.Add(new ValidationError
                {
                    Field = "Internship.Duration",
                    Message = $"Internship '{internship.InternshipName}' duration must be positive",
                    ErrorCode = "INVALID_DURATION",
                    Severity = ValidationSeverity.Error
                });
            }

            if (string.IsNullOrWhiteSpace(internship.SupervisorName))
            {
                warnings.Add(new ValidationWarning
                {
                    Field = "Internship.SupervisorName",
                    Message = $"Supervisor name missing for internship '{internship.InternshipName}'",
                    WarningCode = "SUPERVISOR_MISSING"
                });
            }
        }
    }

    private void ValidateCourses(List<CourseExportDto> courses, List<ValidationError> errors, List<ValidationWarning> warnings)
    {
        var mandatoryCourses = courses.Where(c => c.CourseType == "Obowiązkowy").ToList();
        if (!mandatoryCourses.Any())
        {
            warnings.Add(new ValidationWarning
            {
                Field = "Courses",
                Message = "No mandatory courses found",
                WarningCode = "NO_MANDATORY_COURSES"
            });
        }

        foreach (var course in courses)
        {
            if (string.IsNullOrWhiteSpace(course.CourseName))
            {
                errors.Add(new ValidationError
                {
                    Field = "CourseName",
                    Message = "Course name is required",
                    ErrorCode = "COURSE_NAME_REQUIRED",
                    Severity = ValidationSeverity.Error
                });
            }

            if (!DateFormatRegex.IsMatch(course.StartDate) || !DateFormatRegex.IsMatch(course.EndDate))
            {
                errors.Add(new ValidationError
                {
                    Field = "Course.Dates",
                    Message = $"Course '{course.CourseName}' dates must be in DD.MM.YYYY format",
                    ErrorCode = "DATE_FORMAT_INVALID",
                    Severity = ValidationSeverity.Error
                });
            }

            if (course.CreditHours <= 0)
            {
                warnings.Add(new ValidationWarning
                {
                    Field = "Course.CreditHours",
                    Message = $"Course '{course.CourseName}' has no credit hours",
                    WarningCode = "NO_CREDIT_HOURS"
                });
            }
        }
    }

    private void ValidateMedicalShifts(List<MedicalShiftExportDto> shifts, List<ValidationError> errors, List<ValidationWarning> warnings)
    {
        if (!shifts.Any())
        {
            warnings.Add(new ValidationWarning
            {
                Field = "MedicalShifts",
                Message = "No medical shifts found",
                WarningCode = "NO_MEDICAL_SHIFTS"
            });
        }

        var shiftsByMonth = shifts.GroupBy(s => 
        {
            if (DateTime.TryParseExact(s.Date, "dd.MM.yyyy", null, System.Globalization.DateTimeStyles.None, out var date))
            {
                return new { date.Year, date.Month };
            }
            return new { Year = 0, Month = 0 };
        });

        foreach (var monthGroup in shiftsByMonth.Where(g => g.Key.Year > 0))
        {
            var totalMinutes = 0;
            foreach (var shift in monthGroup)
            {
                // Parse duration HH:MM
                var durationParts = shift.Duration.Split(':');
                if (durationParts.Length == 2 && 
                    int.TryParse(durationParts[0], out var hours) && 
                    int.TryParse(durationParts[1], out var minutes))
                {
                    totalMinutes += hours * 60 + minutes;
                }
            }

            var totalHours = totalMinutes / 60;
            if (totalHours < 160)
            {
                warnings.Add(new ValidationWarning
                {
                    Field = "MedicalShifts.MonthlyMinimum",
                    Message = $"Month {monthGroup.Key.Month}/{monthGroup.Key.Year} has only {totalHours} hours (minimum 160)",
                    WarningCode = "MONTHLY_HOURS_BELOW_MINIMUM"
                });
            }
        }

        foreach (var shift in shifts)
        {
            if (!DateFormatRegex.IsMatch(shift.Date))
            {
                errors.Add(new ValidationError
                {
                    Field = "MedicalShift.Date",
                    Message = $"Medical shift date must be in DD.MM.YYYY format",
                    ErrorCode = "DATE_FORMAT_INVALID",
                    Severity = ValidationSeverity.Error
                });
            }

            if (!TimeFormatRegex.IsMatch(shift.StartTime) || !TimeFormatRegex.IsMatch(shift.EndTime))
            {
                errors.Add(new ValidationError
                {
                    Field = "MedicalShift.Time",
                    Message = $"Medical shift times must be in HH:MM format",
                    ErrorCode = "TIME_FORMAT_INVALID",
                    Severity = ValidationSeverity.Error
                });
            }

            if (string.IsNullOrWhiteSpace(shift.Location))
            {
                warnings.Add(new ValidationWarning
                {
                    Field = "MedicalShift.Location",
                    Message = $"Medical shift on {shift.Date} has no location",
                    WarningCode = "LOCATION_MISSING"
                });
            }
        }
    }

    private void ValidateProcedures(List<ProcedureExportDto> procedures, string smkVersion, List<ValidationError> errors, List<ValidationWarning> warnings)
    {
        if (!procedures.Any())
        {
            warnings.Add(new ValidationWarning
            {
                Field = "Procedures",
                Message = "No procedures found",
                WarningCode = "NO_PROCEDURES"
            });
        }

        foreach (var procedure in procedures)
        {
            if (string.IsNullOrWhiteSpace(procedure.ProcedureCode))
            {
                errors.Add(new ValidationError
                {
                    Field = "ProcedureCode",
                    Message = "Procedure code is required",
                    ErrorCode = "PROCEDURE_CODE_REQUIRED",
                    Severity = ValidationSeverity.Error
                });
            }

            if (!DateFormatRegex.IsMatch(procedure.Date))
            {
                errors.Add(new ValidationError
                {
                    Field = "Procedure.Date",
                    Message = $"Procedure date must be in DD.MM.YYYY format",
                    ErrorCode = "DATE_FORMAT_INVALID",
                    Severity = ValidationSeverity.Error
                });
            }

            if (smkVersion == "old")
            {
                if (string.IsNullOrWhiteSpace(procedure.Role) || (procedure.Role != "A" && procedure.Role != "B"))
                {
                    errors.Add(new ValidationError
                    {
                        Field = "Procedure.Role",
                        Message = $"Procedure role must be 'A' or 'B' for old SMK",
                        ErrorCode = "INVALID_ROLE",
                        Severity = ValidationSeverity.Error
                    });
                }
            }
            else // new SMK
            {
                if (!procedure.ProcedureRequirementId.HasValue || procedure.ProcedureRequirementId.Value <= 0)
                {
                    errors.Add(new ValidationError
                    {
                        Field = "Procedure.RequirementId",
                        Message = $"Procedure requirement ID is required for new SMK",
                        ErrorCode = "REQUIREMENT_ID_MISSING",
                        Severity = ValidationSeverity.Error
                    });
                }
            }
        }
    }

    private void ValidateSelfEducationDays(List<AdditionalSelfEducationExportDto> selfEducationDays, List<ValidationError> errors, List<ValidationWarning> warnings)
    {
        var daysByYear = selfEducationDays.GroupBy(d =>
        {
            if (DateTime.TryParseExact(d.StartDate, "dd.MM.yyyy", null, System.Globalization.DateTimeStyles.None, out var date))
            {
                return date.Year;
            }
            return 0;
        });

        foreach (var yearGroup in daysByYear.Where(g => g.Key > 0))
        {
            var totalDays = yearGroup.Sum(d => d.NumberOfDays);
            if (totalDays > 6)
            {
                errors.Add(new ValidationError
                {
                    Field = "SelfEducationDays",
                    Message = $"Year {yearGroup.Key} has {totalDays} self-education days (maximum 6)",
                    ErrorCode = "SELF_EDUCATION_DAYS_EXCEEDED",
                    Severity = ValidationSeverity.Error
                });
            }
        }

        foreach (var selfEdu in selfEducationDays)
        {
            if (!DateFormatRegex.IsMatch(selfEdu.StartDate) || !DateFormatRegex.IsMatch(selfEdu.EndDate))
            {
                errors.Add(new ValidationError
                {
                    Field = "SelfEducation.Dates",
                    Message = "Self-education dates must be in DD.MM.YYYY format",
                    ErrorCode = "DATE_FORMAT_INVALID",
                    Severity = ValidationSeverity.Error
                });
            }

            if (selfEdu.NumberOfDays <= 0 || selfEdu.NumberOfDays > 6)
            {
                errors.Add(new ValidationError
                {
                    Field = "SelfEducation.Days",
                    Message = $"Self-education days must be between 1 and 6",
                    ErrorCode = "INVALID_DAY_COUNT",
                    Severity = ValidationSeverity.Error
                });
            }
        }
    }

    private void PerformCrossValidation(SpecializationExportDto data, List<ValidationError> errors, List<ValidationWarning> warnings)
    {
        // Check if procedures match internships
        var internshipNames = data.Internships.Select(i => i.InternshipName).ToHashSet();
        var proceduresWithInternship = data.Procedures
            .Where(p => !string.IsNullOrEmpty(p.InternshipName))
            .Where(p => !internshipNames.Contains(p.InternshipName))
            .ToList();

        foreach (var procedure in proceduresWithInternship)
        {
            warnings.Add(new ValidationWarning
            {
                Field = "Procedure.Internship",
                Message = $"Procedure references unknown internship '{procedure.InternshipName}'",
                WarningCode = "UNKNOWN_INTERNSHIP_REFERENCE"
            });
        }

        // Check module consistency
        var moduleNames = data.Internships.Select(i => i.ModuleName)
            .Union(data.Courses.Select(c => c.ModuleName))
            .Union(data.MedicalShifts.Select(s => s.ModuleName))
            .Union(data.Procedures.Select(p => p.ModuleName))
            .Distinct()
            .Where(m => !string.IsNullOrEmpty(m))
            .ToList();

        if (moduleNames.Count > 2)
        {
            warnings.Add(new ValidationWarning
            {
                Field = "Modules",
                Message = $"Found {moduleNames.Count} different module names, expected maximum 2 (Basic and Specialistic)",
                WarningCode = "TOO_MANY_MODULES"
            });
        }
    }
}
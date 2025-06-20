using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using SledzSpecke.Application.Export.DTO;
using SledzSpecke.Core.Abstractions;
using SledzSpecke.Core.DomainServices;
using SledzSpecke.Core.Entities;
using SledzSpecke.Core.Repositories;
using SledzSpecke.Core.ValueObjects;
using SledzSpecke.Infrastructure.DAL;

namespace SledzSpecke.Infrastructure.Export;

public sealed class SmkExportService : ISpecializationExportService
{
    private readonly SledzSpeckeDbContext _context;
    private readonly ISpecializationRepository _specializationRepository;
    private readonly ISmkExcelGenerator _excelGenerator;
    private readonly ISmkValidator _smkValidator;
    private readonly ILogger<SmkExportService> _logger;

    public SmkExportService(
        SledzSpeckeDbContext context,
        ISpecializationRepository specializationRepository,
        ISmkExcelGenerator excelGenerator,
        ISmkValidator smkValidator,
        ILogger<SmkExportService> logger)
    {
        _context = context;
        _specializationRepository = specializationRepository;
        _excelGenerator = excelGenerator;
        _smkValidator = smkValidator;
        _logger = logger;
    }

    public async Task<Result<byte[]>> ExportToXlsxAsync(int specializationId)
    {
        try
        {
            _logger.LogInformation("Starting XLSX export for specialization {SpecializationId}", specializationId);

            // Load complete specialization data
            var exportData = await LoadCompleteSpecializationDataAsync(specializationId);
            if (exportData == null)
            {
                return Result<byte[]>.Failure($"Specialization with ID {specializationId} not found", "SPECIALIZATION_NOT_FOUND");
            }

            // Validate data
            var validationResult = await _smkValidator.ValidateAsync(exportData);
            if (!validationResult.IsValid)
            {
                var errors = string.Join(", ", validationResult.Errors.Select(e => e.Message));
                return Result<byte[]>.Failure($"Export validation failed: {errors}", "VALIDATION_FAILED");
            }

            // Generate Excel file
            var excelBytes = await _excelGenerator.GenerateAsync(exportData);
            
            _logger.LogInformation("Successfully generated XLSX export for specialization {SpecializationId}", specializationId);
            return Result<byte[]>.Success(excelBytes);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error generating XLSX export for specialization {SpecializationId}", specializationId);
            return Result<byte[]>.Failure("An error occurred while generating the export", "EXPORT_ERROR");
        }
    }

    public async Task<Result<ExportPreview>> PreviewExportAsync(int specializationId)
    {
        try
        {
            var exportData = await LoadCompleteSpecializationDataAsync(specializationId);
            if (exportData == null)
            {
                return Result<ExportPreview>.Failure($"Specialization with ID {specializationId} not found", "SPECIALIZATION_NOT_FOUND");
            }

            var validationResult = await _smkValidator.ValidateAsync(exportData);
            
            var preview = new ExportPreview
            {
                SpecializationId = specializationId,
                UserName = $"{exportData.BasicInfo.FirstName} {exportData.BasicInfo.LastName}",
                SpecializationName = exportData.BasicInfo.SpecializationName,
                SmkVersion = exportData.BasicInfo.SmkVersion,
                TotalInternships = exportData.Internships.Count,
                TotalCourses = exportData.Courses.Count,
                TotalMedicalShifts = exportData.MedicalShifts.Count,
                TotalProcedures = exportData.Procedures.Count,
                TotalSelfEducationDays = exportData.AdditionalSelfEducationDays.Count,
                ValidationStatus = validationResult.IsValid 
                    ? (validationResult.Warnings.Any() ? ExportValidationStatus.HasWarnings : ExportValidationStatus.Valid)
                    : ExportValidationStatus.HasErrors,
                ValidationWarnings = validationResult.Warnings.Select(w => w.Message).ToList()
            };

            return Result<ExportPreview>.Success(preview);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error generating export preview for specialization {SpecializationId}", specializationId);
            return Result<ExportPreview>.Failure("An error occurred while generating the preview", "PREVIEW_ERROR");
        }
    }

    public async Task<Result<ValidationReport>> ValidateForExportAsync(int specializationId)
    {
        try
        {
            var exportData = await LoadCompleteSpecializationDataAsync(specializationId);
            if (exportData == null)
            {
                return Result<ValidationReport>.Failure($"Specialization with ID {specializationId} not found", "SPECIALIZATION_NOT_FOUND");
            }

            var validationReport = await _smkValidator.ValidateAsync(exportData);
            return Result<ValidationReport>.Success(validationReport);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error validating export for specialization {SpecializationId}", specializationId);
            return Result<ValidationReport>.Failure("An error occurred during validation", "VALIDATION_ERROR");
        }
    }

    private async Task<SpecializationExportDto> LoadCompleteSpecializationDataAsync(int specializationId)
    {
        var specialization = await _context.Specializations
            .Include(s => s.Modules)
            .FirstOrDefaultAsync(s => s.SpecializationId == specializationId);

        if (specialization == null)
        {
            return null;
        }

        // Load the user separately
        var user = await _context.Users
            .FirstOrDefaultAsync(u => u.UserId == specialization.UserId);

        if (user == null)
        {
            return null;
        }

        // Load internships with medical shifts
        var moduleIds = specialization.Modules.Select(m => m.ModuleId).ToList();
        var internships = await _context.Internships
            .Include(i => i.MedicalShifts)
            .Where(i => i.ModuleId.HasValue && moduleIds.Contains(i.ModuleId.Value))
            .ToListAsync();

        // Load courses
        var courses = await _context.Courses
            .Where(c => moduleIds.Contains(c.ModuleId))
            .ToListAsync();

        // Load procedure realizations for the user
        var procedureRealizations = await _context.ProcedureRealizations
            .Include(pr => pr.Requirement)
            .Where(pr => pr.UserId == user.UserId)
            .ToListAsync();

        // Load additional self-education days
        var selfEducationDays = await _context.Set<AdditionalSelfEducationDays>()
            .Where(d => moduleIds.Contains(d.ModuleId))
            .ToListAsync();

        var currentModule = specialization.Modules
            .OrderByDescending(m => m.StartDate)
            .FirstOrDefault();

        var exportDto = new SpecializationExportDto
        {
            BasicInfo = new BasicInfoExportDto
            {
                FirstName = user.Name ?? "",
                LastName = "", // User now has single Name property
                Email = user.Email ?? "",
                PhoneNumber = user.PhoneNumber ?? "",
                SpecializationName = specialization.Name,
                SmkVersion = specialization.SmkVersion.ToString().ToLower(),
                ProgramVariant = specialization.ProgramVariant ?? "",
                PlannedPesYear = specialization.PlannedPesYear.ToString(),
                SpecializationStartDate = specialization.StartDate.ToString("dd.MM.yyyy"),
                SpecializationEndDate = specialization.PlannedEndDate.ToString("dd.MM.yyyy"),
                CurrentModuleName = currentModule?.Name ?? "",
                CurrentModuleStartDate = currentModule?.StartDate.ToString("dd.MM.yyyy") ?? "",
                CorrespondenceAddress = user.CorrespondenceAddress?.ToString() ?? ""
            }
        };

        // Map internships
        foreach (var module in specialization.Modules)
        {
            var moduleInternships = internships.Where(i => i.ModuleId == module.ModuleId).ToList();
            foreach (var internship in moduleInternships)
            {
                exportDto.Internships.Add(new InternshipExportDto
                {
                    InternshipName = $"{internship.InstitutionName} - {internship.DepartmentName}", // Combine institution and department
                    InstitutionName = internship.InstitutionName ?? "",
                    DepartmentName = internship.DepartmentName ?? "",
                    StartDate = internship.StartDate.ToString("dd.MM.yyyy"),
                    EndDate = internship.EndDate.ToString("dd.MM.yyyy"),
                    DurationDays = (internship.EndDate - internship.StartDate).Days + 1,
                    SupervisorName = internship.SupervisorName ?? "",
                    ModuleName = module.Name,
                    Status = internship.IsCompleted ? "Zakończony" : "W trakcie"
                });

                // Map medical shifts
                foreach (var shift in internship.MedicalShifts)
                {
                    var totalMinutes = shift.Hours * 60 + shift.Minutes;
                    var hours = totalMinutes / 60;
                    var minutes = totalMinutes % 60;
                    
                    exportDto.MedicalShifts.Add(new MedicalShiftExportDto
                    {
                        Date = shift.Date.ToString("dd.MM.yyyy"),
                        StartTime = "00:00", // Start/End times not tracked, only duration
                        EndTime = $"{hours:D2}:{minutes:D2}", // Show end time as duration from 00:00
                        Duration = $"{hours:D2}:{minutes:D2}",
                        Location = shift.Location ?? "",
                        InternshipName = $"{internship.InstitutionName} - {internship.DepartmentName}",
                        ModuleName = module.Name,
                        SupervisorName = shift.ApproverName ?? "", // Use approver as supervisor
                        Notes = shift.AdditionalFields ?? "" // Use AdditionalFields for notes
                    });
                }
            }

            // Map courses
            var moduleCourses = courses.Where(c => c.ModuleId == module.ModuleId).ToList();
            foreach (var course in moduleCourses)
            {
                exportDto.Courses.Add(new CourseExportDto
                {
                    CourseName = course.CourseName ?? "",
                    CourseNumber = course.CourseNumber ?? "",
                    Provider = course.InstitutionName ?? "",
                    StartDate = course.CompletionDate.ToString("dd.MM.yyyy"), // Using completion date for both
                    EndDate = course.CompletionDate.ToString("dd.MM.yyyy"),   // as Course only has CompletionDate
                    CreditHours = 0, // Not tracked in current Course entity
                    CourseType = course.CourseType == SledzSpecke.Core.ValueObjects.CourseType.Specialization ? "Obowiązkowy" : "Dodatkowy",
                    ModuleName = module.Name,
                    CertificateNumber = course.CertificateNumber ?? "",
                    Status = course.HasCertificate ? "Zakończony" : "W trakcie"
                });
            }

            // Map procedure realizations for this module
            // Filter procedure realizations that belong to this module (through requirement)
            var moduleProcedureRealizations = procedureRealizations
                .Where(pr => pr.Requirement != null && pr.Requirement.ModuleId == module.ModuleId)
                .ToList();
                
            foreach (var realization in moduleProcedureRealizations)
            {
                ProcedureExportDto procedureDto;

                // For old SMK
                if (specialization.SmkVersion == Core.Enums.SmkVersion.Old)
                {
                    procedureDto = new ProcedureExportDto
                    {
                        ProcedureCode = realization.Requirement?.Code ?? "",
                        ProcedureName = realization.Requirement?.Name ?? "",
                        Date = realization.Date.ToString("dd.MM.yyyy"),
                        Location = realization.Location ?? "",
                        ModuleName = module.Name,
                        Role = realization.Role == ProcedureRole.Operator ? "A" : "B",
                        ProcedureRequirementId = realization.RequirementId.Value,
                        YearOfTraining = realization.Year?.ToString() ?? "",
                        // Note: PatientInitials, PatientGender, InternshipName, AssistantData are not 
                        // available in the new ProcedureRealization model - these would need to be added
                        // if they are required for export
                        PatientInitials = "",
                        PatientGender = "",
                        InternshipName = "",
                        FirstAssistantData = "",
                        SecondAssistantData = ""
                    };
                }
                else // new SMK
                {
                    procedureDto = new ProcedureExportDto
                    {
                        ProcedureCode = realization.Requirement?.Code ?? "",
                        ProcedureName = realization.Requirement?.Name ?? "",
                        Date = realization.Date.ToString("dd.MM.yyyy"),
                        Location = realization.Location ?? "",
                        ModuleName = module.Name,
                        Role = realization.Role == ProcedureRole.Operator ? "A" : "B",
                        ProcedureRequirementId = realization.RequirementId.Value,
                        // For new SMK, we need to count procedures by role
                        // This would require aggregating realizations by requirement and role
                        // For now, we'll just export individual realizations
                        CountA = realization.Role == ProcedureRole.Operator ? 1 : 0,
                        CountB = realization.Role == ProcedureRole.Assistant ? 1 : 0,
                        // Note: Supervisor field is not available in ProcedureRealization
                        Supervisor = ""
                    };
                }

                exportDto.Procedures.Add(procedureDto);
            }

            // Map self-education days for this module
            var moduleSelfEducation = selfEducationDays
                .Where(d => d.ModuleId == module.ModuleId)
                .ToList();

            foreach (var selfEdu in moduleSelfEducation)
            {
                var internship = moduleInternships.FirstOrDefault(i => i.InternshipId == selfEdu.InternshipId);
                
                exportDto.AdditionalSelfEducationDays.Add(new AdditionalSelfEducationExportDto
                {
                    StartDate = selfEdu.StartDate.ToString("dd.MM.yyyy"),
                    EndDate = selfEdu.EndDate.ToString("dd.MM.yyyy"),
                    NumberOfDays = selfEdu.NumberOfDays,
                    Purpose = selfEdu.Purpose ?? "",
                    EventName = selfEdu.EventName ?? "",
                    ModuleName = module.Name,
                    InternshipName = internship != null ? $"{internship.InstitutionName} - {internship.DepartmentName}" : ""
                });
            }
        }

        return exportDto;
    }
}

public interface ISmkExcelGenerator
{
    Task<byte[]> GenerateAsync(SpecializationExportDto data);
}

public interface ISmkValidator
{
    Task<ValidationReport> ValidateAsync(SpecializationExportDto data);
}
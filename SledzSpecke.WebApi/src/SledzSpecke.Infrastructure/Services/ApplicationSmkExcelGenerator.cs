using SledzSpecke.Application.Abstractions;
using SledzSpecke.Application.Export.DTO;
using SledzSpecke.Core.DomainServices;
using SledzSpecke.Core.Entities;

namespace SledzSpecke.Infrastructure.Services;

/// <summary>
/// Adapter that implements the Application layer's ISmkExcelGenerator interface
/// by delegating to the existing Export.SmkExcelGenerator
/// </summary>
public sealed class ApplicationSmkExcelGenerator : ISmkExcelGenerator
{
    private readonly Export.ISmkExcelGenerator _exportGenerator;
    
    public ApplicationSmkExcelGenerator(Export.ISmkExcelGenerator exportGenerator)
    {
        _exportGenerator = exportGenerator;
    }
    
    public async Task<byte[]> GenerateSmkExportAsync(SmkExportData exportData)
    {
        // Convert SmkExportData to SpecializationExportDto
        var exportDto = ConvertToExportDto(exportData);
        
        // Use the existing export generator
        return await _exportGenerator.GenerateAsync(exportDto);
    }
    
    private SpecializationExportDto ConvertToExportDto(SmkExportData data)
    {
        return new SpecializationExportDto
        {
            BasicInfo = new BasicInfoExportDto
            {
                FirstName = data.User.Name.Split(' ').FirstOrDefault() ?? "",
                LastName = data.User.Name.Split(' ').Skip(1).FirstOrDefault() ?? "",
                Email = data.User.Email,
                PhoneNumber = data.User.PhoneNumber,
                SpecializationName = data.Specialization.Name,
                SmkVersion = data.SmkVersion.ToString(),
                ProgramVariant = data.Specialization.ProgramVariant,
                PlannedPesYear = data.Specialization.PlannedPesYear.ToString(),
                SpecializationStartDate = data.Specialization.StartDate.ToString("dd.MM.yyyy"),
                SpecializationEndDate = data.Specialization.PlannedEndDate.ToString("dd.MM.yyyy"),
                CurrentModuleName = data.Modules.FirstOrDefault(m => m.ModuleId == data.Specialization.CurrentModuleId)?.Name ?? "",
                CurrentModuleStartDate = data.Modules.FirstOrDefault(m => m.ModuleId == data.Specialization.CurrentModuleId)?.StartDate.ToString("dd.MM.yyyy") ?? "",
                CorrespondenceAddress = data.User.CorrespondenceAddress ?? ""
            },
            
            Internships = data.Internships.Select(i => new InternshipExportDto
            {
                InternshipName = i.Name,
                InstitutionName = i.InstitutionName,
                DepartmentName = i.DepartmentName,
                StartDate = i.StartDate.ToString("dd.MM.yyyy"),
                EndDate = i.EndDate.ToString("dd.MM.yyyy"),
                DurationDays = i.DaysCount,
                SupervisorName = i.SupervisorName ?? "",
                ModuleName = data.Modules.FirstOrDefault(m => m.ModuleId == i.ModuleId)?.Name ?? "",
                Status = i.Status.ToString()
            }).ToList(),
            
            Courses = data.Courses.Select(c => new CourseExportDto
            {
                CourseName = c.CourseName,
                CourseNumber = c.CourseNumber ?? "",
                Provider = c.OrganizerName,
                StartDate = c.StartDate.ToString("dd.MM.yyyy"),
                EndDate = c.EndDate.ToString("dd.MM.yyyy"),
                CreditHours = c.DurationHours,
                CourseType = c.CourseType.ToString(),
                ModuleName = data.Modules.FirstOrDefault(m => m.ModuleId == c.ModuleId)?.Name ?? "",
                CertificateNumber = c.CmkpCertificateNumber ?? "",
                Status = c.IsApproved ? "Zatwierdzony" : "Oczekujący"
            }).ToList(),
            
            MedicalShifts = data.MedicalShifts.Select(s => new MedicalShiftExportDto
            {
                Date = s.Date.ToString("dd.MM.yyyy"),
                StartTime = "00:00", // Medical shifts don't have start time
                EndTime = CalculateEndTime(s.Hours, s.Minutes),
                Duration = $"{s.Hours}h {s.Minutes}min",
                Location = s.Location,
                InternshipName = data.Internships.FirstOrDefault(i => i.InternshipId == s.InternshipId)?.Name ?? "",
                ModuleName = data.Modules.FirstOrDefault(m => m.ModuleId == s.ModuleId)?.Name ?? "",
                SupervisorName = s.SupervisorName ?? "",
                Notes = ""
            }).ToList(),
            
            Procedures = MapProcedures(data),
            
            AdditionalSelfEducationDays = data.AdditionalDays.Select(d => new AdditionalSelfEducationExportDto
            {
                StartDate = d.StartDate.ToString("dd.MM.yyyy"),
                EndDate = d.EndDate.ToString("dd.MM.yyyy"),
                NumberOfDays = d.NumberOfDays,
                Purpose = d.Purpose,
                EventName = d.EventName,
                ModuleName = data.Modules.FirstOrDefault(m => m.ModuleId == d.ModuleId)?.Name ?? "",
                InternshipName = data.Internships.FirstOrDefault(i => i.InternshipId == d.InternshipId)?.Name ?? ""
            }).ToList()
        };
    }
    
    private string FormatAddress(Core.ValueObjects.Address? address)
    {
        if (address == null) return "";
        
        return $"{address.Street} {address.HouseNumber}" +
               (!string.IsNullOrEmpty(address.ApartmentNumber) ? $"/{address.ApartmentNumber}" : "") +
               $", {address.PostalCode} {address.City}";
    }
    
    private string CalculateEndTime(int hours, int minutes)
    {
        var totalMinutes = hours * 60 + minutes;
        var endHours = totalMinutes / 60;
        var endMinutes = totalMinutes % 60;
        return $"{endHours:00}:{endMinutes:00}";
    }
    
    private List<ProcedureExportDto> MapProcedures(SmkExportData data)
    {
        var procedures = new List<ProcedureExportDto>();
        
        // Group realizations by requirement to get module information
        var requirementMap = data.ProcedureRequirements.ToDictionary(r => r.Id, r => r);
        
        foreach (var realization in data.ProcedureRealizations)
        {
            ProcedureRequirement? requirement = null;
            if (requirementMap.TryGetValue(realization.RequirementId, out var req))
            {
                requirement = req;
            }
            
            var moduleName = "";
            if (requirement != null)
            {
                var module = data.Modules.FirstOrDefault(m => m.ModuleId == requirement.ModuleId);
                moduleName = module?.Name ?? "";
            }
            
            ProcedureExportDto procedureDto;
            
            // For old SMK
            if (data.SmkVersion.Value == "old")
            {
                procedureDto = new ProcedureExportDto
                {
                    ProcedureCode = requirement?.Code ?? "",
                    ProcedureName = requirement?.Name ?? "",
                    Date = realization.Date.ToString("dd.MM.yyyy"),
                    Location = realization.Location ?? "",
                    ModuleName = moduleName,
                    Role = realization.Role == Core.ValueObjects.ProcedureRole.Operator ? "A" : "B",
                    ProcedureRequirementId = realization.RequirementId.Value,
                    YearOfTraining = realization.Year?.ToString() ?? "",
                    // Note: PatientInitials, PatientGender, InternshipName, AssistantData are not available
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
                    ProcedureCode = requirement?.Code ?? "",
                    ProcedureName = requirement?.Name ?? "",
                    Date = realization.Date.ToString("dd.MM.yyyy"),
                    Location = realization.Location ?? "",
                    ModuleName = moduleName,
                    Role = realization.Role == Core.ValueObjects.ProcedureRole.Operator ? "A" : "B",
                    ProcedureRequirementId = realization.RequirementId.Value,
                    // For new SMK, we export individual realizations
                    CountA = realization.Role == Core.ValueObjects.ProcedureRole.Operator ? 1 : 0,
                    CountB = realization.Role == Core.ValueObjects.ProcedureRole.Assistant ? 1 : 0,
                    Supervisor = ""
                };
            }
            
            procedures.Add(procedureDto);
        }
        
        return procedures;
    }
}
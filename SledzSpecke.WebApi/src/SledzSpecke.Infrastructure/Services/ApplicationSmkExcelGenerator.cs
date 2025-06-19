using SledzSpecke.Application.Abstractions;
using SledzSpecke.Application.Export.DTO;
using SledzSpecke.Core.DomainServices;

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
                FirstName = data.User.FirstName.Value,
                LastName = data.User.LastName.Value,
                Email = data.User.Email.Value,
                PhoneNumber = data.User.PhoneNumber.Value,
                SpecializationName = data.Specialization.Name,
                SmkVersion = data.SmkVersion.Value,
                ProgramVariant = data.Specialization.ProgramVariant,
                PlannedPesYear = data.Specialization.PlannedPesYear.ToString(),
                SpecializationStartDate = data.Specialization.StartDate.ToString("dd.MM.yyyy"),
                SpecializationEndDate = data.Specialization.PlannedEndDate.ToString("dd.MM.yyyy"),
                CurrentModuleName = data.Modules.FirstOrDefault(m => m.Id == data.Specialization.CurrentModuleId)?.Name ?? "",
                CurrentModuleStartDate = data.Modules.FirstOrDefault(m => m.Id == data.Specialization.CurrentModuleId)?.StartDate.ToString("dd.MM.yyyy") ?? "",
                CorrespondenceAddress = FormatAddress(data.User.CorrespondenceAddress)
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
                ModuleName = data.Modules.FirstOrDefault(m => m.Id == i.ModuleId)?.Name ?? "",
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
                ModuleName = data.Modules.FirstOrDefault(m => m.Id == c.ModuleId)?.Name ?? "",
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
                ModuleName = data.Modules.FirstOrDefault(m => m.Id == s.ModuleId)?.Name ?? "",
                SupervisorName = s.SupervisorName ?? "",
                Notes = ""
            }).ToList(),
            
            Procedures = data.Procedures.Select(p => new ProcedureExportDto
            {
                ProcedureCode = p.Code,
                ProcedureName = p.Name,
                Date = p.PerformedDate.ToString("dd.MM.yyyy"),
                Location = p.Location,
                PatientInitials = p.PatientInitials ?? "",
                PatientGender = p.PatientGender?.ToString() ?? "",
                YearOfTraining = p.Year.ToString(),
                InternshipName = data.Internships.FirstOrDefault(i => i.InternshipId == p.InternshipId)?.Name ?? "",
                FirstAssistantData = p.AssistantData ?? "",
                SecondAssistantData = "",
                Role = p.ExecutionType == Core.ValueObjects.ProcedureExecutionType.CodeA ? "Operator" : "Asystent",
                ModuleName = data.Modules.FirstOrDefault(m => m.Id == p.ModuleId)?.Name ?? "",
                Supervisor = p.SupervisorName,
                ProcedureRequirementId = p is Core.Entities.ProcedureNewSmk newSmk ? newSmk.ProcedureRequirementId : null,
                CountA = p.ExecutionType == Core.ValueObjects.ProcedureExecutionType.CodeA ? 1 : 0,
                CountB = p.ExecutionType == Core.ValueObjects.ProcedureExecutionType.CodeB ? 1 : 0
            }).ToList(),
            
            AdditionalSelfEducationDays = data.AdditionalDays.Select(d => new AdditionalSelfEducationExportDto
            {
                StartDate = d.StartDate.ToString("dd.MM.yyyy"),
                EndDate = d.EndDate.ToString("dd.MM.yyyy"),
                NumberOfDays = d.NumberOfDays,
                Purpose = d.Purpose,
                EventName = d.EventName,
                ModuleName = data.Modules.FirstOrDefault(m => m.Id == d.ModuleId)?.Name ?? "",
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
}
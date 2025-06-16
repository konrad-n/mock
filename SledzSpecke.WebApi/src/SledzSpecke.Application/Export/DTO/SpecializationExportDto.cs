using System;
using System.Collections.Generic;

namespace SledzSpecke.Application.Export.DTO;

public sealed class SpecializationExportDto
{
    public BasicInfoExportDto BasicInfo { get; init; }
    public List<InternshipExportDto> Internships { get; init; } = new();
    public List<CourseExportDto> Courses { get; init; } = new();
    public List<MedicalShiftExportDto> MedicalShifts { get; init; } = new();
    public List<ProcedureExportDto> Procedures { get; init; } = new();
    public List<AdditionalSelfEducationExportDto> AdditionalSelfEducationDays { get; init; } = new();
}

public sealed class BasicInfoExportDto
{
    public string Pesel { get; init; }
    public string PwzNumber { get; init; }
    public string FirstName { get; init; }
    public string LastName { get; init; }
    public string Email { get; init; }
    public string PhoneNumber { get; init; }
    public string SpecializationName { get; init; }
    public string SmkVersion { get; init; } // "old" or "new"
    public string ProgramVariant { get; init; }
    public string PlannedPesYear { get; init; }
    public string SpecializationStartDate { get; init; } // DD.MM.YYYY
    public string SpecializationEndDate { get; init; } // DD.MM.YYYY
    public string CurrentModuleName { get; init; }
    public string CurrentModuleStartDate { get; init; } // DD.MM.YYYY
    public string CorrespondenceAddress { get; init; }
}

public sealed class InternshipExportDto
{
    public string InternshipName { get; init; }
    public string InstitutionName { get; init; }
    public string DepartmentName { get; init; }
    public string StartDate { get; init; } // DD.MM.YYYY
    public string EndDate { get; init; } // DD.MM.YYYY
    public int DurationDays { get; init; }
    public string SupervisorName { get; init; }
    public string SupervisorPwz { get; init; }
    public string ModuleName { get; init; }
    public string Status { get; init; } // "W trakcie", "Zakończony", "Zatwierdzony"
}

public sealed class CourseExportDto
{
    public string CourseName { get; init; }
    public string CourseNumber { get; init; }
    public string Provider { get; init; }
    public string StartDate { get; init; } // DD.MM.YYYY
    public string EndDate { get; init; } // DD.MM.YYYY
    public int CreditHours { get; init; }
    public string CourseType { get; init; } // "Obowiązkowy", "Dodatkowy"
    public string ModuleName { get; init; }
    public string CertificateNumber { get; init; }
    public string Status { get; init; } // "W trakcie", "Zakończony"
}

public sealed class MedicalShiftExportDto
{
    public string Date { get; init; } // DD.MM.YYYY
    public string StartTime { get; init; } // HH:MM
    public string EndTime { get; init; } // HH:MM
    public string Duration { get; init; } // HH:MM (can be > 24:00)
    public string Location { get; init; }
    public string InternshipName { get; init; }
    public string ModuleName { get; init; }
    public string SupervisorName { get; init; }
    public string Notes { get; init; }
}

public sealed class ProcedureExportDto
{
    public string ProcedureCode { get; init; }
    public string ProcedureName { get; init; }
    public string Date { get; init; } // DD.MM.YYYY
    public string Location { get; init; }
    public string ModuleName { get; init; }
    
    // For old SMK
    public string PatientInitials { get; init; }
    public string PatientGender { get; init; } // "M" or "K"
    public string YearOfTraining { get; init; }
    public string InternshipName { get; init; }
    public string FirstAssistantData { get; init; }
    public string SecondAssistantData { get; init; }
    public string Role { get; init; } // "A" or "B"
    
    // For new SMK
    public int? CountA { get; init; } // Performed count
    public int? CountB { get; init; } // Assisted count
    public string Supervisor { get; init; }
    public int? ProcedureRequirementId { get; init; }
}

public sealed class AdditionalSelfEducationExportDto
{
    public string StartDate { get; init; } // DD.MM.YYYY
    public string EndDate { get; init; } // DD.MM.YYYY
    public int NumberOfDays { get; init; }
    public string Purpose { get; init; }
    public string EventName { get; init; }
    public string ModuleName { get; init; }
    public string InternshipName { get; init; }
}
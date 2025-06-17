using SledzSpecke.Api.Validation;
using SledzSpecke.Application.DTO;
using System.ComponentModel.DataAnnotations;

namespace SledzSpecke.Api.Models;

public class CreateUserRequest
{
    [Required]
    [EmailAddress]
    public string Email { get; set; }

    [Required]
    [MinLength(3)]
    public string FirstName { get; set; }

    [Required]
    [MinLength(2)]
    public string LastName { get; set; }

    [Required]
    [Pesel]
    public string Pesel { get; set; }

    [Required]
    [Pwz]
    public string PwzNumber { get; set; }

    [Required]
    public AddressDto CorrespondenceAddress { get; set; }

    [PolishPhoneNumber]
    public string? Phone { get; set; }
}

public class AddMedicalShiftRequest
{
    [Required]
    [NotFutureDate]
    public DateTime Date { get; set; }

    [Required]
    [Range(0, 24)]
    public int Hours { get; set; }

    [Required]
    [ShiftDuration]
    public int Minutes { get; set; }

    public string? Location { get; set; }
}

public class AddProcedureRequest
{
    [Required]
    public int InternshipId { get; set; }

    [Required]
    [NotFutureDate]
    public DateTime Date { get; set; }

    [Required]
    [MedicalYear]
    public int Year { get; set; }

    [Required]
    [MinLength(3)]
    public string Code { get; set; }

    [Required]
    public string Name { get; set; }

    [Required]
    public string Location { get; set; }

    [Required]
    [ProcedureExecutionType]
    public string ExecutionType { get; set; }

    public string? SupervisorName { get; set; }

    [Pwz]
    public string? SupervisorPwz { get; set; }
}

public class CreateSpecializationRequest
{
    [Required]
    public string Name { get; set; }

    [Required]
    [MinLength(3)]
    [MaxLength(10)]
    public string SmkCode { get; set; }

    [Required]
    [SmkVersion]
    public string SmkVersion { get; set; }

    [Required]
    [SpecializationDuration]
    public int TotalMonths { get; set; }

    public bool IsActive { get; set; } = true;
}

public class CreateModuleRequest
{
    [Required]
    public int SpecializationId { get; set; }

    [Required]
    public string Name { get; set; }

    [Required]
    [SmkModuleType]
    public string ModuleType { get; set; }

    [Required]
    public DateTime StartDate { get; set; }

    [Required]
    public DateTime EndDate { get; set; }

    public string? Description { get; set; }
}

public class CreateInternshipRequest
{
    [Required]
    public string Name { get; set; }

    [Required]
    public string InstitutionName { get; set; }

    [Required]
    public string Department { get; set; }

    [Required]
    public DateTime StartDate { get; set; }

    [Required]
    public DateTime PlannedEndDate { get; set; }

    public string? SupervisorName { get; set; }

    [Pwz]
    public string? SupervisorPwz { get; set; }
}

public class CreateCourseRequest
{
    [Required]
    public string CourseName { get; set; }

    [Required]
    public string CourseNumber { get; set; }

    [Required]
    public string InstitutionName { get; set; }

    [Required]
    [NotFutureDate]
    public DateTime StartDate { get; set; }

    [Required]
    public DateTime EndDate { get; set; }

    [Required]
    [Range(1, 365)]
    public int DurationDays { get; set; }

    [Required]
    [Range(1, 8760)] // Max hours in a year
    public int DurationHours { get; set; }

    public string? CmkpCertificateNumber { get; set; }
}

public class UpdateUserProfileRequest
{
    [MinLength(3)]
    public string? FirstName { get; set; }

    [MinLength(2)]
    public string? LastName { get; set; }

    [Pesel]
    public string? Pesel { get; set; }

    [Pwz]
    public string? PwzNumber { get; set; }

    [PolishPhoneNumber]
    public string? Phone { get; set; }

    public AddressDto? CorrespondenceAddress { get; set; }
}

public class ValidatedAddressDto
{
    [Required]
    public string Street { get; set; }

    [Required]
    public string BuildingNumber { get; set; }

    public string? ApartmentNumber { get; set; }

    [Required]
    public string City { get; set; }

    [Required]
    [PolishPostalCode]
    public string PostalCode { get; set; }

    [Required]
    public string Province { get; set; }

    public string Country { get; set; } = "Poland";
}

public class AdditionalSelfEducationDaysRequest
{
    [Required]
    public int SpecializationId { get; set; }

    [Required]
    [Range(2020, 2100)]
    public int Year { get; set; }

    [Required]
    [Range(1, 6)]
    public int DaysUsed { get; set; }

    public string? Comment { get; set; }
}

public class MedicalShiftBulkRequest
{
    [Required]
    public int InternshipId { get; set; }

    [Required]
    [MinLength(1)]
    [MaxLength(31)] // Max days in a month
    public List<MedicalShiftEntry> Shifts { get; set; }
}

public class MedicalShiftEntry
{
    [Required]
    [NotFutureDate]
    public DateTime Date { get; set; }

    [Required]
    [Range(0, 24)]
    public int Hours { get; set; }

    [Required]
    [ShiftDuration]
    public int Minutes { get; set; }

    public string? Location { get; set; }
}
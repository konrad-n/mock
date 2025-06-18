using SledzSpecke.Core.Entities;
using SledzSpecke.Core.ValueObjects;
using SledzSpecke.Application.Commands;
using System;

namespace SledzSpecke.Tests.Integration.Helpers;

public static class TestDataFactory
{
    private static readonly Random _random = new();

    public static User CreateTestUser(
        int? id = null,
        string? email = null,
        string? password = null,
        string? firstName = null,
        string? lastName = null,
        string? pesel = null,
        string? pwzNumber = null,
        string? phoneNumber = null,
        DateTime? dateOfBirth = null,
        Address? address = null)
    {
        var user = User.Create(
            new Email(email ?? $"test{Guid.NewGuid():N}@example.com"),
            new HashedPassword(password ?? "$2a$10$8JJ.Xzx8JJ.Xzx8JJ.Xzx8JJ.Xzx8JJ.Xzx8JJ.Xzx8JJ.Xzx8JJ"),
            new FirstName(firstName ?? "Test"),
            null, // secondName
            new LastName(lastName ?? "User"),
            new Pesel(pesel ?? "90010112345"),
            new PwzNumber(pwzNumber ?? "1234567"),
            new PhoneNumber(phoneNumber ?? "+48123456789"),
            dateOfBirth ?? new DateTime(1990, 1, 1),
            address ?? new Address(
                "Test Street 1",
                "00-001",
                "Warsaw",
                "Poland"
            )
        );
        
        // If specific ID requested, we'll need to use reflection or another approach
        // For now, the ID will be set by the repository
        return user;
    }

    public static Internship CreateTestInternship(
        InternshipId? id = null,
        SpecializationId? specializationId = null,
        string? institution = null,
        string? department = null,
        string? supervisor = null,
        DateTime? startDate = null,
        DateTime? endDate = null,
        int year = 1,
        int supervisorPesel = 2024)
    {
        return Internship.Create(
            id ?? InternshipId.New(),
            specializationId ?? new SpecializationId(1),
            institution ?? "Test Hospital",
            department ?? "Test Department",
            supervisor ?? "Dr. Supervisor",
            startDate ?? DateTime.UtcNow.Date,
            endDate ?? DateTime.UtcNow.Date.AddMonths(6),
            year,
            supervisorPesel
        );
    }

    public static MedicalShift CreateTestMedicalShift(
        MedicalShiftId? id = null,
        InternshipId? internshipId = null,
        ModuleId? moduleId = null,
        DateTime? date = null,
        int hours = 8,
        int minutes = 0,
        ShiftType? shiftType = null,
        string? location = null,
        string? supervisor = null,
        int year = 2024)
    {
        return MedicalShift.Create(
            id ?? MedicalShiftId.New(),
            internshipId ?? new InternshipId(1),
            moduleId,
            date ?? DateTime.UtcNow.Date,
            hours,
            minutes,
            shiftType ?? ShiftType.Accompanying,
            location ?? "Emergency Department",
            supervisor ?? "Dr. Smith",
            year
        );
    }

    public static Procedure CreateTestProcedure(
        ProcedureId? id = null,
        ModuleId? moduleId = null,
        InternshipId? internshipId = null,
        DateTime? date = null,
        string? name = null,
        string? code = null,
        string? supervisorName = null,
        ProcedureExecutionType? executionType = null,
        string? location = null,
        int year = 2024,
        SmkVersion? smkVersion = null)
    {
        return Procedure.Create(
            id ?? ProcedureId.New(),
            moduleId ?? new ModuleId(1),
            internshipId ?? new InternshipId(1),
            date ?? DateTime.UtcNow.Date,
            name ?? "Test Procedure",
            code ?? "P001",
            supervisorName ?? "Dr. Johnson",
            executionType ?? ProcedureExecutionType.CodeA,
            location ?? "Test Hospital",
            year,
            smkVersion ?? new SmkVersion("new")
        );
    }

    public static Specialization CreateTestSpecialization(
        SpecializationId? id = null,
        UserId? userId = null,
        string? name = null,
        string? programCode = null,
        SmkVersion? smkVersion = null,
        string? programVariant = null,
        DateTime? startDate = null,
        DateTime? plannedEndDate = null,
        int plannedPesYear = 1,
        string? programStructure = null,
        int durationYears = 5)
    {
        return new Specialization(
            id ?? new SpecializationId(_random.Next(1000, 9999)),
            userId ?? new UserId(1),
            name ?? "Test Specialization",
            programCode ?? "SPEC001",
            smkVersion ?? new SmkVersion("new"),
            programVariant ?? "Standard",
            startDate ?? DateTime.UtcNow,
            plannedEndDate ?? DateTime.UtcNow.AddYears(durationYears),
            plannedPesYear,
            programStructure ?? "Basic+Specialized",
            durationYears
        );
    }

    public static class Commands
    {
        public static AddMedicalShift CreateAddMedicalShiftCommand(
            int? internshipId = null,
            DateTime? date = null,
            int hours = 8,
            int minutes = 0,
            string? location = null,
            int year = 2024)
        {
            return new AddMedicalShift(
                internshipId ?? 1,
                date ?? DateTime.Today,
                hours,
                minutes,
                location ?? "Emergency Department",
                year
            );
        }
    }

    public static class ValueObjects
    {
        public static Email Email(string? value = null) 
            => new(value ?? $"test{Guid.NewGuid():N}@example.com");
            
        public static DateRange DateRange(DateTime? start = null, DateTime? end = null)
            => new(start ?? DateTime.Today, end ?? DateTime.Today.AddDays(30));
            
        public static Pesel Pesel(string? value = null)
            => new(value ?? "90010112345");
            
        public static PwzNumber Pwz(string? value = null)
            => new(value ?? "1234567");
            
        public static PhoneNumber Phone(string? value = null)
            => new(value ?? "+48123456789");
    }
}
using Bogus;
using SledzSpecke.Core.Entities;
using SledzSpecke.Core.ValueObjects;
using System;
using System.Collections.Generic;

namespace SledzSpecke.Tests.Integration.Common;

public static class TestDataFactory
{
    private static readonly Faker Faker = new();

    public static User CreateUser(string? email = null, string? role = null)
    {
        email ??= Faker.Internet.Email();
        var password = new HashedPassword("$2a$10$abcdefghijklmnopqrstuvwxyz123456789012345678901234567890"); // BCrypt hash format
        var firstName = new FirstName(Faker.Name.FirstName());
        var secondName = Faker.Random.Bool() ? new SecondName(Faker.Name.FirstName()) : null;
        var lastName = new LastName(Faker.Name.LastName());
        var dateOfBirth = new DateTime(1970 + Faker.Random.Int(0, 29), Faker.Random.Int(1, 12), Faker.Random.Int(1, 28));
        var gender = Faker.Random.Bool() ? Gender.Male : Gender.Female;
        var pesel = new Pesel(Pesel.GenerateValidPesel(dateOfBirth, gender, Faker.Random.Int(0, 999)));
        var pwzNumber = new PwzNumber(PwzNumber.GenerateValidPwz(Faker.Random.Int(0, 999999)));
        var phoneNumber = new PhoneNumber($"+48{Faker.Random.Int(100000000, 999999999):D9}");
        var address = new Address(
            Faker.Address.StreetName(),
            Faker.Random.Int(1, 200).ToString(),
            Faker.Random.Bool() ? Faker.Random.Int(1, 100).ToString() : null,
            $"{Faker.Random.Int(10, 99):D2}-{Faker.Random.Int(100, 999):D3}",
            Faker.Address.City(),
            Faker.PickRandom("Mazowieckie", "Małopolskie", "Śląskie", "Wielkopolskie", "Dolnośląskie"),
            "Polska"
        );
        
        return User.Create(
            new Email(email),
            password,
            firstName,
            secondName,
            lastName,
            pesel,
            pwzNumber,
            phoneNumber,
            dateOfBirth,
            address);
    }

    public static Specialization CreateSpecialization(SmkVersion? smkVersion = null, int userId = 1)
    {
        var specializationId = SpecializationId.New();
        var programCode = Faker.Random.AlphaNumeric(6).ToUpper();
        var name = Faker.Lorem.Word();
        var actualSmkVersion = smkVersion ?? Faker.PickRandom<SmkVersion>();
        var startDate = Faker.Date.Recent(30);
        var plannedEndDate = startDate.AddYears(5);
        var durationYears = 5;
        
        var specialization = new Specialization(
            specializationId,
            new UserId(userId),
            name,
            programCode,
            actualSmkVersion,
            "Standard", // programVariant
            startDate,
            plannedEndDate,
            1, // plannedPesYear
            "Basic + Specialized", // programStructure
            durationYears);
            
        return specialization;
    }

    public static Module CreateModule(int specializationId, int moduleNumber = 1)
    {
        var moduleId = ModuleId.New();
        var moduleName = $"Module {moduleNumber}";
        var startDate = Faker.Date.Recent(30);
        var endDate = startDate.AddMonths(3);
        var moduleType = moduleNumber == 1 ? ModuleType.Basic : ModuleType.Specialistic;
        
        return new Module(
            moduleId,
            new SpecializationId(specializationId),
            moduleType,
            SmkVersion.New,
            "1.0", // version
            moduleName,
            startDate,
            endDate,
            "Standard Structure"); // structure
    }

    public static Internship CreateInternship(
        int specializationId,
        DateTime? startDate = null,
        DateTime? endDate = null,
        int? moduleId = null)
    {
        var internshipId = InternshipId.New();
        var institutionName = Faker.Company.CompanyName();
        var departmentName = Faker.Commerce.Department();
        startDate ??= Faker.Date.Recent(30);
        endDate ??= startDate.Value.AddMonths(2);
        
        var internship = Internship.Create(
            internshipId,
            new SpecializationId(specializationId),
            $"{institutionName} Internship", // name
            institutionName,
            departmentName,
            startDate.Value,
            endDate.Value,
            8, // plannedWeeks
            40); // plannedDays
            
        if (moduleId.HasValue)
        {
            internship.AssignToModule(moduleId.Value);
        }
        
        return internship;
    }

    public static ProcedureBase CreateProcedure(
        int internshipId,
        SmkVersion smkVersion,
        DateTime? date = null,
        ProcedureStatus status = ProcedureStatus.Pending,
        int? moduleId = null,
        int year = 0)
    {
        var procedureId = ProcedureId.New();
        var procedureCode = Faker.Random.AlphaNumeric(8).ToUpper();
        var location = new Location(Faker.Address.City());
        date ??= Faker.Date.Recent(7);
        
        ProcedureBase procedure;
        
        if (smkVersion == SmkVersion.Old)
        {
            var actualModuleId = moduleId ?? Faker.Random.Int(1, 1000);
            procedure = ProcedureOldSmk.Create(
                procedureId,
                new ModuleId(actualModuleId),
                new InternshipId(internshipId),
                date.Value,
                year,
                procedureCode,
                Faker.Lorem.Sentence(3), // name
                location.Value,
                ProcedureExecutionType.CodeA,
                Faker.Name.FullName()); // supervisorName
        }
        else
        {
            var actualModuleId = moduleId ?? Faker.Random.Int(1, 1000);
            var procedureRequirementId = Faker.Random.Int(1, 100);
            var procedureName = Faker.Lorem.Sentence(3);
            
            procedure = ProcedureNewSmk.Create(
                procedureId,
                new ModuleId(actualModuleId),
                new InternshipId(internshipId),
                date.Value,
                procedureCode,
                procedureName,
                location.Value,
                ProcedureExecutionType.CodeA,
                Faker.Name.FullName(), // supervisorName
                procedureRequirementId);
        }
        
        if (status != ProcedureStatus.Pending)
        {
            procedure.ChangeStatus(status);
        }
        
        return procedure;
    }

    public static MedicalShift CreateMedicalShift(
        int internshipId,
        DateTime? date = null,
        int? hours = null,
        int? minutes = null,
        int? moduleId = null,
        int year = 1)
    {
        var shiftId = MedicalShiftId.New();
        date ??= Faker.Date.Recent(7);
        hours ??= Faker.Random.Int(1, 8);
        minutes ??= Faker.Random.Int(0, 59);
        var location = Faker.Address.City();
        var supervisorName = Faker.Name.FullName();
        var shiftType = Faker.PickRandom<ShiftType>();
        
        return MedicalShift.Create(
            shiftId,
            new InternshipId(internshipId),
            moduleId.HasValue ? new ModuleId(moduleId.Value) : null,
            date.Value,
            hours.Value,
            minutes.Value,
            shiftType,
            location,
            supervisorName,
            year);
    }

    public static Course CreateCourse(
        string? name = null,
        DateTime? startDate = null,
        DateTime? endDate = null)
    {
        var courseId = CourseId.New();
        name ??= Faker.Lorem.Word();
        var description = Faker.Lorem.Paragraph();
        startDate ??= Faker.Date.Recent(30);
        endDate ??= startDate.Value.AddDays(5);
        var credits = Faker.Random.Int(1, 10);
        
        return Course.Create(
            courseId,
            new SpecializationId(1), // default specialization
            CourseType.Mandatory,
            name,
            "Test Organizer", // organizerName
            "Test Institution", // institutionName
            startDate.Value,
            endDate.Value,
            5, // durationDays
            40); // durationHours
    }

    public static Recognition CreateRecognition(
        string? title = null,
        DateTime? date = null)
    {
        var recognitionId = RecognitionId.New();
        title ??= Faker.Lorem.Sentence(3);
        var description = Faker.Lorem.Paragraph();
        date ??= Faker.Date.Recent(30);
        var issuer = Faker.Company.CompanyName();
        
        return Recognition.Create(
            recognitionId,
            new SpecializationId(1), // default specialization
            new UserId(1), // default user
            RecognitionType.Award,
            title,
            date.Value,
            date.Value.AddDays(1), // endDate
            0); // daysReduction
    }

    public static Publication CreatePublication(
        string? title = null,
        DateTime? publicationDate = null)
    {
        var publicationId = PublicationId.New();
        title ??= Faker.Lorem.Sentence(5);
        var authors = Faker.Name.FullName();
        var journal = Faker.Lorem.Word();
        publicationDate ??= Faker.Date.Recent(90);
        var doi = $"10.{Faker.Random.Int(1000, 9999)}/{Faker.Random.AlphaNumeric(10)}";
        
        var result = Publication.Create(
            publicationId,
            new SpecializationId(1), // default specialization
            new UserId(1), // default user
            PublicationType.Journal,
            title,
            publicationDate.Value);
            
        if (!result.IsSuccess)
            throw new InvalidOperationException($"Failed to create publication: {result.Error}");
            
        var publication = result.Value;
        publication.UpdatePublicationDetails(null, null, null, doi, null, null, null);
        publication.UpdateBasicDetails(PublicationType.Journal, title, publicationDate.Value, authors, journal, null);
        
        return publication;
    }

    public static SelfEducation CreateSelfEducation(
        string? topic = null,
        DateTime? date = null)
    {
        var selfEducationId = SelfEducationId.New();
        topic ??= Faker.Lorem.Sentence(3);
        var description = Faker.Lorem.Paragraph();
        var hoursSpent = Faker.Random.Int(1, 20);
        date ??= Faker.Date.Recent(30);
        
        return SelfEducation.Create(
            selfEducationId,
            new ModuleId(1), // default module
            SelfEducationType.Literature,
            topic,
            date.Value,
            hoursSpent);
    }
}
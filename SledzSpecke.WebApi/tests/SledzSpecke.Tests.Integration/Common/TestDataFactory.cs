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
        var userId = UserId.New();
        email ??= Faker.Internet.Email();
        var fullName = Faker.Name.FullName();
        var passwordHash = "hashedPassword123";
        role ??= "User";
        
        return User.Create(userId, email, fullName, passwordHash, role);
    }

    public static Specialization CreateSpecialization(SmkVersion? smkVersion = null)
    {
        var specializationId = SpecializationId.New();
        var programCode = Faker.Random.AlphaNumeric(6).ToUpper();
        var name = Faker.Lorem.Word();
        var actualSmkVersion = smkVersion ?? Faker.PickRandom<SmkVersion>();
        
        var specialization = Specialization.Create(
            specializationId,
            programCode,
            name,
            actualSmkVersion);
            
        return specialization;
    }

    public static Module CreateModule(int specializationId, int moduleNumber = 1)
    {
        var moduleId = ModuleId.New();
        var moduleName = $"Module {moduleNumber}";
        var startDate = Faker.Date.Recent(30);
        var endDate = startDate.AddMonths(3);
        
        return Module.Create(
            moduleId,
            new SpecializationId(specializationId),
            moduleName,
            moduleNumber,
            startDate,
            endDate);
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
            institutionName,
            departmentName,
            startDate.Value,
            endDate.Value);
            
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
        var location = Faker.Address.City();
        date ??= Faker.Date.Recent(7);
        
        ProcedureBase procedure;
        
        if (smkVersion == SmkVersion.Old)
        {
            procedure = ProcedureOldSmk.Create(
                procedureId,
                new InternshipId(internshipId),
                date.Value,
                year,
                procedureCode,
                location);
        }
        else
        {
            var actualModuleId = moduleId ?? Faker.Random.Int(1, 1000);
            var procedureRequirementId = Faker.Random.Int(1, 100);
            var procedureName = Faker.Lorem.Sentence(3);
            
            procedure = ProcedureNewSmk.Create(
                procedureId,
                new InternshipId(internshipId),
                date.Value,
                procedureCode,
                location,
                new ModuleId(actualModuleId),
                procedureRequirementId,
                procedureName);
        }
        
        if (status != ProcedureStatus.Pending)
        {
            procedure.ChangeStatus(status);
        }
        
        return procedure;
    }

    public static MedicalShift CreateMedicalShift(
        int specializationId,
        DateTime? date = null,
        TimeSpan? startTime = null,
        TimeSpan? endTime = null)
    {
        var shiftId = MedicalShiftId.New();
        date ??= Faker.Date.Recent(7);
        startTime ??= new TimeSpan(8, 0, 0);
        endTime ??= new TimeSpan(16, 0, 0);
        var location = Faker.Address.City();
        var description = Faker.Lorem.Sentence();
        
        return MedicalShift.Create(
            shiftId,
            new SpecializationId(specializationId),
            date.Value,
            startTime.Value,
            endTime.Value,
            location,
            description);
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
            name,
            description,
            startDate.Value,
            endDate.Value,
            credits);
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
            title,
            description,
            date.Value,
            issuer);
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
        
        return Publication.Create(
            publicationId,
            title,
            authors,
            journal,
            publicationDate.Value,
            doi);
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
            topic,
            description,
            hoursSpent,
            date.Value);
    }
}
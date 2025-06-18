using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using SledzSpecke.Application.Abstractions;
using SledzSpecke.Core.Entities;
using SledzSpecke.Core.Repositories;
using SledzSpecke.Core.ValueObjects;
using SledzSpecke.Infrastructure.DAL;
using SledzSpecke.Tests.Integration.Common;
using Xunit;

namespace SledzSpecke.Tests.Integration.Repositories;

public class InternshipRepositoryTests : IntegrationTestBase
{
    private readonly IInternshipRepository _repository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly SledzSpeckeDbContext _dbContext;

    public InternshipRepositoryTests(SledzSpeckeApiFactory factory) : base(factory)
    {
        _repository = Scope.ServiceProvider.GetRequiredService<IInternshipRepository>();
        _unitOfWork = Scope.ServiceProvider.GetRequiredService<IUnitOfWork>();
        _dbContext = DbContext;
    }

    [Fact]
    public async Task AddAsync_Should_Add_Internship_To_Database()
    {
        // Arrange
        var specializationId = new SpecializationId(1);
        var internship = Internship.Create(
            InternshipId.New(),
            specializationId,
            "Test Internship",
            "Test Hospital",
            "Test Department",
            DateTime.UtcNow.Date,
            DateTime.UtcNow.Date.AddDays(30),
            4, // plannedWeeks
            20 // plannedDays
        );

        // Act
        await _repository.AddAsync(internship);
        await _unitOfWork.SaveChangesAsync();

        // Assert
        var savedInternship = await _repository.GetByIdAsync(internship.InternshipId);
        savedInternship.Should().NotBeNull();
        savedInternship!.InstitutionName.Should().Be("Test Hospital");
        savedInternship.DepartmentName.Should().Be("Test Department");
        savedInternship.SpecializationId.Should().Be(specializationId);
    }

    [Fact]
    public async Task UpdateAsync_Should_Update_Internship_In_Database()
    {
        // Arrange
        var internship = Internship.Create(
            InternshipId.New(),
            new SpecializationId(1),
            "Original Hospital",
            "Original Department",
            DateTime.UtcNow.Date,
            DateTime.UtcNow.Date.AddDays(30)
        );

        await _repository.AddAsync(internship);
        await _unitOfWork.SaveChangesAsync();

        // Act
        internship.UpdateInstitution("Updated Hospital", "Updated Department");
        var updateResult = internship.UpdateDates(
            DateTime.UtcNow.Date.AddDays(5),
            DateTime.UtcNow.Date.AddDays(35)
        );

        updateResult.IsSuccess.Should().BeTrue();
        
        await _repository.UpdateAsync(internship);
        await _unitOfWork.SaveChangesAsync();

        // Assert
        var updatedInternship = await _repository.GetByIdAsync(internship.InternshipId);
        updatedInternship.Should().NotBeNull();
        updatedInternship!.InstitutionName.Should().Be("Updated Hospital");
        updatedInternship.DepartmentName.Should().Be("Updated Department");
        updatedInternship.SupervisorName.Should().Be("Dr. Smith");
    }

    [Fact]
    public async Task GetBySpecializationIdAsync_Should_Return_Internships_For_Specialization()
    {
        // Arrange
        var specializationId = new SpecializationId(1);
        var otherSpecializationId = new SpecializationId(2);

        var internship1 = Internship.Create(
            InternshipId.New(),
            specializationId,
            "Internship 1",
            "Hospital 1",
            "Department 1",
            DateTime.UtcNow.Date,
            DateTime.UtcNow.Date.AddDays(30),
            4, 20
        );

        var internship2 = Internship.Create(
            InternshipId.New(),
            specializationId,
            "Internship 2",
            "Hospital 2",
            "Department 2",
            DateTime.UtcNow.Date,
            DateTime.UtcNow.Date.AddDays(60),
            8, 40
        );

        var internship3 = Internship.Create(
            InternshipId.New(),
            otherSpecializationId,
            "Internship 3",
            "Hospital 3",
            "Department 3",
            DateTime.UtcNow.Date,
            DateTime.UtcNow.Date.AddDays(90),
            12, 60
        );

        await _repository.AddAsync(internship1);
        await _repository.AddAsync(internship2);
        await _repository.AddAsync(internship3);
        await _unitOfWork.SaveChangesAsync();

        // Act
        var internships = await _repository.GetBySpecializationIdAsync(specializationId);

        // Assert
        internships.Should().HaveCount(2);
        internships.Should().Contain(i => i.InstitutionName == "Hospital 1");
        internships.Should().Contain(i => i.InstitutionName == "Hospital 2");
        internships.Should().NotContain(i => i.InstitutionName == "Hospital 3");
    }

    [Fact]
    public async Task DeleteAsync_Should_Remove_Internship_From_Database()
    {
        // Arrange
        var internship = Internship.Create(
            InternshipId.New(),
            new SpecializationId(1),
            "Test Internship",
            "Test Hospital",
            "Test Department",
            DateTime.UtcNow.Date,
            DateTime.UtcNow.Date.AddDays(30),
            4, 20
        );

        await _repository.AddAsync(internship);
        await _unitOfWork.SaveChangesAsync();

        // Act
        await _repository.DeleteAsync(internship.InternshipId);
        await _unitOfWork.SaveChangesAsync();

        // Assert
        var deletedInternship = await _repository.GetByIdAsync(internship.InternshipId);
        deletedInternship.Should().BeNull();
    }

    [Fact]
    public async Task AddMedicalShift_With_UnitOfWork_Should_Save_Correctly()
    {
        // Arrange
        var internship = Internship.Create(
            InternshipId.New(),
            new SpecializationId(1),
            "Test Internship",
            "Test Hospital",
            "Test Department",
            DateTime.UtcNow.Date,
            DateTime.UtcNow.Date.AddDays(30),
            4, 20
        );

        await _repository.AddAsync(internship);
        await _unitOfWork.SaveChangesAsync();

        // Act
        var result = internship.AddMedicalShift(
            DateTime.UtcNow.Date.AddDays(5),
            8,
            30,
            "Emergency Room",
            2,
            SmkVersion.New,
            new[] { 1, 2, 3, 4, 5 }
        );

        result.IsSuccess.Should().BeTrue();
        
        await _repository.UpdateAsync(internship);
        await _unitOfWork.SaveChangesAsync();

        // Assert
        var updatedInternship = await _repository.GetByIdAsync(internship.InternshipId);
        updatedInternship.Should().NotBeNull();
        updatedInternship!.MedicalShifts.Should().HaveCount(1);
        
        var shift = updatedInternship.MedicalShifts.First();
        shift.Hours.Should().Be(8);
        shift.Minutes.Should().Be(30);
        shift.Location.Should().Be("Emergency Room");
    }
}
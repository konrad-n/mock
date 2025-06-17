using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using SledzSpecke.Application.Commands;
using SledzSpecke.Application.MedicalShifts.Handlers;
using SledzSpecke.Core.Entities;
using SledzSpecke.Core.Repositories;
using SledzSpecke.Core.ValueObjects;
using SledzSpecke.Infrastructure.DAL;
using Xunit;
using FluentAssertions;

namespace SledzSpecke.Integration.Tests.MedicalShifts;

public class MedicalShiftWithValueObjectsTests : IClassFixture<SledzSpeckeApiFactory>
{
    private readonly SledzSpeckeApiFactory _factory;
    private readonly IServiceScope _scope;
    private readonly SledzSpeckeDbContext _dbContext;
    private readonly IMedicalShiftRepository _medicalShiftRepository;
    private readonly IInternshipRepository _internshipRepository;

    public MedicalShiftWithValueObjectsTests(SledzSpeckeApiFactory factory)
    {
        _factory = factory;
        _scope = factory.Services.CreateScope();
        _dbContext = _scope.ServiceProvider.GetRequiredService<SledzSpeckeDbContext>();
        _medicalShiftRepository = _scope.ServiceProvider.GetRequiredService<IMedicalShiftRepository>();
        _internshipRepository = _scope.ServiceProvider.GetRequiredService<IInternshipRepository>();
    }

    [Fact]
    public async Task MedicalShift_WithShiftDuration_PersistsCorrectly()
    {
        // Arrange
        var internshipId = new InternshipId(1);
        var moduleId = new ModuleId(1);
        
        var internship = CreateTestInternship(internshipId);
        await _dbContext.Internships.AddAsync(internship);
        await _dbContext.SaveChangesAsync();
        
        var shiftId = new MedicalShiftId(1);
        var duration = new ShiftDuration(8, 30); // 8 hours 30 minutes
        var shiftType = ShiftType.Accompanying;
        
        // Act
        var medicalShift = MedicalShift.Create(
            shiftId,
            internshipId,
            moduleId,
            DateTime.Today,
            duration.Hours,
            duration.Minutes,
            shiftType,
            "Test Hospital",
            "Dr. Smith",
            1
        );
        
        await _medicalShiftRepository.AddAsync(medicalShift);
        await _dbContext.SaveChangesAsync();
        
        // Assert - Reload from database
        _dbContext.ChangeTracker.Clear();
        var savedShift = await _medicalShiftRepository.GetByIdAsync(shiftId);
        
        savedShift.Should().NotBeNull();
        savedShift!.Duration.Should().NotBeNull();
        savedShift.Duration.Hours.Should().Be(8);
        savedShift.Duration.Minutes.Should().Be(30);
        savedShift.Duration.TotalMinutes.Should().Be(510);
        savedShift.Type.Should().Be(ShiftType.Accompanying);
    }

    [Fact]
    public async Task MedicalShift_WithLargeMinutes_HandlesCorrectly()
    {
        // Arrange - Test the business rule that allows minutes > 59
        var internshipId = new InternshipId(2);
        var moduleId = new ModuleId(2);
        
        var internship = CreateTestInternship(internshipId);
        await _dbContext.Internships.AddAsync(internship);
        await _dbContext.SaveChangesAsync();
        
        var shiftId = new MedicalShiftId(2);
        var duration = new ShiftDuration(0, 90); // 90 minutes
        
        // Act
        var medicalShift = MedicalShift.Create(
            shiftId,
            internshipId,
            moduleId,
            DateTime.Today,
            0,
            90, // 90 minutes input
            ShiftType.Independent,
            "Test Clinic",
            "Dr. Johnson",
            2
        );
        
        await _medicalShiftRepository.AddAsync(medicalShift);
        await _dbContext.SaveChangesAsync();
        
        // Assert
        _dbContext.ChangeTracker.Clear();
        var savedShift = await _medicalShiftRepository.GetByIdAsync(shiftId);
        
        savedShift.Should().NotBeNull();
        savedShift!.Duration.TotalMinutes.Should().Be(90);
        // The display formatting would show this as "1h 30m" but internal storage is 90 minutes
        savedShift.Hours.Should().Be(1);
        savedShift.Minutes.Should().Be(30);
    }

    [Fact]
    public async Task AddMedicalShiftHandler_WithValueObjects_CreatesShiftSuccessfully()
    {
        // Arrange
        var internshipId = new InternshipId(3);
        var specializationId = new SpecializationId(1);
        
        var internship = CreateTestInternship(internshipId, specializationId);
        await _dbContext.Internships.AddAsync(internship);
        
        var specialization = CreateTestSpecialization(specializationId);
        await _dbContext.Specializations.AddAsync(specialization);
        
        await _dbContext.SaveChangesAsync();
        
        var handler = _scope.ServiceProvider.GetRequiredService<AddMedicalShiftHandlerEnhanced>();
        
        var command = new AddMedicalShift(
            internshipId.Value,
            DateTime.Today,
            8,
            45,
            "Emergency Department",
            1
        );
        
        // Act
        var shiftId = await handler.HandleAsync(command);
        
        // Assert
        var createdShift = await _medicalShiftRepository.GetByIdAsync(new MedicalShiftId(shiftId));
        
        createdShift.Should().NotBeNull();
        createdShift!.Duration.Hours.Should().Be(8);
        createdShift!.Duration.Minutes.Should().Be(45);
        createdShift!.Duration.TotalMinutes.Should().Be(525);
        createdShift!.Location.Should().Be("Emergency Department");
        createdShift!.InternshipId.Should().Be(internshipId);
    }

    [Fact]
    public async Task MedicalShift_UpdateDuration_RaisesDomainEvent()
    {
        // Arrange
        var internshipId = new InternshipId(4);
        var moduleId = new ModuleId(4);
        
        var internship = CreateTestInternship(internshipId);
        await _dbContext.Internships.AddAsync(internship);
        await _dbContext.SaveChangesAsync();
        
        var shiftId = new MedicalShiftId(4);
        var medicalShift = MedicalShift.Create(
            shiftId,
            internshipId,
            moduleId,
            DateTime.Today,
            6,
            0,
            ShiftType.Accompanying,
            "Test Location",
            "Dr. Brown",
            1
        );
        
        await _medicalShiftRepository.AddAsync(medicalShift);
        await _dbContext.SaveChangesAsync();
        
        // Act
        _dbContext.ChangeTracker.Clear();
        var shiftToUpdate = await _medicalShiftRepository.GetByIdAsync(shiftId);
        shiftToUpdate!.UpdateShiftDetails(8, 30, "Updated Location");
        
        // Assert
        shiftToUpdate.Duration.Hours.Should().Be(8);
        shiftToUpdate.Duration.Minutes.Should().Be(30);
        shiftToUpdate.Location.Should().Be("Updated Location");
        shiftToUpdate.DomainEvents.Should().ContainSingle(e => e is MedicalShiftDurationChanged);
        
        await _dbContext.SaveChangesAsync();
    }

    private Internship CreateTestInternship(InternshipId id, SpecializationId? specializationId = null)
    {
        return new Internship
        {
            Id = id,
            UserId = new UserId(1),
            SpecializationId = specializationId ?? new SpecializationId(1),
            StartDate = DateTime.Today.AddMonths(-1),
            EndDate = DateTime.Today.AddMonths(11),
            InstitutionName = "Test Hospital",
            DepartmentName = "Internal Medicine",
            SupervisorName = "Dr. Test Supervisor",
            IsCompleted = false,
            CreatedAt = DateTime.UtcNow
        };
    }

    private Specialization CreateTestSpecialization(SpecializationId id)
    {
        return new Specialization
        {
            Id = id,
            Name = "Internal Medicine",
            Code = "IM",
            SmkVersion = SmkVersion.Old,
            DurationInYears = 5,
            CreatedAt = DateTime.UtcNow
        };
    }

    public void Dispose()
    {
        _scope?.Dispose();
    }
}
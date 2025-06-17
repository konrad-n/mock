using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using SledzSpecke.Core.DomainEvents;
using SledzSpecke.Core.DomainEvents.Base;
using SledzSpecke.Core.Entities;
using SledzSpecke.Core.Repositories;
using SledzSpecke.Core.ValueObjects;
using SledzSpecke.Infrastructure.DAL;
using Xunit;
using FluentAssertions;
using System.Linq;

namespace SledzSpecke.Integration.Tests.DomainEvents;

public class MedicalShiftDomainEventsTests : IClassFixture<SledzSpeckeApiFactory>
{
    private readonly SledzSpeckeApiFactory _factory;
    private readonly IServiceScope _scope;
    private readonly SledzSpeckeDbContext _dbContext;
    private readonly IMedicalShiftRepository _medicalShiftRepository;

    public MedicalShiftDomainEventsTests(SledzSpeckeApiFactory factory)
    {
        _factory = factory;
        _scope = factory.Services.CreateScope();
        _dbContext = _scope.ServiceProvider.GetRequiredService<SledzSpeckeDbContext>();
        _medicalShiftRepository = _scope.ServiceProvider.GetRequiredService<IMedicalShiftRepository>();
    }

    [Fact]
    public void Create_MedicalShift_RaisesMedicalShiftCreatedEvent()
    {
        // Arrange
        var shiftId = new MedicalShiftId(100);
        var internshipId = new InternshipId(10);
        var moduleId = new ModuleId(5);
        var date = DateTime.Today;
        var hours = 8;
        var minutes = 30;
        var shiftType = ShiftType.Accompanying;
        var location = "Emergency Room";
        var supervisorName = "Dr. Event Test";
        var year = 3;

        // Act
        var medicalShift = MedicalShift.Create(
            shiftId,
            internshipId,
            moduleId,
            date,
            hours,
            minutes,
            shiftType,
            location,
            supervisorName,
            year
        );

        // Assert
        medicalShift.DomainEvents.Should().HaveCount(1);
        
        var createdEvent = medicalShift.DomainEvents.First();
        createdEvent.Should().BeOfType<MedicalShiftCreated>();
        
        var typedEvent = (MedicalShiftCreated)createdEvent;
        typedEvent.ShiftId.Should().Be(shiftId);
        typedEvent.InternshipId.Should().Be(internshipId);
        typedEvent.Date.Should().Be(date);
        typedEvent.Duration.TotalMinutes.Should().Be(510); // 8*60 + 30
        typedEvent.Type.Should().Be(shiftType);
        typedEvent.Location.Should().Be(location);
        typedEvent.OccurredAt.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(1));
    }

    [Fact]
    public void UpdateShiftDetails_WithDifferentDuration_RaisesDurationChangedEvent()
    {
        // Arrange
        var medicalShift = MedicalShift.Create(
            new MedicalShiftId(101),
            new InternshipId(11),
            new ModuleId(6),
            DateTime.Today,
            6,
            0,
            ShiftType.Independent,
            "ICU",
            "Dr. Update Test",
            2
        );

        medicalShift.ClearDomainEvents(); // Clear the creation event

        // Act
        medicalShift.UpdateShiftDetails(8, 45, "Updated ICU");

        // Assert
        medicalShift.DomainEvents.Should().HaveCount(1);
        
        var changedEvent = medicalShift.DomainEvents.First();
        changedEvent.Should().BeOfType<MedicalShiftDurationChanged>();
        
        var typedEvent = (MedicalShiftDurationChanged)changedEvent;
        typedEvent.ShiftId.Should().Be(new MedicalShiftId(101));
        typedEvent.OldDuration.TotalMinutes.Should().Be(360); // 6*60
        typedEvent.NewDuration.TotalMinutes.Should().Be(525); // 8*60 + 45
    }

    [Fact]
    public void UpdateShiftDetails_WithSameDuration_DoesNotRaiseDurationChangedEvent()
    {
        // Arrange
        var medicalShift = MedicalShift.Create(
            new MedicalShiftId(102),
            new InternshipId(12),
            new ModuleId(7),
            DateTime.Today,
            8,
            0,
            ShiftType.Accompanying,
            "Ward A",
            "Dr. No Change",
            1
        );

        medicalShift.ClearDomainEvents();

        // Act
        medicalShift.UpdateShiftDetails(8, 0, "Ward B"); // Same duration, different location

        // Assert
        medicalShift.DomainEvents.Should().BeEmpty();
        medicalShift.Location.Should().Be("Ward B");
    }

    [Fact]
    public void Approve_MedicalShift_RaisesCompletedEvent()
    {
        // Arrange
        var shiftId = new MedicalShiftId(103);
        var internshipId = new InternshipId(13);
        var medicalShift = MedicalShift.Create(
            shiftId,
            internshipId,
            new ModuleId(8),
            DateTime.Today.AddDays(-5),
            10,
            15,
            ShiftType.Independent,
            "Surgery",
            "Dr. Approve Test",
            4
        );

        // First, set it as synced (requirement for approval)
        medicalShift.SetSyncStatus(SyncStatus.Synced);
        medicalShift.ClearDomainEvents();

        // Act
        var approverName = "Prof. Chief";
        var approverRole = "Department Head";
        medicalShift.Approve(approverName, approverRole);

        // Assert
        medicalShift.DomainEvents.Should().HaveCount(1);
        
        var completedEvent = medicalShift.DomainEvents.First();
        completedEvent.Should().BeOfType<MedicalShiftCompleted>();
        
        var typedEvent = (MedicalShiftCompleted)completedEvent;
        typedEvent.ShiftId.Should().Be(shiftId);
        typedEvent.InternshipId.Should().Be(internshipId);
        typedEvent.Duration.TotalMinutes.Should().Be(615); // 10*60 + 15
        typedEvent.Type.Should().Be(ShiftType.Independent);
        typedEvent.ApprovalDate.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(1));
        
        medicalShift.IsApproved.Should().BeTrue();
        medicalShift.ApproverName.Should().Be(approverName);
        medicalShift.ApproverRole.Should().Be(approverRole);
    }

    [Fact]
    public async Task DomainEvents_PersistedEntity_CanBeReloaded()
    {
        // Arrange
        await SetupTestInternship();
        
        var shiftId = new MedicalShiftId(104);
        var medicalShift = MedicalShift.Create(
            shiftId,
            new InternshipId(999),
            new ModuleId(9),
            DateTime.Today,
            7,
            20,
            ShiftType.Accompanying,
            "Persistence Test Location",
            "Dr. Persistence",
            2
        );

        // Act - Save to database
        await _medicalShiftRepository.AddAsync(medicalShift);
        await _dbContext.SaveChangesAsync();

        // Clear tracking and reload
        _dbContext.ChangeTracker.Clear();
        var reloadedShift = await _medicalShiftRepository.GetByIdAsync(shiftId);

        // Assert
        reloadedShift.Should().NotBeNull();
        reloadedShift!.Duration.Hours.Should().Be(7);
        reloadedShift.Duration.Minutes.Should().Be(20);
        reloadedShift.Duration.TotalMinutes.Should().Be(440);
        reloadedShift.Type.Should().Be(ShiftType.Accompanying);
        reloadedShift.Location.Should().Be("Persistence Test Location");
        
        // Domain events are transient and not persisted
        reloadedShift.DomainEvents.Should().BeEmpty();
    }

    private async Task SetupTestInternship()
    {
        var internship = new Internship
        {
            Id = new InternshipId(999),
            UserId = new UserId(999),
            SpecializationId = new SpecializationId(999),
            StartDate = DateTime.Today.AddMonths(-1),
            EndDate = DateTime.Today.AddMonths(11),
            InstitutionName = "Test Hospital for Events",
            DepartmentName = "Test Department",
            SupervisorName = "Dr. Test Events",
            IsCompleted = false,
            CreatedAt = DateTime.UtcNow
        };

        await _dbContext.Internships.AddAsync(internship);
        await _dbContext.SaveChangesAsync();
    }

    public void Dispose()
    {
        _scope?.Dispose();
    }
}
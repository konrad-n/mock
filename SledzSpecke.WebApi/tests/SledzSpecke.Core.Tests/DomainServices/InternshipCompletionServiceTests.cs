using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Moq;
using Xunit;
using SledzSpecke.Core.Abstractions;
using SledzSpecke.Core.DomainServices;
using SledzSpecke.Core.Entities;
using SledzSpecke.Core.Repositories;
using SledzSpecke.Core.ValueObjects;

namespace SledzSpecke.Core.Tests.DomainServices;

public class InternshipCompletionServiceTests
{
    private readonly Mock<IInternshipRepository> _internshipRepositoryMock;
    private readonly Mock<IMedicalShiftRepository> _medicalShiftRepositoryMock;
    private readonly Mock<IProcedureRepository> _procedureRepositoryMock;
    private readonly Mock<ISpecializationRepository> _specializationRepositoryMock;
    private readonly Mock<IDomainEventDispatcher> _eventDispatcherMock;
    private readonly InternshipCompletionService _service;

    public InternshipCompletionServiceTests()
    {
        _internshipRepositoryMock = new Mock<IInternshipRepository>();
        _medicalShiftRepositoryMock = new Mock<IMedicalShiftRepository>();
        _procedureRepositoryMock = new Mock<IProcedureRepository>();
        _specializationRepositoryMock = new Mock<ISpecializationRepository>();
        _eventDispatcherMock = new Mock<IDomainEventDispatcher>();
        _service = new InternshipCompletionService(
            _internshipRepositoryMock.Object,
            _medicalShiftRepositoryMock.Object,
            _procedureRepositoryMock.Object,
            _specializationRepositoryMock.Object,
            _eventDispatcherMock.Object);
    }

    [Fact]
    public async Task CalculateProgressAsync_WithCompletedRequirements_ReturnsFullProgress()
    {
        // Arrange
        var internshipId = new InternshipId(1);
        var specializationId = new SpecializationId(1);
        var internship = Internship.Create(
            internshipId,
            specializationId,
            "Internship Name", // name
            "Hospital A", // institutionName
            "Department B", // departmentName
            DateTime.UtcNow.AddDays(-31), // startDate
            DateTime.UtcNow.AddDays(-1), // endDate
            4, // plannedWeeks
            20); // plannedDays

        var shifts = new List<MedicalShift>();
        // Add 180 hours of shifts (more than required 160)
        for (int i = 0; i < 30; i++)
        {
            var shift = MedicalShift.Create(
                new MedicalShiftId(i + 1),
                internshipId,
                null, // moduleId
                DateTime.UtcNow.AddDays(-30 + i), // Start from 30 days ago
                6, // 6 hours per day
                0, // minutes
                ShiftType.Independent,
                "Department B", // location
                "Dr. Smith", // supervisorName
                1); // year
            shifts.Add(shift);
        }

        var procedures = new List<ProcedureBase>();
        // Add 12 completed procedures
        for (int i = 0; i < 12; i++)
        {
            var proc = ProcedureOldSmk.Create(
                new ProcedureId(i + 1),
                null, // moduleId
                internshipId,
                DateTime.UtcNow.AddDays(-25 + i), // Within internship dates
                1,
                "PROC001",
                "Test Procedure", // name
                "Department B", // location
                ProcedureExecutionType.CodeA,
                "Dr. Smith"); // supervisorName
            proc.Complete();
            procedures.Add(proc);
        }

        var specialization = new Specialization(
            specializationId,
            new UserId(1),
            "Test Specialization",
            "TST001",
            new SmkVersion("old"),
            "standard", // programVariant
            DateTime.Today.AddYears(-1),
            DateTime.Today.AddYears(4),
            2025, // plannedPesYear
            "Standard Program",
            5);

        _internshipRepositoryMock.Setup(r => r.GetByIdAsync(internshipId))
            .ReturnsAsync(internship);
        _specializationRepositoryMock.Setup(r => r.GetByIdAsync(specializationId))
            .ReturnsAsync(specialization);

        // Act
        var result = await _service.CalculateProgressAsync(internship, shifts, procedures);

        // Assert
        Assert.True(result.IsSuccess);
        var progress = result.Value;
        Assert.True(progress.DaysProgressPercentage >= 100); // Should be 100% or more since internship ended
        Assert.Equal(31, progress.CompletedDays); // 31 days inclusive
        Assert.Equal(180, progress.CompletedShiftHours);
        Assert.True(progress.ShiftProgressPercentage >= 100);
        Assert.Equal(12, progress.CompletedProcedures);
        Assert.True(progress.MeetsAllRequirements);
        Assert.Empty(progress.UnmetRequirements);
    }

    [Fact]
    public async Task CalculateProgressAsync_WithIncompleteRequirements_ShowsPartialProgress()
    {
        // Arrange
        var internshipId = new InternshipId(1);
        var specializationId = new SpecializationId(1);
        var internship = Internship.Create(
            internshipId,
            specializationId,
            "Internship Name", // name
            "Hospital A", // institutionName
            "Department B", // departmentName
            DateTime.UtcNow.AddDays(-15), // startDate (Half way through)
            DateTime.UtcNow.AddDays(15), // endDate
            4, // plannedWeeks
            20); // plannedDays

        var shifts = new List<MedicalShift>();
        // Add only 60 hours of shifts
        for (int i = 0; i < 10; i++)
        {
            var shift = MedicalShift.Create(
                new MedicalShiftId(i + 1),
                internshipId,
                null, // moduleId
                DateTime.UtcNow.AddDays(-14 + i),
                6, // hours
                0, // minutes
                ShiftType.Independent,
                "Department B", // location
                "Dr. Smith", // supervisorName
                1); // year
            shifts.Add(shift);
        }

        var procedures = new List<ProcedureBase>();
        // Add only 5 procedures
        for (int i = 0; i < 5; i++)
        {
            var proc = ProcedureOldSmk.Create(
                new ProcedureId(i + 1),
                null, // moduleId
                internshipId,
                DateTime.UtcNow.AddDays(-10 + i),
                1,
                "PROC001",
                "Test Procedure", // name
                "Department B", // location
                ProcedureExecutionType.CodeA,
                "Dr. Smith"); // supervisorName
            proc.Complete();
            procedures.Add(proc);
        }

        var specialization = new Specialization(
            specializationId,
            new UserId(1),
            "Test Specialization",
            "TST001",
            new SmkVersion("old"),
            "standard", // programVariant
            DateTime.Today.AddYears(-1),
            DateTime.Today.AddYears(4),
            2025, // plannedPesYear
            "Standard Program",
            5);

        _internshipRepositoryMock.Setup(r => r.GetByIdAsync(internshipId))
            .ReturnsAsync(internship);
        _specializationRepositoryMock.Setup(r => r.GetByIdAsync(specializationId))
            .ReturnsAsync(specialization);

        // Act
        var result = await _service.CalculateProgressAsync(internship, shifts, procedures);

        // Assert
        Assert.True(result.IsSuccess);
        var progress = result.Value;
        // 16 days passed (including today) out of 31 total days = ~51.6%
        Assert.True(progress.DaysProgressPercentage > 50 && progress.DaysProgressPercentage < 52);
        Assert.Equal(16, progress.CompletedDays); // 16 days including today
        Assert.Equal(60, progress.CompletedShiftHours);
        Assert.True(progress.ShiftProgressPercentage < 100);
        Assert.Equal(5, progress.CompletedProcedures);
        Assert.False(progress.MeetsAllRequirements);
        Assert.NotEmpty(progress.UnmetRequirements);
        Assert.NotNull(progress.EstimatedCompletionDate);
    }

    [Fact]
    public async Task CanCompleteAsync_WithAllRequirementsMet_ReturnsTrue()
    {
        // Arrange
        var internshipId = new InternshipId(1);
        var specializationId = new SpecializationId(1);
        var internship = Internship.Create(
            internshipId,
            specializationId,
            "Internship Name", // name
            "Hospital A", // institutionName
            "Department B", // departmentName
            DateTime.UtcNow.AddDays(-31), // startDate
            DateTime.UtcNow.AddDays(-1), // endDate
            4, // plannedWeeks
            20); // plannedDays

        var shifts = CreateCompletedShifts(internshipId, 180); // Sufficient hours
        var procedures = CreateCompletedProcedures(internshipId, 12); // Sufficient procedures

        var specialization = new Specialization(
            specializationId,
            new UserId(1),
            "Test Specialization",
            "TST001",
            new SmkVersion("old"),
            "standard", // programVariant
            DateTime.Today.AddYears(-1),
            DateTime.Today.AddYears(4),
            2025, // plannedPesYear
            "Standard Program",
            5);

        _internshipRepositoryMock.Setup(r => r.GetByIdAsync(internshipId))
            .ReturnsAsync(internship);
        _specializationRepositoryMock.Setup(r => r.GetByIdAsync(specializationId))
            .ReturnsAsync(specialization);

        // Act
        var result = await _service.CanCompleteAsync(internship, shifts, procedures);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.True(result.Value);
    }

    [Fact]
    public async Task CanCompleteAsync_WithUnmetRequirements_ReturnsFalse()
    {
        // Arrange
        var internshipId = new InternshipId(1);
        var specializationId = new SpecializationId(1);
        var internship = Internship.Create(
            internshipId,
            specializationId,
            "Internship Name", // name
            "Hospital A", // institutionName
            "Department B", // departmentName
            DateTime.UtcNow.AddDays(-15), // startDate
            DateTime.UtcNow.AddDays(15), // endDate
            4, // plannedWeeks
            20); // plannedDays

        var shifts = CreateCompletedShifts(internshipId, 60); // Insufficient hours
        var procedures = CreateCompletedProcedures(internshipId, 5); // Insufficient procedures

        var specialization = new Specialization(
            specializationId,
            new UserId(1),
            "Test Specialization",
            "TST001",
            new SmkVersion("old"),
            "standard", // programVariant
            DateTime.Today.AddYears(-1),
            DateTime.Today.AddYears(4),
            2025, // plannedPesYear
            "Standard Program",
            5);

        _internshipRepositoryMock.Setup(r => r.GetByIdAsync(internshipId))
            .ReturnsAsync(internship);
        _specializationRepositoryMock.Setup(r => r.GetByIdAsync(specializationId))
            .ReturnsAsync(specialization);

        // Act
        var result = await _service.CanCompleteAsync(internship, shifts, procedures);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.False(result.Value);
    }

    [Fact]
    public async Task CompleteInternshipAsync_WithValidData_CompletesSuccessfully()
    {
        // Arrange
        var internshipId = new InternshipId(1);
        var specializationId = new SpecializationId(1);
        var userId = new UserId(1);
        var completionDate = DateTime.UtcNow;

        var internship = Internship.Create(
            internshipId,
            specializationId,
            "Internship Name", // name
            "Hospital A", // institutionName
            "Department B", // departmentName
            DateTime.UtcNow.AddDays(-31), // startDate
            DateTime.UtcNow.AddDays(-1), // endDate
            4, // plannedWeeks
            20); // plannedDays

        var shifts = CreateCompletedShifts(internshipId, 180);
        var procedures = CreateCompletedProcedures(internshipId, 12);

        var specialization = new Specialization(
            specializationId,
            new UserId(1),
            "Test Specialization",
            "TST001",
            new SmkVersion("old"),
            "standard", // programVariant
            DateTime.Today.AddYears(-1),
            DateTime.Today.AddYears(4),
            2025, // plannedPesYear
            "Standard Program",
            5);

        _internshipRepositoryMock.Setup(r => r.GetByIdAsync(internshipId))
            .ReturnsAsync(internship);
        _specializationRepositoryMock.Setup(r => r.GetByIdAsync(specializationId))
            .ReturnsAsync(specialization);
        _medicalShiftRepositoryMock.Setup(r => r.GetByInternshipIdAsync(internshipId.Value))
            .ReturnsAsync(shifts);
        _procedureRepositoryMock.Setup(r => r.GetByInternshipIdAsync(internshipId.Value))
            .ReturnsAsync(procedures);

        // Act
        var result = await _service.CompleteInternshipAsync(internshipId, userId, completionDate);

        // Assert
        Assert.True(result.IsSuccess);
        _internshipRepositoryMock.Verify(r => r.UpdateAsync(It.IsAny<Internship>()), Times.Once);
        _eventDispatcherMock.Verify(d => d.DispatchAsync(It.IsAny<InternshipCompletedEvent>(), It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task GetCompletionRequirementsAsync_ForOldSmk_ReturnsCorrectRequirements()
    {
        // Arrange
        var internshipId = new InternshipId(1);
        var specializationId = new SpecializationId(1);
        var internship = Internship.Create(
            internshipId,
            specializationId,
            "Internship Name", // name
            "Hospital A", // institutionName
            "Department B", // departmentName
            DateTime.UtcNow.AddDays(-31), // startDate
            DateTime.UtcNow.AddDays(-1), // endDate
            4, // plannedWeeks
            20); // plannedDays

        var specialization = new Specialization(
            specializationId,
            new UserId(1),
            "Test Specialization",
            "TST001",
            new SmkVersion("old"),
            "standard", // programVariant
            DateTime.Today.AddYears(-1),
            DateTime.Today.AddYears(4),
            2025, // plannedPesYear
            "Standard Program",
            5);

        _internshipRepositoryMock.Setup(r => r.GetByIdAsync(internshipId))
            .ReturnsAsync(internship);
        _specializationRepositoryMock.Setup(r => r.GetByIdAsync(specializationId))
            .ReturnsAsync(specialization);

        // Act
        var result = await _service.GetCompletionRequirementsAsync(internshipId);

        // Assert
        Assert.True(result.IsSuccess);
        var requirements = result.Value;
        Assert.Equal(31, requirements.MinimumDays); // 31 days inclusive
        Assert.Equal(160, requirements.MinimumShiftHours); // 1 month = 160 hours
        Assert.Equal(10, requirements.MinimumProcedures);
        Assert.Equal(SmkVersion.Old, requirements.SmkVersion);
        Assert.False(requirements.RequiresSupervisorApproval);
    }

    [Fact]
    public async Task GetMilestonesAsync_ReturnsCorrectMilestones()
    {
        // Arrange
        var internshipId = new InternshipId(1);
        var specializationId = new SpecializationId(1);
        var internship = Internship.Create(
            internshipId,
            specializationId,
            "Internship Name", // name
            "Hospital A", // institutionName
            "Department B", // departmentName
            DateTime.UtcNow.AddDays(-15), // startDate
            DateTime.UtcNow.AddDays(15), // endDate
            4, // plannedWeeks
            20); // plannedDays

        var shifts = CreateCompletedShifts(internshipId, 80);
        var procedures = CreateCompletedProcedures(internshipId, 5);

        var specialization = new Specialization(
            specializationId,
            new UserId(1),
            "Test Specialization",
            "TST001",
            new SmkVersion("old"),
            "standard", // programVariant
            DateTime.Today.AddYears(-1),
            DateTime.Today.AddYears(4),
            2025, // plannedPesYear
            "Standard Program",
            5);

        _internshipRepositoryMock.Setup(r => r.GetByIdAsync(internshipId))
            .ReturnsAsync(internship);
        _specializationRepositoryMock.Setup(r => r.GetByIdAsync(specializationId))
            .ReturnsAsync(specialization);
        _medicalShiftRepositoryMock.Setup(r => r.GetByInternshipIdAsync(internshipId.Value))
            .ReturnsAsync(shifts);
        _procedureRepositoryMock.Setup(r => r.GetByInternshipIdAsync(internshipId.Value))
            .ReturnsAsync(procedures);

        // Act
        var result = await _service.GetMilestonesAsync(internshipId);

        // Assert
        Assert.True(result.IsSuccess);
        var milestones = result.Value.ToList();
        Assert.NotEmpty(milestones);
        
        // Check specific milestones
        var firstWeekMilestone = milestones.First(m => m.Type == MilestoneType.FirstWeekCompleted);
        Assert.True(firstWeekMilestone.IsAchieved);
        
        var days50Milestone = milestones.First(m => m.Type == MilestoneType.Days50Percent);
        Assert.True(days50Milestone.IsAchieved);
        
        var firstProcedureMilestone = milestones.First(m => m.Type == MilestoneType.FirstProcedure);
        Assert.True(firstProcedureMilestone.IsAchieved);
    }

    private static List<MedicalShift> CreateCompletedShifts(InternshipId internshipId, int totalHours)
    {
        var shifts = new List<MedicalShift>();
        var hoursPerShift = 6;
        var shiftsNeeded = totalHours / hoursPerShift;
        
        for (int i = 0; i < shiftsNeeded; i++)
        {
            var shift = MedicalShift.Create(
                new MedicalShiftId(i + 1),
                internshipId,
                null, // moduleId
                DateTime.UtcNow.AddDays(-30 + i), // Start from 30 days ago
                hoursPerShift,
                0, // minutes
                ShiftType.Independent,
                "Department", // location
                "Dr. Smith", // supervisorName
                1); // year
            shifts.Add(shift);
        }
        
        return shifts;
    }

    private static List<ProcedureBase> CreateCompletedProcedures(InternshipId internshipId, int count)
    {
        var procedures = new List<ProcedureBase>();
        
        for (int i = 0; i < count; i++)
        {
            var proc = ProcedureOldSmk.Create(
                new ProcedureId(i + 1),
                null, // moduleId
                internshipId,
                DateTime.UtcNow.AddDays(-25 + i), // Within internship dates
                1,
                "PROC001",
                "Test Procedure", // name
                "Department", // location
                ProcedureExecutionType.CodeA,
                "Dr. Smith"); // supervisorName
            proc.Complete();
            procedures.Add(proc);
        }
        
        return procedures;
    }
}
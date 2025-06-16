using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Moq;
using Xunit;
using SledzSpecke.Core.Abstractions;
using SledzSpecke.Core.DomainServices;
using SledzSpecke.Core.Entities;
using SledzSpecke.Core.Policies;
using SledzSpecke.Core.Repositories;
using SledzSpecke.Core.ValueObjects;

namespace SledzSpecke.Core.Tests.DomainServices;

public class MedicalShiftValidationServiceTests
{
    private readonly Mock<IMedicalShiftRepository> _shiftRepositoryMock;
    private readonly Mock<ISmkPolicyFactory> _policyFactoryMock;
    private readonly Mock<ISpecializationRepository> _specializationRepositoryMock;
    private readonly MedicalShiftValidationService _service;

    public MedicalShiftValidationServiceTests()
    {
        _shiftRepositoryMock = new Mock<IMedicalShiftRepository>();
        _policyFactoryMock = new Mock<ISmkPolicyFactory>();
        _specializationRepositoryMock = new Mock<ISpecializationRepository>();
        _service = new MedicalShiftValidationService(
            _shiftRepositoryMock.Object,
            _policyFactoryMock.Object,
            _specializationRepositoryMock.Object);
    }

    [Fact]
    public async Task ValidateShiftAsync_WithValidShift_ReturnsSuccess()
    {
        // Arrange
        var userId = new UserId(1);
        var specializationId = new SpecializationId(1);
        var internshipId = new InternshipId(1);
        var shift = MedicalShift.Create(
            new MedicalShiftId(1),
            internshipId,
            null, // moduleId
            DateTime.Now,
            3, // hours
            0, // minutes
            ShiftType.Independent,
            "Department A", // location
            "Dr. Smith", // supervisorName
            1); // year

        var specialization = new Specialization(
            specializationId,
            userId,
            "Test Specialization",
            "TST001",
            new SmkVersion("old"),
            "standard", // programVariant
            DateTime.Today.AddYears(-1),
            DateTime.Today.AddYears(4),
            2025, // plannedPesYear
            "Standard Program",
            5);

        var mockPolicy = new Mock<ISmkPolicy<MedicalShift>>();
        mockPolicy.Setup(p => p.Validate(It.IsAny<MedicalShift>(), It.IsAny<SpecializationContext>()))
            .Returns(Result.Success());

        _policyFactoryMock.Setup(f => f.GetPolicy<MedicalShift>(SmkVersion.Old))
            .Returns(mockPolicy.Object);

        _shiftRepositoryMock.Setup(r => r.GetByUserIdAndDateRangeAsync(
                It.IsAny<UserId>(),
                It.IsAny<DateTime>(),
                It.IsAny<DateTime>()))
            .ReturnsAsync(new List<MedicalShift>());

        // Act
        var result = await _service.ValidateShiftAsync(shift, userId, specialization, null, new List<MedicalShift>());

        // Assert
        Assert.True(result.IsSuccess);
    }

    [Fact]
    public async Task ValidateShiftAsync_WithOverlappingShift_ReturnsFailure()
    {
        // Arrange
        var userId = new UserId(1);
        var specializationId = new SpecializationId(1);
        var internshipId = new InternshipId(1);
        var shiftDate = DateTime.Today.AddHours(8);
        
        var newShift = MedicalShift.Create(
            new MedicalShiftId(2),
            internshipId,
            null, // moduleId
            shiftDate,
            4, // hours
            0, // minutes
            ShiftType.Independent,
            "Department A", // location
            "Dr. Smith", // supervisorName
            1); // year

        var existingShift = MedicalShift.Create(
            new MedicalShiftId(1),
            internshipId,
            null, // moduleId
            shiftDate.AddHours(2), // Overlaps with new shift
            3, // hours
            0, // minutes
            ShiftType.Independent,
            "Department A", // location
            "Dr. Smith", // supervisorName
            1); // year

        var specialization = new Specialization(
            specializationId,
            userId,
            "Test Specialization",
            "TST001",
            new SmkVersion("old"),
            "standard", // programVariant
            DateTime.Today.AddYears(-1),
            DateTime.Today.AddYears(4),
            2025, // plannedPesYear
            "Standard Program",
            5);

        var mockPolicy = new Mock<ISmkPolicy<MedicalShift>>();
        mockPolicy.Setup(p => p.Validate(It.IsAny<MedicalShift>(), It.IsAny<SpecializationContext>()))
            .Returns(Result.Success());

        _policyFactoryMock.Setup(f => f.GetPolicy<MedicalShift>(SmkVersion.Old))
            .Returns(mockPolicy.Object);

        // Act
        var result = await _service.ValidateShiftAsync(newShift, userId, specialization, null, new[] { existingShift });

        // Assert
        Assert.False(result.IsSuccess);
        Assert.Contains("nakłada się", result.Error);
    }

    [Fact]
    public async Task ValidateShiftAsync_ExceedsWeeklyLimit_ReturnsFailure()
    {
        // Arrange
        var userId = new UserId(1);
        var specializationId = new SpecializationId(1);
        var internshipId = new InternshipId(1);
        var shiftDate = DateTime.Today;
        
        var newShift = MedicalShift.Create(
            new MedicalShiftId(2),
            internshipId,
            null, // moduleId
            shiftDate,
            10, // hours
            0, // minutes
            ShiftType.Independent,
            "Department A", // location
            "Dr. Smith", // supervisorName
            1); // year

        var specialization = new Specialization(
            specializationId,
            userId,
            "Test Specialization",
            "TST001",
            new SmkVersion("old"),
            "standard", // programVariant
            DateTime.Today.AddYears(-1),
            DateTime.Today.AddYears(4),
            2025, // plannedPesYear
            "Standard Program",
            5);

        // Create existing shifts that total 40 hours
        var existingShifts = new List<MedicalShift>();
        for (int i = 0; i < 4; i++)
        {
            existingShifts.Add(MedicalShift.Create(
                new MedicalShiftId(i + 10),
                internshipId,
                null, // moduleId
                shiftDate.AddDays(-i),
                10, // hours
                0, // minutes
                ShiftType.Independent,
                "Department A", // location
                "Dr. Smith", // supervisorName
                1)); // year
        }

        var mockPolicy = new Mock<ISmkPolicy<MedicalShift>>();
        mockPolicy.Setup(p => p.Validate(It.IsAny<MedicalShift>(), It.IsAny<SpecializationContext>()))
            .Returns(Result.Success());

        _policyFactoryMock.Setup(f => f.GetPolicy<MedicalShift>(SmkVersion.Old))
            .Returns(mockPolicy.Object);

        _shiftRepositoryMock.Setup(r => r.GetByUserIdAndDateRangeAsync(
                It.IsAny<UserId>(),
                It.IsAny<DateTime>(),
                It.IsAny<DateTime>()))
            .ReturnsAsync(existingShifts);

        // Act
        var result = await _service.ValidateShiftAsync(newShift, userId, specialization, null, new List<MedicalShift>());

        // Assert
        Assert.False(result.IsSuccess);
        Assert.Contains("tygodniowy limit", result.Error);
    }

    [Fact]
    public async Task CalculateMonthlyTotalAsync_ReturnsCorrectTotal()
    {
        // Arrange
        var userId = new UserId(1);
        var yearMonth = new YearMonth(2025, 6);
        var internshipId = new InternshipId(1);
        
        var shifts = new List<MedicalShift>
        {
            MedicalShift.Create(
                new MedicalShiftId(1),
                internshipId,
                null, // moduleId
                new DateTime(2025, 6, 5),
                8, // hours
                0, // minutes
                ShiftType.Independent,
                "Department A", // location
                "Dr. Smith", // supervisorName
                1), // year
            MedicalShift.Create(
                new MedicalShiftId(2),
                internshipId,
                null, // moduleId
                new DateTime(2025, 6, 10),
                6, // hours
                0, // minutes
                ShiftType.Independent,
                "Department B", // location
                "Dr. Jones", // supervisorName
                1), // year
            MedicalShift.Create(
                new MedicalShiftId(3),
                internshipId,
                null, // moduleId
                new DateTime(2025, 6, 15),
                12, // hours
                0, // minutes
                ShiftType.Independent,
                "Department C", // location
                "Dr. Brown", // supervisorName
                1) // year
        };

        _shiftRepositoryMock.Setup(r => r.GetByUserIdAndDateRangeAsync(
                userId,
                yearMonth.StartDate,
                yearMonth.EndDate))
            .ReturnsAsync(shifts);

        // Act
        var result = await _service.CalculateMonthlyTotalAsync(userId, yearMonth);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.Equal(26 * 60, result.Value.TotalMinutes); // 8 + 6 + 12 = 26 hours = 1560 minutes
    }

    [Fact]
    public async Task CheckOverlappingShiftsAsync_NoOverlap_ReturnsFalse()
    {
        // Arrange
        var userId = new UserId(1);
        var internshipId = new InternshipId(1);
        var shiftDate = DateTime.Today.AddHours(8);
        
        var newShift = MedicalShift.Create(
            new MedicalShiftId(1),
            internshipId,
            null, // moduleId
            shiftDate,
            4, // hours
            0, // minutes
            ShiftType.Independent,
            "Department A", // location
            "Dr. Smith", // supervisorName
            1); // year

        var existingShift = MedicalShift.Create(
            new MedicalShiftId(2),
            internshipId,
            null, // moduleId
            shiftDate.AddHours(5), // No overlap
            3, // hours
            0, // minutes
            ShiftType.Independent,
            "Department B", // location
            "Dr. Jones", // supervisorName
            1); // year

        // Act
        var result = await _service.CheckOverlappingShiftsAsync(newShift, new[] { existingShift });

        // Assert
        Assert.True(result.IsSuccess);
        Assert.False(result.Value);
    }

    [Fact]
    public async Task GetValidationSummaryAsync_WithValidData_ReturnsCorrectSummary()
    {
        // Arrange
        var userId = new UserId(1);
        var date = new DateTime(2025, 6, 15);
        var specializationId = new SpecializationId(1);
        var internshipId = new InternshipId(1);
        
        var specialization = new Specialization(
            specializationId,
            userId,
            "Test Specialization",
            "TST001",
            new SmkVersion("old"),
            "standard", // programVariant
            DateTime.Today.AddYears(-1),
            DateTime.Today.AddYears(4),
            2025, // plannedPesYear
            "Standard Program",
            5);

        _specializationRepositoryMock.Setup(r => r.GetByUserIdAsync(userId))
            .ReturnsAsync(new[] { specialization });

        var weeklyShifts = new List<MedicalShift>
        {
            MedicalShift.Create(
                new MedicalShiftId(1),
                internshipId,
                null, // moduleId
                date.AddDays(-1),
                10, // hours
                0, // minutes
                ShiftType.Independent,
                "Department A", // location
                "Dr. Smith", // supervisorName
                1), // year
            MedicalShift.Create(
                new MedicalShiftId(2),
                internshipId,
                null, // moduleId
                date,
                8, // hours
                0, // minutes
                ShiftType.Independent,
                "Department B", // location
                "Dr. Jones", // supervisorName
                1) // year
        };

        _shiftRepositoryMock.Setup(r => r.GetByUserIdAndDateRangeAsync(
                userId,
                It.IsAny<DateTime>(),
                It.IsAny<DateTime>()))
            .ReturnsAsync(weeklyShifts);

        // Act
        var result = await _service.GetValidationSummaryAsync(userId, date);

        // Assert
        Assert.True(result.IsSuccess);
        var summary = result.Value;
        Assert.Equal(18 * 60, summary.WeeklyTotal.TotalMinutes); // 10 + 8 = 18 hours = 1080 minutes
        Assert.Equal(48, summary.WeeklyHoursLimit);
        Assert.Equal(160, summary.MonthlyHoursMinimum);
        Assert.False(summary.ExceedsWeeklyLimit);
    }
}
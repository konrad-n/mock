using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using SledzSpecke.Core.Entities;
using SledzSpecke.Core.ValueObjects;
using SledzSpecke.Infrastructure.DAL;
using SledzSpecke.Tests.Integration.Common;
using Xunit;

namespace SledzSpecke.Tests.Integration;

public class SimpleDomainTests : IntegrationTestBase
{
    [Fact]
    public async Task Internship_Should_Save_To_Database()
    {
        // Arrange
        await ClearDatabaseAsync();
        
        var internship = Internship.Create(
            new InternshipId(1),
            new SpecializationId(1),
            "Test Hospital",
            "Test Department",
            DateTime.UtcNow.Date,
            DateTime.UtcNow.Date.AddDays(30)
        );

        // Act
        DbContext.Internships.Add(internship);
        await DbContext.SaveChangesAsync();

        // Assert
        var savedInternship = await DbContext.Internships.FindAsync(internship.InternshipId);
        savedInternship.Should().NotBeNull();
        savedInternship!.InstitutionName.Should().Be("Test Hospital");
    }

    [Fact]
    public async Task MedicalShift_Should_Be_Added_To_Internship()
    {
        // Arrange
        await ClearDatabaseAsync();
        
        var internship = Internship.Create(
            new InternshipId(1),
            new SpecializationId(1),
            "Test Hospital",
            "Test Department",
            DateTime.UtcNow.Date,
            DateTime.UtcNow.Date.AddDays(30)
        );

        // Act
        var result = internship.AddMedicalShift(
            new MedicalShiftId(1),
            DateTime.UtcNow.Date.AddDays(5),
            8,
            30,
            "Emergency Room",
            2024
        );

        // Assert
        result.IsSuccess.Should().BeTrue();
        internship.MedicalShifts.Should().HaveCount(1);
        
        var shift = internship.MedicalShifts.First();
        shift.Hours.Should().Be(8);
        shift.Minutes.Should().Be(30);
        shift.Location.Should().Be("Emergency Room");
    }

    [Fact]
    public async Task Value_Objects_Should_Work_Correctly()
    {
        // Test Duration
        var duration = new Duration(8, 30);
        duration.Hours.Should().Be(8);
        duration.Minutes.Should().Be(30);
        duration.TotalMinutes.Should().Be(510);

        // Test Email
        var email = new Email("test@example.com");
        email.Value.Should().Be("test@example.com");

        // Test Points
        var points = new Points(85.5m);
        points.Value.Should().Be(85.5m);

        // Test DateRange
        var startDate = DateTime.UtcNow.Date;
        var endDate = startDate.AddDays(30);
        var dateRange = new DateRange(startDate, endDate);
        dateRange.StartDate.Should().Be(startDate);
        dateRange.EndDate.Should().Be(endDate);
        dateRange.Contains(startDate.AddDays(15)).Should().BeTrue();
    }

    [Fact]
    public async Task Result_Pattern_Should_Handle_Success_And_Failure()
    {
        // Arrange
        var internship = Internship.Create(
            new InternshipId(1),
            new SpecializationId(1),
            "Test Hospital",
            "Test Department",
            DateTime.UtcNow.Date.AddDays(-30),
            DateTime.UtcNow.Date.AddDays(30) // Not ended yet
        );

        // Act - Try to mark as completed (should fail because not ended)
        var result = internship.MarkAsCompleted();

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Should().Contain("before the end date");
        result.ErrorCode.Should().Be("INTERN_NOT_ENDED");
        internship.IsCompleted.Should().BeFalse();

        // Act - Update dates so it's ended
        var updateResult = internship.UpdateDates(
            DateTime.UtcNow.Date.AddDays(-30),
            DateTime.UtcNow.Date.AddDays(-1)
        );
        updateResult.IsSuccess.Should().BeTrue();

        // Act - Now mark as completed (should succeed)
        var completeResult = internship.MarkAsCompleted();

        // Assert
        completeResult.IsSuccess.Should().BeTrue();
        internship.IsCompleted.Should().BeTrue();
    }
}
using FluentAssertions;
using SledzSpecke.Core.Entities;
using SledzSpecke.Core.ValueObjects;
using Xunit;

namespace SledzSpecke.Tests.Unit.Core.Entities;

public class ProcedureRealizationTests
{
    [Fact]
    public void Create_WithValidData_ShouldReturnSuccessResult()
    {
        // Arrange
        var requirementId = new ProcedureRequirementId(1);
        var userId = new UserId(1);
        var date = DateTime.UtcNow.Date;
        var location = "Oddział Kardiologii";
        var role = ProcedureRole.Operator;
        int? year = null;

        // Act
        var result = ProcedureRealization.Create(
            requirementId,
            userId,
            date,
            location,
            role,
            year
        );

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().NotBeNull();
        result.Value.RequirementId.Should().Be(requirementId);
        result.Value.UserId.Should().Be(userId);
        result.Value.Date.Should().Be(date);
        result.Value.Location.Should().Be(location);
        result.Value.Role.Should().Be(role);
        result.Value.Year.Should().BeNull();
    }

    [Fact]
    public void Create_WithFutureDate_ShouldReturnFailure()
    {
        // Arrange
        var futureDate = DateTime.UtcNow.AddDays(1);

        // Act
        var result = ProcedureRealization.Create(
            new ProcedureRequirementId(1),
            new UserId(1),
            futureDate,
            "Valid Location",
            ProcedureRole.Operator,
            null
        );

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Error.Should().Contain("future");
    }

    [Theory]
    [InlineData("")]
    [InlineData(" ")]
    [InlineData(null)]
    public void Create_WithInvalidLocation_ShouldReturnFailure(string invalidLocation)
    {
        // Act
        var result = ProcedureRealization.Create(
            new ProcedureRequirementId(1),
            new UserId(1),
            DateTime.UtcNow.Date,
            invalidLocation,
            ProcedureRole.Operator,
            null
        );

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Error.Should().Contain("Location is required");
    }

    [Theory]
    [InlineData(0)]
    [InlineData(7)]
    [InlineData(-1)]
    public void Create_WithInvalidYear_ShouldReturnFailure(int invalidYear)
    {
        // Act
        var result = ProcedureRealization.Create(
            new ProcedureRequirementId(1),
            new UserId(1),
            DateTime.UtcNow.Date,
            "Valid Location",
            ProcedureRole.Operator,
            invalidYear
        );

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Error.Should().Contain("between 1 and 6");
    }

    [Fact]
    public void Update_WithValidData_ShouldReturnSuccess()
    {
        // Arrange
        var realization = ProcedureRealization.Create(
            new ProcedureRequirementId(1),
            new UserId(1),
            DateTime.UtcNow.Date,
            "Original Location",
            ProcedureRole.Operator,
            null
        ).Value;

        var newDate = DateTime.UtcNow.Date.AddDays(-1);
        var newLocation = "Updated Location";
        var newRole = ProcedureRole.Assistant;

        // Act
        var result = realization.Update(newDate, newLocation, newRole);

        // Assert
        result.IsSuccess.Should().BeTrue();
        realization.Date.Should().Be(newDate);
        realization.Location.Should().Be(newLocation);
        realization.Role.Should().Be(newRole);
    }

    [Fact]
    public void Update_WithFutureDate_ShouldReturnFailure()
    {
        // Arrange
        var realization = ProcedureRealization.Create(
            new ProcedureRequirementId(1),
            new UserId(1),
            DateTime.UtcNow.Date,
            "Original Location",
            ProcedureRole.Operator,
            null
        ).Value;

        // Act
        var result = realization.Update(
            DateTime.UtcNow.AddDays(1),
            "Valid Location",
            ProcedureRole.Operator
        );

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Should().Contain("future");
    }

    [Fact]
    public void Update_WithInvalidLocation_ShouldReturnFailure()
    {
        // Arrange
        var realization = ProcedureRealization.Create(
            new ProcedureRequirementId(1),
            new UserId(1),
            DateTime.UtcNow.Date,
            "Original Location",
            ProcedureRole.Operator,
            null
        ).Value;

        // Act
        var result = realization.Update(
            DateTime.UtcNow.Date,
            "",
            ProcedureRole.Operator
        );

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Should().Contain("Location is required");
    }

    [Fact]
    public void IsForUser_WithMatchingUserId_ShouldReturnTrue()
    {
        // Arrange
        var userId = new UserId(1);
        var realization = ProcedureRealization.Create(
            new ProcedureRequirementId(1),
            userId,
            DateTime.UtcNow.Date,
            "Location",
            ProcedureRole.Operator,
            null
        ).Value;

        // Act
        var result = realization.IsForUser(userId);

        // Assert
        result.Should().BeTrue();
    }

    [Fact]
    public void IsForUser_WithDifferentUserId_ShouldReturnFalse()
    {
        // Arrange
        var realization = ProcedureRealization.Create(
            new ProcedureRequirementId(1),
            new UserId(1),
            DateTime.UtcNow.Date,
            "Location",
            ProcedureRole.Operator,
            null
        ).Value;

        // Act
        var result = realization.IsForUser(new UserId(2));

        // Assert
        result.Should().BeFalse();
    }

    [Theory]
    [InlineData(ProcedureRole.Operator)]
    [InlineData(ProcedureRole.Assistant)]
    public void Create_WithValidRole_ShouldSetRole(ProcedureRole role)
    {
        // Act
        var result = ProcedureRealization.Create(
            new ProcedureRequirementId(1),
            new UserId(1),
            DateTime.UtcNow.Date,
            "Location",
            role,
            null
        );

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Role.Should().Be(role);
    }

    [Fact]
    public void Create_WithOldSmkYear_ShouldSetYear()
    {
        // Arrange
        var year = 3;

        // Act
        var result = ProcedureRealization.Create(
            new ProcedureRequirementId(1),
            new UserId(1),
            DateTime.UtcNow.Date,
            "Location",
            ProcedureRole.Operator,
            year
        );

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Year.Should().Be(year);
    }
}
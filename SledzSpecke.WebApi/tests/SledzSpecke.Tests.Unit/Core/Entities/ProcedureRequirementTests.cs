using FluentAssertions;
using SledzSpecke.Core.Entities;
using SledzSpecke.Core.ValueObjects;
using Xunit;

namespace SledzSpecke.Tests.Unit.Core.Entities;

public class ProcedureRequirementTests
{
    [Fact]
    public void Constructor_WithValidData_ShouldCreateProcedureRequirement()
    {
        // Arrange
        var moduleId = new ModuleId(1);
        var code = "PROC001";
        var name = "Test Procedure";
        var requiredAsOperator = 5;
        var requiredAsAssistant = 3;
        var smkVersion = SmkVersion.New;
        int? year = null;

        // Act
        var requirement = new ProcedureRequirement(
            moduleId,
            code,
            name,
            requiredAsOperator,
            requiredAsAssistant,
            smkVersion,
            year
        );

        // Assert
        requirement.Should().NotBeNull();
        requirement.ModuleId.Should().Be(moduleId);
        requirement.Code.Should().Be(code);
        requirement.Name.Should().Be(name);
        requirement.RequiredAsOperator.Should().Be(requiredAsOperator);
        requirement.RequiredAsAssistant.Should().Be(requiredAsAssistant);
        requirement.SmkVersion.Should().Be(smkVersion);
        requirement.Year.Should().BeNull();
    }

    [Fact]
    public void Constructor_WithOldSmkAndYear_ShouldSetYear()
    {
        // Arrange
        var moduleId = new ModuleId(1);
        var smkVersion = SmkVersion.Old;
        var year = 3;

        // Act
        var requirement = new ProcedureRequirement(
            moduleId,
            "OLD001",
            "Old SMK Procedure",
            1,
            0,
            smkVersion,
            year
        );

        // Assert
        requirement.Year.Should().Be(year);
        requirement.SmkVersion.Should().Be(smkVersion);
    }

    [Theory]
    [InlineData("")]
    [InlineData(" ")]
    [InlineData(null)]
    public void Constructor_WithInvalidCode_ShouldThrowException(string invalidCode)
    {
        // Arrange
        var moduleId = new ModuleId(1);

        // Act & Assert
        var act = () => new ProcedureRequirement(
            moduleId,
            invalidCode,
            "Valid Name",
            1,
            0,
            SmkVersion.New,
            null
        );

        act.Should().Throw<ArgumentException>();
    }

    [Theory]
    [InlineData("")]
    [InlineData(" ")]
    [InlineData(null)]
    public void Constructor_WithInvalidName_ShouldThrowException(string invalidName)
    {
        // Arrange
        var moduleId = new ModuleId(1);

        // Act & Assert
        var act = () => new ProcedureRequirement(
            moduleId,
            "PROC001",
            invalidName,
            1,
            0,
            SmkVersion.New,
            null
        );

        act.Should().Throw<ArgumentException>();
    }

    [Theory]
    [InlineData(-1)]
    [InlineData(-10)]
    public void Constructor_WithNegativeRequiredCounts_ShouldThrowException(int negativeCount)
    {
        // Arrange
        var moduleId = new ModuleId(1);

        // Act & Assert
        var act = () => new ProcedureRequirement(
            moduleId,
            "PROC001",
            "Valid Name",
            negativeCount,
            0,
            SmkVersion.New,
            null
        );

        act.Should().Throw<ArgumentOutOfRangeException>();
    }

    [Fact]
    public void Constructor_WithNullModuleId_ShouldThrowException()
    {
        // Act & Assert
        var act = () => new ProcedureRequirement(
            null!,
            "PROC001",
            "Valid Name",
            1,
            0,
            SmkVersion.New,
            null
        );

        act.Should().Throw<ArgumentNullException>();
    }

    [Fact]
    public void Update_WithValidData_ShouldUpdateProperties()
    {
        // Arrange
        var requirement = new ProcedureRequirement(
            new ModuleId(1),
            "PROC001",
            "Original Name",
            1,
            0,
            SmkVersion.New,
            null
        );

        var newName = "Updated Name";
        var newRequiredAsOperator = 3;
        var newRequiredAsAssistant = 2;

        // Act
        requirement.Update(newName, newRequiredAsOperator, newRequiredAsAssistant);

        // Assert
        requirement.Name.Should().Be(newName);
        requirement.RequiredAsOperator.Should().Be(newRequiredAsOperator);
        requirement.RequiredAsAssistant.Should().Be(newRequiredAsAssistant);
    }

    [Fact]
    public void Update_WithInvalidName_ShouldThrowException()
    {
        // Arrange
        var requirement = new ProcedureRequirement(
            new ModuleId(1),
            "PROC001",
            "Original Name",
            1,
            0,
            SmkVersion.New,
            null
        );

        // Act & Assert
        var act = () => requirement.Update("", 1, 0);
        act.Should().Throw<ArgumentException>();
    }

    [Fact]
    public void GetTotalRequired_ShouldReturnSumOfOperatorAndAssistant()
    {
        // Arrange
        var requirement = new ProcedureRequirement(
            new ModuleId(1),
            "PROC001",
            "Test Procedure",
            5,
            3,
            SmkVersion.New,
            null
        );

        // Act
        var total = requirement.GetTotalRequired();

        // Assert
        total.Should().Be(8);
    }

    [Theory]
    [InlineData(SmkVersion.Old, 2, true)]
    [InlineData(SmkVersion.Old, null, false)]
    [InlineData(SmkVersion.New, null, true)]
    [InlineData(SmkVersion.New, 2, false)]
    public void IsValidForSmkVersion_ShouldReturnCorrectResult(SmkVersion smkVersion, int? year, bool expectedResult)
    {
        // Arrange
        var requirement = new ProcedureRequirement(
            new ModuleId(1),
            "PROC001",
            "Test Procedure",
            1,
            0,
            smkVersion,
            year
        );

        // Act
        var result = requirement.IsValidForSmkVersion();

        // Assert
        result.Should().Be(expectedResult);
    }
}
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

public class ProcedureValidationServiceTests
{
    private readonly Mock<IProcedureRepository> _procedureRepositoryMock;
    private readonly Mock<ISpecializationRepository> _specializationRepositoryMock;
    private readonly Mock<ISmkPolicyFactory> _policyFactoryMock;
    private readonly ProcedureValidationService _service;

    public ProcedureValidationServiceTests()
    {
        _procedureRepositoryMock = new Mock<IProcedureRepository>();
        _specializationRepositoryMock = new Mock<ISpecializationRepository>();
        _policyFactoryMock = new Mock<ISmkPolicyFactory>();
        _service = new ProcedureValidationService(
            _procedureRepositoryMock.Object,
            _specializationRepositoryMock.Object,
            _policyFactoryMock.Object);
    }

    [Fact]
    public async Task ValidateProcedureAsync_WithValidOldSmkProcedure_ReturnsSuccess()
    {
        // Arrange
        var userId = new UserId(1);
        var specializationId = new SpecializationId(1);
        var internshipId = new InternshipId(1);
        var procedureId = new ProcedureId(1);
        
        var procedure = ProcedureOldSmk.Create(
            procedureId,
            null, // moduleId
            internshipId,
            DateTime.Today,
            1, // year
            "PROC001",
            "Test Procedure", // name
            "Department A", // location
            ProcedureExecutionType.CodeA,
            "Dr. Smith"); // supervisorName

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

        var mockPolicy = new Mock<ISmkPolicy<ProcedureBase>>();
        mockPolicy.Setup(p => p.Validate(It.IsAny<ProcedureBase>(), It.IsAny<SpecializationContext>()))
            .Returns(Result.Success());

        _policyFactoryMock.Setup(f => f.GetPolicy<ProcedureBase>(SmkVersion.Old))
            .Returns(mockPolicy.Object);

        // Act
        var result = await _service.ValidateProcedureAsync(procedure, userId, specialization, null);

        // Assert
        Assert.True(result.IsSuccess);
    }

    [Fact]
    public async Task ValidateProcedureAsync_WithInvalidCode_ReturnsFailure()
    {
        // Arrange
        var userId = new UserId(1);
        var specializationId = new SpecializationId(1);
        var internshipId = new InternshipId(1);
        var procedureId = new ProcedureId(1);
        
        var procedure = ProcedureOldSmk.Create(
            procedureId,
            null, // moduleId
            internshipId,
            DateTime.Today,
            1,
            "INVALID_CODE", // Invalid code
            "Test Procedure", // name
            "Department A", // location
            ProcedureExecutionType.CodeA,
            "Dr. Smith"); // supervisorName

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

        var mockPolicy = new Mock<ISmkPolicy<ProcedureBase>>();
        mockPolicy.Setup(p => p.Validate(It.IsAny<ProcedureBase>(), It.IsAny<SpecializationContext>()))
            .Returns(Result.Success());

        _policyFactoryMock.Setup(f => f.GetPolicy<ProcedureBase>(SmkVersion.Old))
            .Returns(mockPolicy.Object);

        // Act
        var result = await _service.ValidateProcedureAsync(procedure, userId, specialization, null);

        // Assert
        Assert.False(result.IsSuccess);
        Assert.Contains("nie jest zdefiniowany", result.Error);
    }

    [Fact]
    public async Task CalculateProcedureProgressAsync_ForOldSmk_CalculatesCorrectly()
    {
        // Arrange
        var requirement = new ProcedureRequirement
        {
            Id = 1,
            Code = "PROC001",
            Name = "Test Procedure",
            MinimumCountA = 5,
            MinimumCountB = 3,
            MinimumTotal = 8,
            Year = 1,
            SmkVersion = SmkVersion.Old
        };

        var procedures = new List<ProcedureBase>();
        
        // Add completed procedures with operator code A
        for (int i = 0; i < 3; i++)
        {
            var proc = ProcedureOldSmk.Create(
                new ProcedureId(i + 1),
                null, // moduleId
                new InternshipId(1),
                DateTime.Today.AddDays(-i),
                1,
                "PROC001",
                "Test Procedure", // name
                "Department A", // location
                ProcedureExecutionType.CodeA,
                "Dr. Smith"); // supervisorName
            proc.UpdateProcedureDetails(ProcedureExecutionType.CodeA, "Dr. Smith", null, "AB", 'M');
            proc.Complete();
            procedures.Add(proc);
        }
        
        // Add completed procedures with operator code B
        for (int i = 0; i < 2; i++)
        {
            var proc = ProcedureOldSmk.Create(
                new ProcedureId(i + 10),
                null, // moduleId
                new InternshipId(1),
                DateTime.Today.AddDays(-i),
                1,
                "PROC001",
                "Test Procedure", // name
                "Department B", // location
                ProcedureExecutionType.CodeB,
                "Dr. Smith"); // supervisorName
            proc.UpdateProcedureDetails(ProcedureExecutionType.CodeB, "Dr. Smith", null, "CD", 'K');
            proc.Complete();
            procedures.Add(proc);
        }

        // Act
        var result = await _service.CalculateProcedureProgressAsync(requirement, procedures);

        // Assert
        Assert.True(result.IsSuccess);
        var progress = result.Value;
        Assert.Equal(3, progress.CompletedCountA);
        Assert.Equal(2, progress.CompletedCountB);
        Assert.Equal(5, progress.TotalCompleted);
        Assert.Equal(62.5, progress.ProgressPercentage); // 5/8 * 100
        Assert.False(progress.IsRequirementMet);
        Assert.Contains("Brakuje 2 procedur jako operator", progress.ValidationMessages[0]);
    }

    [Fact]
    public async Task CalculateProcedureProgressAsync_ForNewSmk_AggregatesCounts()
    {
        // Arrange
        var moduleId = new ModuleId(1);
        var requirement = new ProcedureRequirement
        {
            Id = 3,
            Code = "PROC001",
            Name = "Test Procedure",
            MinimumCountA = 20,
            MinimumCountB = 10,
            MinimumTotal = 30,
            ModuleId = moduleId,
            SmkVersion = SmkVersion.New
        };

        var procedures = new List<ProcedureBase>();
        
        // Add New SMK procedure with counts
        var proc1 = ProcedureNewSmk.Create(
            new ProcedureId(1),
            moduleId,
            new InternshipId(1),
            DateTime.Today,
            "PROC001",
            "Test Procedure", // procedureName
            "Department A", // location
            ProcedureExecutionType.CodeA,
            "Dr. Smith", // supervisorName
            3); // procedureRequirementId
        proc1.UpdateCounts(15, 8);
        proc1.Complete();
        procedures.Add(proc1);

        // Act
        var result = await _service.CalculateProcedureProgressAsync(requirement, procedures);

        // Assert
        Assert.True(result.IsSuccess);
        var progress = result.Value;
        Assert.Equal(15, progress.CompletedCountA);
        Assert.Equal(8, progress.CompletedCountB);
        Assert.Equal(23, progress.TotalCompleted);
        Assert.Equal(76.67, Math.Round(progress.ProgressPercentage, 2)); // 23/30 * 100
        Assert.False(progress.IsRequirementMet);
    }

    [Fact]
    public async Task IsProcedureRequirementMetAsync_WhenMet_ReturnsTrue()
    {
        // Arrange
        var requirement = new ProcedureRequirement
        {
            Id = 1,
            Code = "PROC001",
            Name = "Test Procedure",
            MinimumCountA = 2,
            MinimumCountB = 1,
            MinimumTotal = 3,
            Year = 1,
            SmkVersion = SmkVersion.Old
        };

        var procedures = new List<ProcedureBase>();
        
        // Add enough procedures to meet requirement
        for (int i = 0; i < 2; i++)
        {
            var proc = ProcedureOldSmk.Create(
                new ProcedureId(i + 1),
                null, // moduleId
                new InternshipId(1),
                DateTime.Today.AddDays(-i),
                1,
                "PROC001",
                "Test Procedure", // name
                "Department A", // location
                ProcedureExecutionType.CodeA,
                "Dr. Smith"); // supervisorName
            proc.UpdateProcedureDetails(ProcedureExecutionType.CodeA, "Dr. Smith", null, "AB", 'M');
            proc.Complete();
            procedures.Add(proc);
        }
        
        var procB = ProcedureOldSmk.Create(
            new ProcedureId(10),
            null, // moduleId
            new InternshipId(1),
            DateTime.Today,
            1,
            "PROC001",
            "Test Procedure", // name
            "Department B", // location
            ProcedureExecutionType.CodeB,
            "Dr. Smith"); // supervisorName
        procB.UpdateProcedureDetails(ProcedureExecutionType.CodeB, "Dr. Smith", null, "CD", 'K');
        procB.Complete();
        procedures.Add(procB);

        // Act
        var result = await _service.IsProcedureRequirementMetAsync(requirement, procedures);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.True(result.Value);
    }

    [Fact]
    public async Task GetValidationSummaryAsync_ReturnsCorrectSummary()
    {
        // Arrange
        var userId = new UserId(1);
        var specializationId = new SpecializationId(1);
        var moduleId = new ModuleId(1);
        
        var specialization = new Specialization(
            specializationId,
            userId,
            "Test Specialization",
            "TST001",
            new SmkVersion("new"),
            "standard", // programVariant
            DateTime.Today.AddYears(-1),
            DateTime.Today.AddYears(4),
            2025, // plannedPesYear
            "Standard Program",
            5);

        _specializationRepositoryMock.Setup(r => r.GetByIdAsync(specializationId))
            .ReturnsAsync(specialization);

        var procedures = new List<ProcedureBase>();
        var proc = ProcedureNewSmk.Create(
            new ProcedureId(1),
            moduleId,
            new InternshipId(1),
            DateTime.Today,
            "PROC001",
            "Badanie fizykalne", // procedureName
            "Department A", // location
            ProcedureExecutionType.CodeA,
            "Dr. Smith", // supervisorName
            3); // procedureRequirementId
        proc.UpdateCounts(15, 5);
        proc.Complete();
        procedures.Add(proc);

        _procedureRepositoryMock.Setup(r => r.GetByUserAsync(userId))
            .ReturnsAsync(procedures);

        // Act
        var result = await _service.GetValidationSummaryAsync(userId, specializationId, moduleId);

        // Assert
        Assert.True(result.IsSuccess);
        var summary = result.Value;
        Assert.Equal(SmkVersion.New, summary.SmkVersion);
        Assert.Equal(2, summary.TotalRequirements); // Mock has 2 requirements for module 1
        Assert.Equal(0, summary.CompletedRequirements); // Not enough procedures
        Assert.Equal(1, summary.TotalProceduresCompleted);
        Assert.True(summary.ValidationWarnings.Any(w => w.Contains("poniżej 25%")));
    }

    [Fact]
    public async Task GetUnmetRequirementsAsync_ReturnsUnmetRequirements()
    {
        // Arrange
        var userId = new UserId(1);
        var specializationId = new SpecializationId(1);
        
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

        _specializationRepositoryMock.Setup(r => r.GetByIdAsync(specializationId))
            .ReturnsAsync(specialization);

        _procedureRepositoryMock.Setup(r => r.GetByUserAsync(userId))
            .ReturnsAsync(new List<ProcedureBase>()); // No procedures completed

        // Act
        var result = await _service.GetUnmetRequirementsAsync(userId, specializationId);

        // Assert
        Assert.True(result.IsSuccess);
        var unmetRequirements = result.Value.ToList();
        Assert.Equal(2, unmetRequirements.Count); // All Old SMK requirements are unmet
    }
}
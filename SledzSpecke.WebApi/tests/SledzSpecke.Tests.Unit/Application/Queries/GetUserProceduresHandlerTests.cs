using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;
using SledzSpecke.Application.DTO;
using SledzSpecke.Application.Queries;
using SledzSpecke.Application.Queries.Handlers;
using SledzSpecke.Core.Entities;
using SledzSpecke.Core.Repositories;
using SledzSpecke.Core.ValueObjects;
using Xunit;

namespace SledzSpecke.Tests.Unit.Application.Queries;

public class GetUserProceduresHandlerTests
{
    private readonly Mock<IProcedureRealizationRepository> _mockRealizationRepository;
    private readonly Mock<IProcedureRequirementRepository> _mockRequirementRepository;
    private readonly Mock<ISpecializationRepository> _mockSpecializationRepository;
    private readonly Mock<IModuleRepository> _mockModuleRepository;
    private readonly Mock<ILogger<GetUserProceduresHandler>> _mockLogger;
    private readonly GetUserProceduresHandler _handler;

    public GetUserProceduresHandlerTests()
    {
        _mockRealizationRepository = new Mock<IProcedureRealizationRepository>();
        _mockRequirementRepository = new Mock<IProcedureRequirementRepository>();
        _mockSpecializationRepository = new Mock<ISpecializationRepository>();
        _mockModuleRepository = new Mock<IModuleRepository>();
        _mockLogger = new Mock<ILogger<GetUserProceduresHandler>>();

        _handler = new GetUserProceduresHandler(
            _mockRealizationRepository.Object,
            _mockRequirementRepository.Object,
            _mockSpecializationRepository.Object,
            _mockModuleRepository.Object,
            _mockLogger.Object);
    }

    [Fact]
    public async Task HandleAsync_WithProcedures_ShouldReturnSummary()
    {
        // Arrange
        var userId = new UserId(1);
        var specializationId = new SpecializationId(1);
        var moduleId1 = new ModuleId(1);
        var moduleId2 = new ModuleId(2);
        
        var specialization = new Specialization(
            specializationId,
            userId,
            "Kardiologia",
            "cardiology",
            SmkVersion.New,
            "standard",
            DateTime.UtcNow,
            DateTime.UtcNow.AddYears(5),
            2024,
            "modular",
            5);

        var modules = new List<Module>
        {
            new Module(moduleId1, "Moduł Podstawowy", ModuleType.Basic, specializationId,
                1, 0, 0, 0, 0, 10, 20, DateTime.UtcNow, null, null, null, false, SmkVersion.New),
            new Module(moduleId2, "Moduł Specjalistyczny", ModuleType.Specialist, specializationId,
                1, 0, 0, 0, 0, 15, 30, DateTime.UtcNow, null, null, null, false, SmkVersion.New)
        };

        var requirements = new List<ProcedureRequirement>
        {
            new ProcedureRequirement(moduleId1, "PROC001", "Procedure 1", 2, 1, SmkVersion.New, null),
            new ProcedureRequirement(moduleId1, "PROC002", "Procedure 2", 1, 0, SmkVersion.New, null),
            new ProcedureRequirement(moduleId2, "PROC003", "Procedure 3", 3, 2, SmkVersion.New, null)
        };

        var realizations = new List<ProcedureRealization>
        {
            ProcedureRealization.Create(new ProcedureRequirementId(1), userId,
                DateTime.UtcNow.Date, "Location 1", ProcedureRole.Operator, null).Value,
            ProcedureRealization.Create(new ProcedureRequirementId(1), userId,
                DateTime.UtcNow.Date.AddDays(-1), "Location 2", ProcedureRole.Assistant, null).Value,
            ProcedureRealization.Create(new ProcedureRequirementId(2), userId,
                DateTime.UtcNow.Date.AddDays(-2), "Location 3", ProcedureRole.Operator, null).Value
        };

        var query = new GetUserProcedures(userId.Value);

        _mockSpecializationRepository.Setup(x => x.GetByUserIdAsync(userId))
            .ReturnsAsync(new List<Specialization> { specialization });
        _mockModuleRepository.Setup(x => x.GetBySpecializationIdAsync(specializationId))
            .ReturnsAsync(modules);
        _mockRequirementRepository.Setup(x => x.GetAllAsync())
            .ReturnsAsync(requirements);
        _mockRealizationRepository.Setup(x => x.GetByUserIdAsync(userId))
            .ReturnsAsync(realizations);

        // Act
        var result = await _handler.HandleAsync(query);

        // Assert
        result.Should().NotBeNull();
        result.TotalRealizations.Should().Be(3);
        result.ModuleSummaries.Should().HaveCount(2);
        
        var basicModule = result.ModuleSummaries.First(m => m.ModuleName == "Moduł Podstawowy");
        basicModule.TotalRequired.Should().Be(3); // 2+1 for PROC001, 1+0 for PROC002
        basicModule.Completed.Should().Be(3); // 1 operator + 1 assistant for PROC001, 1 operator for PROC002
        
        var specialistModule = result.ModuleSummaries.First(m => m.ModuleName == "Moduł Specjalistyczny");
        specialistModule.TotalRequired.Should().Be(5); // 3+2 for PROC003
        specialistModule.Completed.Should().Be(0); // No realizations for PROC003
    }

    [Fact]
    public async Task HandleAsync_WithNoSpecialization_ShouldReturnEmptyResult()
    {
        // Arrange
        var userId = new UserId(1);
        var query = new GetUserProcedures(userId.Value);

        _mockSpecializationRepository.Setup(x => x.GetByUserIdAsync(userId))
            .ReturnsAsync(new List<Specialization>());

        // Act
        var result = await _handler.HandleAsync(query);

        // Assert
        result.Should().NotBeNull();
        result.TotalRealizations.Should().Be(0);
        result.ModuleSummaries.Should().BeEmpty();
        result.RecentRealizations.Should().BeEmpty();
    }

    [Fact]
    public async Task HandleAsync_ShouldReturnRecentRealizationsInDescendingOrder()
    {
        // Arrange
        var userId = new UserId(1);
        var specializationId = new SpecializationId(1);
        var moduleId = new ModuleId(1);
        
        var specialization = new Specialization(
            specializationId,
            userId,
            "Test",
            "test",
            SmkVersion.New,
            "standard",
            DateTime.UtcNow,
            DateTime.UtcNow.AddYears(5),
            2024,
            "modular",
            5);

        var module = new Module(moduleId, "Test Module", ModuleType.Basic, specializationId,
            1, 0, 0, 0, 0, 10, 20, DateTime.UtcNow, null, null, null, false, SmkVersion.New);

        var requirements = new List<ProcedureRequirement>
        {
            new ProcedureRequirement(moduleId, "PROC001", "Procedure 1", 1, 0, SmkVersion.New, null),
            new ProcedureRequirement(moduleId, "PROC002", "Procedure 2", 1, 0, SmkVersion.New, null)
        };

        var realizations = new List<ProcedureRealization>
        {
            ProcedureRealization.Create(new ProcedureRequirementId(1), userId,
                DateTime.UtcNow.Date.AddDays(-5), "Old Location", ProcedureRole.Operator, null).Value,
            ProcedureRealization.Create(new ProcedureRequirementId(2), userId,
                DateTime.UtcNow.Date, "Recent Location", ProcedureRole.Operator, null).Value,
            ProcedureRealization.Create(new ProcedureRequirementId(1), userId,
                DateTime.UtcNow.Date.AddDays(-2), "Middle Location", ProcedureRole.Assistant, null).Value
        };

        var query = new GetUserProcedures(userId.Value);

        _mockSpecializationRepository.Setup(x => x.GetByUserIdAsync(userId))
            .ReturnsAsync(new List<Specialization> { specialization });
        _mockModuleRepository.Setup(x => x.GetBySpecializationIdAsync(specializationId))
            .ReturnsAsync(new List<Module> { module });
        _mockRequirementRepository.Setup(x => x.GetAllAsync())
            .ReturnsAsync(requirements);
        _mockRealizationRepository.Setup(x => x.GetByUserIdAsync(userId))
            .ReturnsAsync(realizations);

        // Act
        var result = await _handler.HandleAsync(query);

        // Assert
        result.RecentRealizations.Should().HaveCount(3);
        result.RecentRealizations[0].Date.Should().Be(DateTime.UtcNow.Date);
        result.RecentRealizations[1].Date.Should().Be(DateTime.UtcNow.Date.AddDays(-2));
        result.RecentRealizations[2].Date.Should().Be(DateTime.UtcNow.Date.AddDays(-5));
    }

    [Fact]
    public async Task HandleAsync_WithMixedSmkVersions_ShouldHandleCorrectly()
    {
        // Arrange
        var userId = new UserId(1);
        var specializationId = new SpecializationId(1);
        var moduleId = new ModuleId(1);
        
        var specialization = new Specialization(
            specializationId,
            userId,
            "Mixed SMK",
            "mixed",
            SmkVersion.Old,
            "standard",
            DateTime.UtcNow,
            DateTime.UtcNow.AddYears(5),
            2024,
            "modular",
            5);

        var module = new Module(moduleId, "Old Module", ModuleType.Basic, specializationId,
            1, 0, 0, 0, 0, 10, 20, DateTime.UtcNow, null, null, null, false, SmkVersion.Old);

        var requirements = new List<ProcedureRequirement>
        {
            new ProcedureRequirement(moduleId, "OLD001", "Old Procedure", 1, 0, SmkVersion.Old, 3)
        };

        var realizations = new List<ProcedureRealization>
        {
            ProcedureRealization.Create(new ProcedureRequirementId(1), userId,
                DateTime.UtcNow.Date, "Hospital", ProcedureRole.Operator, 3).Value
        };

        var query = new GetUserProcedures(userId.Value);

        _mockSpecializationRepository.Setup(x => x.GetByUserIdAsync(userId))
            .ReturnsAsync(new List<Specialization> { specialization });
        _mockModuleRepository.Setup(x => x.GetBySpecializationIdAsync(specializationId))
            .ReturnsAsync(new List<Module> { module });
        _mockRequirementRepository.Setup(x => x.GetAllAsync())
            .ReturnsAsync(requirements);
        _mockRealizationRepository.Setup(x => x.GetByUserIdAsync(userId))
            .ReturnsAsync(realizations);

        // Act
        var result = await _handler.HandleAsync(query);

        // Assert
        result.ModuleSummaries.Should().HaveCount(1);
        result.ModuleSummaries[0].SmkVersion.Should().Be("old");
        result.RecentRealizations.Should().HaveCount(1);
        result.RecentRealizations[0].Year.Should().Be(3);
    }
}
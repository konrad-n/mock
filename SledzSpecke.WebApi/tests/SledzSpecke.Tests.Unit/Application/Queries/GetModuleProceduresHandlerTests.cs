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

public class GetModuleProceduresHandlerTests
{
    private readonly Mock<IProcedureRequirementRepository> _mockRequirementRepository;
    private readonly Mock<IProcedureRealizationRepository> _mockRealizationRepository;
    private readonly Mock<IModuleRepository> _mockModuleRepository;
    private readonly Mock<ILogger<GetModuleProceduresHandler>> _mockLogger;
    private readonly GetModuleProceduresHandler _handler;

    public GetModuleProceduresHandlerTests()
    {
        _mockRequirementRepository = new Mock<IProcedureRequirementRepository>();
        _mockRealizationRepository = new Mock<IProcedureRealizationRepository>();
        _mockModuleRepository = new Mock<IModuleRepository>();
        _mockLogger = new Mock<ILogger<GetModuleProceduresHandler>>();

        _handler = new GetModuleProceduresHandler(
            _mockRequirementRepository.Object,
            _mockRealizationRepository.Object,
            _mockModuleRepository.Object,
            _mockLogger.Object);
    }

    [Fact]
    public async Task HandleAsync_WithValidModuleId_ShouldReturnProcedures()
    {
        // Arrange
        var moduleId = new ModuleId(1);
        var userId = new UserId(1);
        var specializationId = new SpecializationId(1);
        
        var module = new Module(
            moduleId,
            "Test Module",
            ModuleType.Basic,
            specializationId,
            1, 0, 0, 0, 0, 10, 20,
            DateTime.UtcNow, null, null, null, false,
            SmkVersion.New);

        var requirements = new List<ProcedureRequirement>
        {
            new ProcedureRequirement(moduleId, "PROC001", "Procedure 1", 2, 1, SmkVersion.New, null),
            new ProcedureRequirement(moduleId, "PROC002", "Procedure 2", 1, 0, SmkVersion.New, null)
        };

        var realizations = new List<ProcedureRealization>
        {
            ProcedureRealization.Create(
                new ProcedureRequirementId(1),
                userId,
                DateTime.UtcNow.Date,
                "Location 1",
                ProcedureRole.Operator,
                null).Value,
            ProcedureRealization.Create(
                new ProcedureRequirementId(1),
                userId,
                DateTime.UtcNow.Date.AddDays(-1),
                "Location 2",
                ProcedureRole.Assistant,
                null).Value
        };

        var query = new GetModuleProcedures(moduleId.Value, userId.Value);

        _mockModuleRepository.Setup(x => x.GetByIdAsync(moduleId))
            .ReturnsAsync(module);
        _mockRequirementRepository.Setup(x => x.GetByModuleIdAsync(moduleId))
            .ReturnsAsync(requirements);
        _mockRealizationRepository.Setup(x => x.GetByUserIdAsync(userId))
            .ReturnsAsync(realizations);

        // Act
        var result = await _handler.HandleAsync(query);

        // Assert
        result.Should().NotBeNull();
        result.ModuleId.Should().Be(moduleId.Value);
        result.ModuleName.Should().Be("Test Module");
        result.Procedures.Should().HaveCount(2);
        result.Procedures[0].Code.Should().Be("PROC001");
        result.Procedures[0].ExecutedAsOperator.Should().Be(1);
        result.Procedures[0].ExecutedAsAssistant.Should().Be(1);
    }

    [Fact]
    public async Task HandleAsync_WithNonExistentModule_ShouldThrowException()
    {
        // Arrange
        var query = new GetModuleProcedures(999, 1);

        _mockModuleRepository.Setup(x => x.GetByIdAsync(It.IsAny<ModuleId>()))
            .ReturnsAsync((Module)null);

        // Act & Assert
        await Assert.ThrowsAsync<InvalidOperationException>(() => _handler.HandleAsync(query));
    }

    [Fact]
    public async Task HandleAsync_WithNoProcedures_ShouldReturnEmptyList()
    {
        // Arrange
        var moduleId = new ModuleId(1);
        var userId = new UserId(1);
        var specializationId = new SpecializationId(1);
        
        var module = new Module(
            moduleId,
            "Empty Module",
            ModuleType.Basic,
            specializationId,
            1, 0, 0, 0, 0, 0, 0,
            DateTime.UtcNow, null, null, null, false,
            SmkVersion.New);

        var query = new GetModuleProcedures(moduleId.Value, userId.Value);

        _mockModuleRepository.Setup(x => x.GetByIdAsync(moduleId))
            .ReturnsAsync(module);
        _mockRequirementRepository.Setup(x => x.GetByModuleIdAsync(moduleId))
            .ReturnsAsync(new List<ProcedureRequirement>());
        _mockRealizationRepository.Setup(x => x.GetByUserIdAsync(It.IsAny<UserId>()))
            .ReturnsAsync(new List<ProcedureRealization>());

        // Act
        var result = await _handler.HandleAsync(query);

        // Assert
        result.Should().NotBeNull();
        result.ModuleId.Should().Be(moduleId.Value);
        result.Procedures.Should().BeEmpty();
    }

    [Fact]
    public async Task HandleAsync_WithOldSmkProcedures_ShouldIncludeYear()
    {
        // Arrange
        var moduleId = new ModuleId(1);
        var userId = new UserId(1);
        var specializationId = new SpecializationId(1);
        
        var module = new Module(
            moduleId,
            "Old SMK Module",
            ModuleType.Basic,
            specializationId,
            1, 0, 0, 0, 0, 5, 10,
            DateTime.UtcNow, null, null, null, false,
            SmkVersion.Old);

        var requirements = new List<ProcedureRequirement>
        {
            new ProcedureRequirement(moduleId, "OLD001", "Old Procedure", 1, 0, SmkVersion.Old, 3)
        };

        var realizations = new List<ProcedureRealization>
        {
            ProcedureRealization.Create(
                new ProcedureRequirementId(1),
                userId,
                DateTime.UtcNow.Date,
                "Hospital",
                ProcedureRole.Operator,
                3).Value
        };

        var query = new GetModuleProcedures(moduleId.Value, userId.Value);

        _mockModuleRepository.Setup(x => x.GetByIdAsync(moduleId))
            .ReturnsAsync(module);
        _mockRequirementRepository.Setup(x => x.GetByModuleIdAsync(moduleId))
            .ReturnsAsync(requirements);
        _mockRealizationRepository.Setup(x => x.GetByUserIdAsync(userId))
            .ReturnsAsync(realizations);

        // Act
        var result = await _handler.HandleAsync(query);

        // Assert
        result.Procedures.Should().HaveCount(1);
        result.Procedures[0].Year.Should().Be(3);
        result.SmkVersion.Should().Be("old");
    }
}
using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;
using SledzSpecke.Application.Commands;
using SledzSpecke.Application.Commands.Handlers;
using SledzSpecke.Core.Entities;
using SledzSpecke.Core.Repositories;
using SledzSpecke.Core.ValueObjects;
using SledzSpecke.Shared.Abstractions.Commands;
using Xunit;

namespace SledzSpecke.Tests.Unit.Application.Commands;

public class AddProcedureRealizationHandlerTests
{
    private readonly Mock<IProcedureRealizationRepository> _mockRealizationRepository;
    private readonly Mock<IProcedureRequirementRepository> _mockRequirementRepository;
    private readonly Mock<IUserRepository> _mockUserRepository;
    private readonly Mock<IUnitOfWork> _mockUnitOfWork;
    private readonly Mock<ILogger<AddProcedureRealizationHandler>> _mockLogger;
    private readonly AddProcedureRealizationHandler _handler;

    public AddProcedureRealizationHandlerTests()
    {
        _mockRealizationRepository = new Mock<IProcedureRealizationRepository>();
        _mockRequirementRepository = new Mock<IProcedureRequirementRepository>();
        _mockUserRepository = new Mock<IUserRepository>();
        _mockUnitOfWork = new Mock<IUnitOfWork>();
        _mockLogger = new Mock<ILogger<AddProcedureRealizationHandler>>();

        _handler = new AddProcedureRealizationHandler(
            _mockRealizationRepository.Object,
            _mockRequirementRepository.Object,
            _mockUserRepository.Object,
            _mockUnitOfWork.Object,
            _mockLogger.Object);
    }

    [Fact]
    public async Task HandleAsync_WithValidData_ShouldCreateRealization()
    {
        // Arrange
        var userId = new UserId(1);
        var requirementId = new ProcedureRequirementId(1);
        var moduleId = new ModuleId(1);
        
        var command = new AddProcedureRealization(
            requirementId.Value,
            userId.Value,
            DateTime.UtcNow.Date,
            "Oddział Kardiologii",
            ProcedureRole.Operator,
            null);

        var user = new User(userId, "test@example.com", "Test User", "Test123!");
        var requirement = new ProcedureRequirement(
            moduleId,
            "PROC001",
            "Test Procedure",
            1,
            0,
            SmkVersion.New,
            null);

        _mockUserRepository.Setup(x => x.GetByIdAsync(userId))
            .ReturnsAsync(user);
        _mockRequirementRepository.Setup(x => x.GetByIdAsync(requirementId))
            .ReturnsAsync(requirement);

        // Act
        await _handler.HandleAsync(command);

        // Assert
        _mockRealizationRepository.Verify(x => x.AddAsync(It.IsAny<ProcedureRealization>()), Times.Once);
        _mockUnitOfWork.Verify(x => x.SaveChangesAsync(), Times.Once);
    }

    [Fact]
    public async Task HandleAsync_WithNonExistentUser_ShouldThrowException()
    {
        // Arrange
        var command = new AddProcedureRealization(
            1,
            999,
            DateTime.UtcNow.Date,
            "Test Location",
            ProcedureRole.Operator,
            null);

        _mockUserRepository.Setup(x => x.GetByIdAsync(It.IsAny<UserId>()))
            .ReturnsAsync((User)null);

        // Act & Assert
        await Assert.ThrowsAsync<InvalidOperationException>(() => _handler.HandleAsync(command));
    }

    [Fact]
    public async Task HandleAsync_WithNonExistentRequirement_ShouldThrowException()
    {
        // Arrange
        var userId = new UserId(1);
        var command = new AddProcedureRealization(
            999,
            userId.Value,
            DateTime.UtcNow.Date,
            "Test Location",
            ProcedureRole.Operator,
            null);

        var user = new User(userId, "test@example.com", "Test User", "Test123!");
        _mockUserRepository.Setup(x => x.GetByIdAsync(userId))
            .ReturnsAsync(user);
        _mockRequirementRepository.Setup(x => x.GetByIdAsync(It.IsAny<ProcedureRequirementId>()))
            .ReturnsAsync((ProcedureRequirement)null);

        // Act & Assert
        await Assert.ThrowsAsync<InvalidOperationException>(() => _handler.HandleAsync(command));
    }

    [Fact]
    public async Task HandleAsync_WithFutureDate_ShouldThrowException()
    {
        // Arrange
        var userId = new UserId(1);
        var requirementId = new ProcedureRequirementId(1);
        var moduleId = new ModuleId(1);
        
        var command = new AddProcedureRealization(
            requirementId.Value,
            userId.Value,
            DateTime.UtcNow.AddDays(1),
            "Test Location",
            ProcedureRole.Operator,
            null);

        var user = new User(userId, "test@example.com", "Test User", "Test123!");
        var requirement = new ProcedureRequirement(
            moduleId,
            "PROC001",
            "Test Procedure",
            1,
            0,
            SmkVersion.New,
            null);

        _mockUserRepository.Setup(x => x.GetByIdAsync(userId))
            .ReturnsAsync(user);
        _mockRequirementRepository.Setup(x => x.GetByIdAsync(requirementId))
            .ReturnsAsync(requirement);

        // Act & Assert
        await Assert.ThrowsAsync<InvalidOperationException>(() => _handler.HandleAsync(command));
    }

    [Fact]
    public async Task HandleAsync_WithOldSmkAndValidYear_ShouldCreateRealization()
    {
        // Arrange
        var userId = new UserId(1);
        var requirementId = new ProcedureRequirementId(1);
        var moduleId = new ModuleId(1);
        
        var command = new AddProcedureRealization(
            requirementId.Value,
            userId.Value,
            DateTime.UtcNow.Date,
            "Test Location",
            ProcedureRole.Operator,
            3);

        var user = new User(userId, "test@example.com", "Test User", "Test123!");
        var requirement = new ProcedureRequirement(
            moduleId,
            "OLD001",
            "Old SMK Procedure",
            1,
            0,
            SmkVersion.Old,
            3);

        _mockUserRepository.Setup(x => x.GetByIdAsync(userId))
            .ReturnsAsync(user);
        _mockRequirementRepository.Setup(x => x.GetByIdAsync(requirementId))
            .ReturnsAsync(requirement);

        // Act
        await _handler.HandleAsync(command);

        // Assert
        _mockRealizationRepository.Verify(x => x.AddAsync(It.Is<ProcedureRealization>(pr => 
            pr.Year == 3)), Times.Once);
        _mockUnitOfWork.Verify(x => x.SaveChangesAsync(), Times.Once);
    }
}
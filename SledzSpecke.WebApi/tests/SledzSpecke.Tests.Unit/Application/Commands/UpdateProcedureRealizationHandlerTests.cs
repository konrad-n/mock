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

public class UpdateProcedureRealizationHandlerTests
{
    private readonly Mock<IProcedureRealizationRepository> _mockRepository;
    private readonly Mock<IUnitOfWork> _mockUnitOfWork;
    private readonly Mock<ILogger<UpdateProcedureRealizationHandler>> _mockLogger;
    private readonly UpdateProcedureRealizationHandler _handler;

    public UpdateProcedureRealizationHandlerTests()
    {
        _mockRepository = new Mock<IProcedureRealizationRepository>();
        _mockUnitOfWork = new Mock<IUnitOfWork>();
        _mockLogger = new Mock<ILogger<UpdateProcedureRealizationHandler>>();

        _handler = new UpdateProcedureRealizationHandler(
            _mockRepository.Object,
            _mockUnitOfWork.Object,
            _mockLogger.Object);
    }

    [Fact]
    public async Task HandleAsync_WithValidData_ShouldUpdateRealization()
    {
        // Arrange
        var realizationId = new ProcedureRealizationId(1);
        var userId = new UserId(1);
        var requirementId = new ProcedureRequirementId(1);
        
        var existingRealization = ProcedureRealization.Create(
            requirementId,
            userId,
            DateTime.UtcNow.Date.AddDays(-5),
            "Original Location",
            ProcedureRole.Operator,
            null).Value;

        var command = new UpdateProcedureRealization(
            realizationId.Value,
            DateTime.UtcNow.Date.AddDays(-1),
            "Updated Location",
            ProcedureRole.Assistant);

        _mockRepository.Setup(x => x.GetByIdAsync(realizationId))
            .ReturnsAsync(existingRealization);

        // Act
        await _handler.HandleAsync(command);

        // Assert
        existingRealization.Location.Should().Be("Updated Location");
        existingRealization.Role.Should().Be(ProcedureRole.Assistant);
        existingRealization.Date.Should().Be(command.Date);
        _mockUnitOfWork.Verify(x => x.SaveChangesAsync(), Times.Once);
    }

    [Fact]
    public async Task HandleAsync_WithNonExistentRealization_ShouldThrowException()
    {
        // Arrange
        var command = new UpdateProcedureRealization(
            999,
            DateTime.UtcNow.Date,
            "Test Location",
            ProcedureRole.Operator);

        _mockRepository.Setup(x => x.GetByIdAsync(It.IsAny<ProcedureRealizationId>()))
            .ReturnsAsync((ProcedureRealization)null);

        // Act & Assert
        await Assert.ThrowsAsync<InvalidOperationException>(() => _handler.HandleAsync(command));
    }

    [Fact]
    public async Task HandleAsync_WithFutureDate_ShouldThrowException()
    {
        // Arrange
        var realizationId = new ProcedureRealizationId(1);
        var userId = new UserId(1);
        var requirementId = new ProcedureRequirementId(1);
        
        var existingRealization = ProcedureRealization.Create(
            requirementId,
            userId,
            DateTime.UtcNow.Date.AddDays(-5),
            "Original Location",
            ProcedureRole.Operator,
            null).Value;

        var command = new UpdateProcedureRealization(
            realizationId.Value,
            DateTime.UtcNow.AddDays(1),
            "Updated Location",
            ProcedureRole.Assistant);

        _mockRepository.Setup(x => x.GetByIdAsync(realizationId))
            .ReturnsAsync(existingRealization);

        // Act & Assert
        await Assert.ThrowsAsync<InvalidOperationException>(() => _handler.HandleAsync(command));
    }

    [Fact]
    public async Task HandleAsync_WithEmptyLocation_ShouldThrowException()
    {
        // Arrange
        var realizationId = new ProcedureRealizationId(1);
        var userId = new UserId(1);
        var requirementId = new ProcedureRequirementId(1);
        
        var existingRealization = ProcedureRealization.Create(
            requirementId,
            userId,
            DateTime.UtcNow.Date.AddDays(-5),
            "Original Location",
            ProcedureRole.Operator,
            null).Value;

        var command = new UpdateProcedureRealization(
            realizationId.Value,
            DateTime.UtcNow.Date,
            "",
            ProcedureRole.Assistant);

        _mockRepository.Setup(x => x.GetByIdAsync(realizationId))
            .ReturnsAsync(existingRealization);

        // Act & Assert
        await Assert.ThrowsAsync<InvalidOperationException>(() => _handler.HandleAsync(command));
    }

    [Fact]
    public async Task HandleAsync_WithDifferentRole_ShouldUpdateRole()
    {
        // Arrange
        var realizationId = new ProcedureRealizationId(1);
        var userId = new UserId(1);
        var requirementId = new ProcedureRequirementId(1);
        
        var existingRealization = ProcedureRealization.Create(
            requirementId,
            userId,
            DateTime.UtcNow.Date.AddDays(-5),
            "Test Location",
            ProcedureRole.Operator,
            null).Value;

        var command = new UpdateProcedureRealization(
            realizationId.Value,
            existingRealization.Date,
            existingRealization.Location,
            ProcedureRole.Assistant);

        _mockRepository.Setup(x => x.GetByIdAsync(realizationId))
            .ReturnsAsync(existingRealization);

        // Act
        await _handler.HandleAsync(command);

        // Assert
        existingRealization.Role.Should().Be(ProcedureRole.Assistant);
        _mockUnitOfWork.Verify(x => x.SaveChangesAsync(), Times.Once);
    }
}
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

public class DeleteProcedureRealizationHandlerTests
{
    private readonly Mock<IProcedureRealizationRepository> _mockRepository;
    private readonly Mock<IUnitOfWork> _mockUnitOfWork;
    private readonly Mock<ILogger<DeleteProcedureRealizationHandler>> _mockLogger;
    private readonly DeleteProcedureRealizationHandler _handler;

    public DeleteProcedureRealizationHandlerTests()
    {
        _mockRepository = new Mock<IProcedureRealizationRepository>();
        _mockUnitOfWork = new Mock<IUnitOfWork>();
        _mockLogger = new Mock<ILogger<DeleteProcedureRealizationHandler>>();

        _handler = new DeleteProcedureRealizationHandler(
            _mockRepository.Object,
            _mockUnitOfWork.Object,
            _mockLogger.Object);
    }

    [Fact]
    public async Task HandleAsync_WithExistingRealization_ShouldDeleteSuccessfully()
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

        var command = new DeleteProcedureRealization(realizationId.Value, userId.Value);

        _mockRepository.Setup(x => x.GetByIdAsync(realizationId))
            .ReturnsAsync(existingRealization);

        // Act
        await _handler.HandleAsync(command);

        // Assert
        _mockRepository.Verify(x => x.DeleteAsync(existingRealization), Times.Once);
        _mockUnitOfWork.Verify(x => x.SaveChangesAsync(), Times.Once);
    }

    [Fact]
    public async Task HandleAsync_WithNonExistentRealization_ShouldThrowException()
    {
        // Arrange
        var command = new DeleteProcedureRealization(999, 1);

        _mockRepository.Setup(x => x.GetByIdAsync(It.IsAny<ProcedureRealizationId>()))
            .ReturnsAsync((ProcedureRealization)null);

        // Act & Assert
        await Assert.ThrowsAsync<InvalidOperationException>(() => _handler.HandleAsync(command));
        _mockRepository.Verify(x => x.DeleteAsync(It.IsAny<ProcedureRealization>()), Times.Never);
        _mockUnitOfWork.Verify(x => x.SaveChangesAsync(), Times.Never);
    }

    [Fact]
    public async Task HandleAsync_WithDifferentUserId_ShouldThrowException()
    {
        // Arrange
        var realizationId = new ProcedureRealizationId(1);
        var ownerId = new UserId(1);
        var otherUserId = new UserId(2);
        var requirementId = new ProcedureRequirementId(1);
        
        var existingRealization = ProcedureRealization.Create(
            requirementId,
            ownerId,
            DateTime.UtcNow.Date.AddDays(-5),
            "Test Location",
            ProcedureRole.Operator,
            null).Value;

        var command = new DeleteProcedureRealization(realizationId.Value, otherUserId.Value);

        _mockRepository.Setup(x => x.GetByIdAsync(realizationId))
            .ReturnsAsync(existingRealization);

        // Act & Assert
        await Assert.ThrowsAsync<UnauthorizedAccessException>(() => _handler.HandleAsync(command));
        _mockRepository.Verify(x => x.DeleteAsync(It.IsAny<ProcedureRealization>()), Times.Never);
        _mockUnitOfWork.Verify(x => x.SaveChangesAsync(), Times.Never);
    }

    [Fact]
    public async Task HandleAsync_ShouldLogDeletion()
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

        var command = new DeleteProcedureRealization(realizationId.Value, userId.Value);

        _mockRepository.Setup(x => x.GetByIdAsync(realizationId))
            .ReturnsAsync(existingRealization);

        // Act
        await _handler.HandleAsync(command);

        // Assert
        _mockLogger.Verify(
            x => x.Log(
                LogLevel.Information,
                It.IsAny<EventId>(),
                It.IsAny<It.IsAnyType>(),
                It.IsAny<Exception>(),
                It.IsAny<Func<It.IsAnyType, Exception, string>>()),
            Times.AtLeastOnce);
    }

    [Fact]
    public async Task HandleAsync_WithDatabaseError_ShouldPropagateException()
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

        var command = new DeleteProcedureRealization(realizationId.Value, userId.Value);

        _mockRepository.Setup(x => x.GetByIdAsync(realizationId))
            .ReturnsAsync(existingRealization);
        _mockUnitOfWork.Setup(x => x.SaveChangesAsync())
            .ThrowsAsync(new Exception("Database error"));

        // Act & Assert
        await Assert.ThrowsAsync<Exception>(() => _handler.HandleAsync(command));
    }
}
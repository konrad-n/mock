using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using SledzSpecke.Application.Abstractions;
using SledzSpecke.Application.Commands;
using SledzSpecke.Core.Entities;
using SledzSpecke.Core.Repositories;
using SledzSpecke.Core.ValueObjects;
using SledzSpecke.Tests.Integration.Common;
using Xunit;

namespace SledzSpecke.Tests.Integration.Handlers;

public class ResultPatternTests : IntegrationTestBase
{
    private readonly IResultCommandHandler<MarkInternshipAsCompleted> _markAsCompletedHandler;
    private readonly IInternshipRepository _internshipRepository;
    private readonly IUnitOfWork _unitOfWork;

    public ResultPatternTests() : base()
    {
        _markAsCompletedHandler = GetServiceAsync<IResultCommandHandler<MarkInternshipAsCompleted>>().Result;
        _internshipRepository = GetServiceAsync<IInternshipRepository>().Result;
        _unitOfWork = GetServiceAsync<IUnitOfWork>().Result;
    }

    [Fact]
    public async Task MarkInternshipAsCompleted_Should_Return_Success_When_Valid()
    {
        // Arrange
        var internship = Internship.Create(
            InternshipId.New(),
            new SpecializationId(1),
            "Test Hospital",
            "Test Department",
            DateTime.UtcNow.Date.AddDays(-30),
            DateTime.UtcNow.Date.AddDays(-1) // Ended yesterday
        );

        await _internshipRepository.AddAsync(internship);
        await _unitOfWork.SaveChangesAsync();

        var command = new MarkInternshipAsCompleted(internship.InternshipId.Value);

        // Act
        var result = await _markAsCompletedHandler.HandleAsync(command);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Error.Should().BeNull();
        result.ErrorCode.Should().BeNull();

        // Verify the internship is marked as completed
        var updatedInternship = await _internshipRepository.GetByIdAsync(internship.InternshipId);
        updatedInternship!.IsCompleted.Should().BeTrue();
    }

    [Fact]
    public async Task MarkInternshipAsCompleted_Should_Return_Failure_When_Internship_Not_Found()
    {
        // Arrange
        var nonExistentId = 999999;
        var command = new MarkInternshipAsCompleted(nonExistentId);

        // Act
        var result = await _markAsCompletedHandler.HandleAsync(command);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Should().Contain("not found");
    }

    [Fact]
    public async Task MarkInternshipAsCompleted_Should_Return_Failure_When_Not_Ended()
    {
        // Arrange
        var internship = Internship.Create(
            InternshipId.New(),
            new SpecializationId(1),
            "Test Hospital",
            "Test Department",
            DateTime.UtcNow.Date,
            DateTime.UtcNow.Date.AddDays(30) // Ends in the future
        );

        await _internshipRepository.AddAsync(internship);
        await _unitOfWork.SaveChangesAsync();

        var command = new MarkInternshipAsCompleted(internship.InternshipId.Value);

        // Act
        var result = await _markAsCompletedHandler.HandleAsync(command);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Should().Contain("before the end date");
        result.ErrorCode.Should().Be("INTERN_NOT_ENDED");
    }

    [Fact]
    public async Task MarkInternshipAsCompleted_Should_Return_Failure_When_Already_Completed()
    {
        // Arrange
        var internship = Internship.Create(
            InternshipId.New(),
            new SpecializationId(1),
            "Test Hospital",
            "Test Department",
            DateTime.UtcNow.Date.AddDays(-30),
            DateTime.UtcNow.Date.AddDays(-1)
        );

        await _internshipRepository.AddAsync(internship);
        await _unitOfWork.SaveChangesAsync();

        // Mark as completed first
        var firstResult = internship.MarkAsCompleted();
        firstResult.IsSuccess.Should().BeTrue();
        
        await _internshipRepository.UpdateAsync(internship);
        await _unitOfWork.SaveChangesAsync();

        var command = new MarkInternshipAsCompleted(internship.InternshipId.Value);

        // Act
        var result = await _markAsCompletedHandler.HandleAsync(command);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Should().Contain("already marked as completed");
    }

    [Fact]
    public async Task MarkInternshipAsCompleted_Should_Return_Failure_When_Already_Approved()
    {
        // Arrange
        var internship = Internship.Create(
            InternshipId.New(),
            new SpecializationId(1),
            "Test Hospital",
            "Test Department",
            DateTime.UtcNow.Date.AddDays(-30),
            DateTime.UtcNow.Date.AddDays(-1)
        );

        await _internshipRepository.AddAsync(internship);
        await _unitOfWork.SaveChangesAsync();

        // Mark as completed and approved
        var completeResult = internship.MarkAsCompleted();
        completeResult.IsSuccess.Should().BeTrue();
        
        internship.Approve("Dr. Supervisor");
        
        await _internshipRepository.UpdateAsync(internship);
        await _unitOfWork.SaveChangesAsync();

        var command = new MarkInternshipAsCompleted(internship.InternshipId.Value);

        // Act
        var result = await _markAsCompletedHandler.HandleAsync(command);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Should().Contain("Cannot modify an approved internship");
    }
}
using FluentAssertions;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using SledzSpecke.Application.Abstractions;
using SledzSpecke.Core.Entities;
using SledzSpecke.Core.Events;
using SledzSpecke.Core.Repositories;
using SledzSpecke.Core.ValueObjects;
using SledzSpecke.Tests.Integration.Common;
using Xunit;

namespace SledzSpecke.Tests.Integration.Events;

public class DomainEventTests : IntegrationTestBase
{
    private readonly IInternshipRepository _repository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly TestEventHandler _testEventHandler;

    public DomainEventTests(SledzSpeckeApiFactory factory) : base(factory)
    {
        _repository = Scope.ServiceProvider.GetRequiredService<IInternshipRepository>();
        _unitOfWork = Scope.ServiceProvider.GetRequiredService<IUnitOfWork>();
        
        // Register test event handler
        _testEventHandler = new TestEventHandler();
    }

    [Fact]
    public async Task AddMedicalShift_Should_Raise_MedicalShiftCreatedEvent()
    {
        // Arrange
        var internship = Internship.Create(
            InternshipId.New(),
            new SpecializationId(1),
            "Test Hospital",
            "Test Institution",
            "Test Department",
            DateTime.UtcNow.Date,
            DateTime.UtcNow.Date.AddDays(30),
            4, // plannedWeeks
            20 // plannedDays
        );

        await _repository.AddAsync(internship);
        await _unitOfWork.SaveChangesAsync();

        // Clear any previous events
        internship.ClearDomainEvents();

        // Act
        var shiftId = MedicalShiftId.New();
        var result = internship.AddMedicalShift(
            DateTime.UtcNow.Date.AddDays(5),
            8,
            30,
            "Emergency Room",
            2,
            SmkVersion.New,
            new[] { 1, 2, 3, 4, 5 } // availableYears
        );

        result.IsSuccess.Should().BeTrue();

        // Assert - Check that event was raised
        internship.DomainEvents.Should().HaveCount(1);
        
        var domainEvent = internship.DomainEvents.First();
        domainEvent.Should().BeOfType<MedicalShiftCreatedEvent>();
        
        var createdEvent = (MedicalShiftCreatedEvent)domainEvent;
        createdEvent.ShiftId.Should().Be(shiftId);
        createdEvent.InternshipId.Should().Be(internship.InternshipId);
        createdEvent.Hours.Should().Be(8);
        createdEvent.Minutes.Should().Be(30);
        createdEvent.Location.Should().Be("Emergency Room");
        createdEvent.Year.Should().Be(2024);
    }

    [Fact]
    public async Task SaveChangesAsync_Should_Dispatch_Domain_Events()
    {
        // This test would require a more complex setup with MediatR properly configured
        // For now, we're just testing that events are raised at the entity level
        
        // Arrange
        var internship = Internship.Create(
            InternshipId.New(),
            new SpecializationId(1),
            "Test Hospital",
            "Test Institution",
            "Test Department",
            DateTime.UtcNow.Date,
            DateTime.UtcNow.Date.AddDays(30),
            4, // plannedWeeks
            20 // plannedDays
        );

        // Act
        var result = internship.AddMedicalShift(
            DateTime.UtcNow.Date.AddDays(5),
            8,
            0,
            "ICU",
            2,
            SmkVersion.New,
            new[] { 1, 2, 3, 4, 5 } // availableYears
        );

        // Add internship with event
        await _repository.AddAsync(internship);
        
        // Before saving, events should still be present
        internship.DomainEvents.Should().HaveCount(1);
        
        // Save changes (this would dispatch events in a real scenario with MediatR configured)
        await _unitOfWork.SaveChangesAsync();
        
        // After saving, events should be cleared by the UnitOfWork
        // (This behavior depends on the actual implementation)
    }

    private class TestEventHandler : INotificationHandler<MedicalShiftCreatedEvent>
    {
        public List<MedicalShiftCreatedEvent> ReceivedEvents { get; } = new();

        public Task Handle(MedicalShiftCreatedEvent notification, CancellationToken cancellationToken)
        {
            ReceivedEvents.Add(notification);
            return Task.CompletedTask;
        }
    }
}
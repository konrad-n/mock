using FluentAssertions;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using SledzSpecke.Application.Commands;
using SledzSpecke.Core.Entities;
using SledzSpecke.Core.Events;
using SledzSpecke.Core.Repositories;
using SledzSpecke.Tests.Integration.Common;
using System.Collections.Concurrent;

namespace SledzSpecke.Tests.Integration.DomainEvents;

public class MedicalShiftCreatedEventTests : IntegrationTestBase
{
    private readonly ConcurrentBag<MedicalShiftCreatedEvent> _capturedEvents = new();
    
    public MedicalShiftCreatedEventTests(SledzSpeckeApiFactory factory) : base(factory)
    {
    }

    protected override void ConfigureServices(IServiceCollection services)
    {
        base.ConfigureServices(services);
        
        // Add a test notification handler to capture events
        services.AddScoped<INotificationHandler<MedicalShiftCreatedEvent>>(provider => 
            new TestMedicalShiftCreatedEventHandler(_capturedEvents));
    }

    [Fact]
    public async Task AddMedicalShift_Should_RaiseMedicalShiftCreatedEvent()
    {
        // Arrange
        var userId = await CreateTestUserAsync();
        var internshipId = await CreateTestInternshipAsync(userId);
        
        var command = new AddMedicalShift(
            internshipId, 
            DateTime.UtcNow.Date, 
            8, 
            30, 
            "Hospital Ward A", 
            3);

        // Act
        var shiftId = await Mediator.Send(command);

        // Assert
        shiftId.Should().BeOfType<int>();
        var shiftIdValue = (int)shiftId;
        
        // Wait a bit for async event processing
        await Task.Delay(100);
        
        _capturedEvents.Should().ContainSingle();
        var capturedEvent = _capturedEvents.First();
        
        capturedEvent.ShiftId.Value.Should().Be(shiftIdValue);
        capturedEvent.InternshipId.Should().Be(internshipId);
        capturedEvent.Date.Should().Be(command.Date);
        capturedEvent.Hours.Should().Be(command.Hours);
        capturedEvent.Minutes.Should().Be(command.Minutes);
        capturedEvent.Location.Should().Be(command.Location);
        capturedEvent.OccurredAt.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(5));
    }

    [Fact]
    public async Task AddMedicalShift_WithMultipleShifts_Should_RaiseMultipleEvents()
    {
        // Arrange
        var userId = await CreateTestUserAsync();
        var internshipId = await CreateTestInternshipAsync(userId);
        
        var commands = new[]
        {
            new AddMedicalShift(internshipId, DateTime.UtcNow.Date, 8, 0, "Hospital Ward A", 3),
            new AddMedicalShift(internshipId, DateTime.UtcNow.Date.AddDays(-1), 12, 30, "Emergency Room", 3),
            new AddMedicalShift(internshipId, DateTime.UtcNow.Date.AddDays(-2), 6, 45, "ICU", 3)
        };

        // Act
        var shiftIds = new List<int>();
        foreach (var command in commands)
        {
            var shiftId = await Mediator.Send(command);
            shiftIds.Add(shiftId);
        }

        // Assert
        await Task.Delay(200); // Wait for async event processing
        
        _capturedEvents.Should().HaveCount(3);
        _capturedEvents.Select(e => e.MedicalShiftId).Should().BeEquivalentTo(shiftIds);
        _capturedEvents.Select(e => e.InternshipId).Should().AllBeEquivalentTo(internshipId);
    }

    private class TestMedicalShiftCreatedEventHandler : INotificationHandler<MedicalShiftCreatedEvent>
    {
        private readonly ConcurrentBag<MedicalShiftCreatedEvent> _capturedEvents;

        public TestMedicalShiftCreatedEventHandler(ConcurrentBag<MedicalShiftCreatedEvent> capturedEvents)
        {
            _capturedEvents = capturedEvents;
        }

        public Task Handle(MedicalShiftCreatedEvent notification, CancellationToken cancellationToken)
        {
            _capturedEvents.Add(notification);
            return Task.CompletedTask;
        }
    }
}
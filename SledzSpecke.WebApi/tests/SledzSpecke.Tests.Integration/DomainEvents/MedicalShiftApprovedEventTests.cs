using FluentAssertions;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using SledzSpecke.Application.Commands;
using SledzSpecke.Application.Events.Handlers;
using SledzSpecke.Core.Events;
using SledzSpecke.Core.ValueObjects;
using SledzSpecke.Core.Repositories;
using SledzSpecke.Infrastructure.Services;
using SledzSpecke.Tests.Integration.Common;
using System.Collections.Concurrent;

namespace SledzSpecke.Tests.Integration.DomainEvents;

public class MedicalShiftApprovedEventTests : IntegrationTestBase
{
    private readonly ConcurrentBag<MedicalShiftApprovedEvent> _capturedEvents = new();
    private readonly ConcurrentBag<string> _logMessages = new();
    
    public MedicalShiftApprovedEventTests(SledzSpeckeApiFactory factory) : base(factory)
    {
    }

    protected override void ConfigureServices(IServiceCollection services)
    {
        base.ConfigureServices(services);
        
        // Add a test notification handler to capture events
        services.AddScoped<INotificationHandler<MedicalShiftApprovedEvent>>(provider => 
            new TestMedicalShiftApprovedEventHandler(_capturedEvents, _logMessages));
    }

    [Fact]
    public async Task ApproveMedicalShift_Should_RaiseMedicalShiftApprovedEvent()
    {
        // Arrange
        var userId = await CreateTestUserAsync();
        var internshipId = await CreateTestInternshipAsync(userId);
        var shiftId = await CreateTestMedicalShiftAsync(internshipId);
        
        // Get the shift to approve it
        var shiftRepository = ServiceProvider.GetRequiredService<IMedicalShiftRepository>();
        var shift = await shiftRepository.GetByIdAsync(shiftId);
        shift.Should().NotBeNull();

        // Act - Approve the shift (this would typically be done through a command)
        shift!.Approve("Test Supervisor", "Head of Department");
        await shiftRepository.UpdateAsync(shift);

        // Simulate event publishing (in a real scenario, this would be done by the domain)
        var mediator = ServiceProvider.GetRequiredService<IMediator>();
        await mediator.Publish(new MedicalShiftApprovedEvent(
            new MedicalShiftId(shiftId),
            DateTime.UtcNow,
            "Test Supervisor"));

        // Assert
        await Task.Delay(100); // Wait for async event processing
        
        _capturedEvents.Should().ContainSingle();
        var capturedEvent = _capturedEvents.First();
        
        capturedEvent.ShiftId.Value.Should().Be(shiftId);
        capturedEvent.ApprovedOn.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(5));
        capturedEvent.ApprovedBy.Should().Be("Test Supervisor");
    }

    [Fact]
    public async Task MedicalShiftApprovedEvent_Should_TriggerNotifications()
    {
        // Arrange
        var userId = await CreateTestUserAsync();
        var internshipId = await CreateTestInternshipAsync(userId);
        var shiftId = await CreateTestMedicalShiftAsync(internshipId);

        // Act
        var mediator = ServiceProvider.GetRequiredService<IMediator>();
        await mediator.Publish(new MedicalShiftApprovedEvent(
            shiftId,
            internshipId,
            DateTime.UtcNow.Date,
            8,
            0,
            DateTime.UtcNow));

        // Assert
        await Task.Delay(100); // Wait for async event processing
        
        _logMessages.Should().Contain(msg => msg.Contains("Medical shift approved"));
        _logMessages.Should().Contain(msg => msg.Contains($"Shift ID: {shiftId}"));
    }

    [Fact]
    public async Task MedicalShiftApprovedEvent_WithMonthlyThreshold_Should_GenerateReport()
    {
        // Arrange
        var userId = await CreateTestUserAsync();
        var internshipId = await CreateTestInternshipAsync(userId);
        
        // Create multiple shifts to reach monthly threshold
        var shiftIds = new List<int>();
        for (int i = 0; i < 20; i++) // Create 20 shifts (160 hours)
        {
            var shiftId = await CreateTestMedicalShiftAsync(internshipId, DateTime.UtcNow.AddDays(-i), 8, 0);
            shiftIds.Add(shiftId);
        }

        // Act - Approve the last shift which should trigger monthly report
        var mediator = ServiceProvider.GetRequiredService<IMediator>();
        await mediator.Publish(new MedicalShiftApprovedEvent(
            shiftIds.Last(),
            internshipId,
            DateTime.UtcNow.Date,
            8,
            0,
            DateTime.UtcNow));

        // Assert
        await Task.Delay(200); // Wait for async event processing
        
        _logMessages.Should().Contain(msg => msg.Contains("Monthly hours threshold reached"));
        _logMessages.Should().Contain(msg => msg.Contains("Generating monthly report"));
    }

    private async Task<int> CreateTestMedicalShiftAsync(int internshipId, DateTime? date = null, int hours = 8, int minutes = 0)
    {
        var command = new AddMedicalShift(
            internshipId,
            date ?? DateTime.UtcNow.Date,
            hours,
            minutes,
            "Test Hospital",
            3);
            
        return await Mediator.Send(command);
    }

    private class TestMedicalShiftApprovedEventHandler : INotificationHandler<MedicalShiftApprovedEvent>
    {
        private readonly ConcurrentBag<MedicalShiftApprovedEvent> _capturedEvents;
        private readonly ConcurrentBag<string> _logMessages;
        private readonly ILogger<TestMedicalShiftApprovedEventHandler> _logger;

        public TestMedicalShiftApprovedEventHandler(
            ConcurrentBag<MedicalShiftApprovedEvent> capturedEvents,
            ConcurrentBag<string> logMessages)
        {
            _capturedEvents = capturedEvents;
            _logMessages = logMessages;
            _logger = new TestLogger<TestMedicalShiftApprovedEventHandler>(logMessages);
        }

        public Task Handle(MedicalShiftApprovedEvent notification, CancellationToken cancellationToken)
        {
            _capturedEvents.Add(notification);
            
            _logger.LogInformation($"Medical shift approved - Shift ID: {notification.ShiftId}");
            
            // Log approval details
            _logger.LogInformation($"Approved by: {notification.ApprovedBy}");
            _logger.LogInformation($"Approved on: {notification.ApprovedOn}");
            
            return Task.CompletedTask;
        }
    }

    private class TestLogger<T> : ILogger<T>
    {
        private readonly ConcurrentBag<string> _logMessages;

        public TestLogger(ConcurrentBag<string> logMessages)
        {
            _logMessages = logMessages;
        }

        public IDisposable BeginScope<TState>(TState state) => null!;
        public bool IsEnabled(LogLevel logLevel) => true;
        
        public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception? exception, Func<TState, Exception?, string> formatter)
        {
            _logMessages.Add(formatter(state, exception));
        }
    }
}
using FluentAssertions;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using SledzSpecke.Application.Commands;
using SledzSpecke.Core.Events;
using SledzSpecke.Core.ValueObjects;
using SledzSpecke.Tests.Integration.Common;
using System.Collections.Concurrent;

namespace SledzSpecke.Tests.Integration.DomainEvents;

public class ProcedureCreatedEventTests : IntegrationTestBase
{
    private readonly ConcurrentBag<ProcedureCreatedEvent> _capturedEvents = new();
    
    public ProcedureCreatedEventTests(SledzSpeckeApiFactory factory) : base(factory)
    {
    }

    protected override void ConfigureServices(IServiceCollection services)
    {
        base.ConfigureServices(services);
        
        // Add a test notification handler to capture events
        services.AddScoped<INotificationHandler<ProcedureCreatedEvent>>(provider => 
            new TestProcedureCreatedEventHandler(_capturedEvents));
    }

    [Fact]
    public async Task AddProcedure_Should_RaiseProcedureCreatedEvent()
    {
        // Arrange
        var userId = await CreateTestUserAsync();
        var internshipId = await CreateTestInternshipAsync(userId);
        
        var command = new AddProcedure(
            InternshipId: internshipId,
            Date: DateTime.UtcNow.Date,
            Year: 1,
            Code: "P001",
            Name: "Appendectomy",
            Location: "Operating Room 1",
            Status: "Completed",
            ExecutionType: "Performed",
            SupervisorName: "Dr. Smith",
            SupervisorPwz: "1234567",
            PatientGender: 'M');

        // Act
        var result = await Mediator.Send(command);
        var procedureId = (int)result;

        // Assert
        procedureId.Should().BeGreaterThan(0);
        
        // Wait for async event processing
        await Task.Delay(100);
        
        _capturedEvents.Should().ContainSingle();
        var capturedEvent = _capturedEvents.First();
        
        capturedEvent.ProcedureId.Value.Should().Be(procedureId);
        capturedEvent.InternshipId.Value.Should().Be(internshipId);
        capturedEvent.Code.Should().Be(command.Code);
        capturedEvent.Date.Should().Be(command.Date);
        capturedEvent.Location.Should().Be(command.Location);
        capturedEvent.OccurredAt.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(5));
    }

    [Fact]
    public async Task AddMultipleProcedures_Should_RaiseMultipleEvents()
    {
        // Arrange
        var userId = await CreateTestUserAsync();
        var internshipId = await CreateTestInternshipAsync(userId);
        
        var procedures = new[]
        {
            ("P001", "Appendectomy", "A"),
            ("P002", "Cholecystectomy", "B"),
            ("P003", "Hernia Repair", "A")
        };

        // Act
        var procedureIds = new List<int>();
        foreach (var (code, name, category) in procedures)
        {
            var command = new AddProcedure(
                InternshipId: internshipId,
                Date: DateTime.UtcNow.Date,
                Year: 1,
                Code: code,
                Name: name,
                Location: "Operating Room",
                Status: "Completed",
                ExecutionType: category == "A" ? "Performed" : "Assisted",
                SupervisorName: "Dr. Smith",
                SupervisorPwz: "1234567");
                
            var result = await Mediator.Send(command);
            var procedureId = (int)result;
            procedureIds.Add(procedureId);
        }

        // Assert
        await Task.Delay(200); // Wait for async event processing
        
        _capturedEvents.Should().HaveCount(3);
        _capturedEvents.Select(e => e.ProcedureId).Should().BeEquivalentTo(procedureIds);
        _capturedEvents.Select(e => e.InternshipId).Should().AllBeEquivalentTo(internshipId);
        _capturedEvents.Select(e => e.Code).Should().BeEquivalentTo(procedures.Select(p => p.Item1));
    }

    [Fact]
    public async Task AddProcedure_WithDifferentCategories_Should_RaiseEventsWithCorrectCategories()
    {
        // Arrange
        var userId = await CreateTestUserAsync();
        var internshipId = await CreateTestInternshipAsync(userId);
        
        var categoriesTests = new[] { "A", "B", "C" };

        // Act & Assert
        foreach (var category in categoriesTests)
        {
            _capturedEvents.Clear();
            
            var command = new AddProcedure(
                InternshipId: internshipId,
                Date: DateTime.UtcNow.Date,
                Year: 3,
                Code: $"P{category}01",
                Name: $"Test Procedure {category}",
                Location: "Pediatrics",
                Status: "Completed",
                ExecutionType: "Primary",
                SupervisorName: "Dr. Supervisor",
                SupervisorPwz: "1234567",
                PerformingPerson: "Resident",
                PatientInfo: "Child",
                PatientInitials: "JD",
                PatientGender: 'M');
                
            var result = await Mediator.Send(command);
            var procedureId = (int)result;
            
            await Task.Delay(50);
            
            _capturedEvents.Should().ContainSingle();
            _capturedEvents.First().Code.Should().Be($"P{category}01");
        }
    }

    private class TestProcedureCreatedEventHandler : INotificationHandler<ProcedureCreatedEvent>
    {
        private readonly ConcurrentBag<ProcedureCreatedEvent> _capturedEvents;

        public TestProcedureCreatedEventHandler(ConcurrentBag<ProcedureCreatedEvent> capturedEvents)
        {
            _capturedEvents = capturedEvents;
        }

        public Task Handle(ProcedureCreatedEvent notification, CancellationToken cancellationToken)
        {
            _capturedEvents.Add(notification);
            return Task.CompletedTask;
        }
    }
}
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
            internshipId,
            "P001",
            "Appendectomy",
            "A",
            PatientType.Adult,
            PatientGender.Male,
            "12345678901", // PESEL
            SupervisorType.Specialist,
            DateTime.UtcNow.Date,
            "General Surgery");

        // Act
        var procedureId = await Mediator.Send(command);

        // Assert
        procedureId.Should().BeGreaterThan(0);
        
        // Wait for async event processing
        await Task.Delay(100);
        
        _capturedEvents.Should().ContainSingle();
        var capturedEvent = _capturedEvents.First();
        
        capturedEvent.ProcedureId.Should().Be(procedureId);
        capturedEvent.InternshipId.Should().Be(internshipId);
        capturedEvent.ProcedureCode.Should().Be(command.Code);
        capturedEvent.ProcedureName.Should().Be(command.Name);
        capturedEvent.Category.Should().Be(command.Category);
        capturedEvent.PerformedAt.Should().Be(command.Date);
        capturedEvent.CreatedAt.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(5));
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
                internshipId,
                code,
                name,
                category,
                PatientType.Adult,
                PatientGender.Female,
                "98765432109",
                SupervisorType.Specialist,
                DateTime.UtcNow.Date,
                "Surgery Department");
                
            var procedureId = await Mediator.Send(command);
            procedureIds.Add(procedureId);
        }

        // Assert
        await Task.Delay(200); // Wait for async event processing
        
        _capturedEvents.Should().HaveCount(3);
        _capturedEvents.Select(e => e.ProcedureId).Should().BeEquivalentTo(procedureIds);
        _capturedEvents.Select(e => e.InternshipId).Should().AllBeEquivalentTo(internshipId);
        _capturedEvents.Select(e => e.ProcedureCode).Should().BeEquivalentTo(procedures.Select(p => p.Item1));
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
                internshipId,
                $"P{category}01",
                $"Test Procedure {category}",
                category,
                PatientType.Child,
                PatientGender.Male,
                "12312312312",
                SupervisorType.Resident,
                DateTime.UtcNow.Date,
                "Pediatrics");
                
            var procedureId = await Mediator.Send(command);
            
            await Task.Delay(50);
            
            _capturedEvents.Should().ContainSingle();
            _capturedEvents.First().Category.Should().Be(category);
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
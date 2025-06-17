using MediatR;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using SledzSpecke.Application.Commands;
using SledzSpecke.Core.Entities;
using SledzSpecke.Core.ValueObjects;
using SledzSpecke.Infrastructure.DAL;
using Xunit;

namespace SledzSpecke.Tests.Integration.Common;

public abstract class IntegrationTestBase : IClassFixture<SledzSpeckeApiFactory>, IAsyncLifetime
{
    protected readonly SledzSpeckeApiFactory Factory;
    protected readonly HttpClient Client;
    protected IServiceScope Scope = null!;
    protected SledzSpeckeDbContext DbContext = null!;
    protected IMediator Mediator = null!;
    protected IServiceProvider ServiceProvider = null!;

    protected IntegrationTestBase(SledzSpeckeApiFactory factory)
    {
        Factory = factory;
        Client = Factory.CreateClient();
    }

    public virtual async Task InitializeAsync()
    {
        Scope = Factory.Services.CreateScope();
        ServiceProvider = Scope.ServiceProvider;
        DbContext = ServiceProvider.GetRequiredService<SledzSpeckeDbContext>();
        Mediator = ServiceProvider.GetRequiredService<IMediator>();
        
        // Configure additional services if needed
        ConfigureServices(Scope.ServiceProvider.GetRequiredService<IServiceCollection>());
        
        // Ensure database is created
        await DbContext.Database.EnsureCreatedAsync();
    }

    public virtual async Task DisposeAsync()
    {
        await ClearDatabaseAsync();
        Scope?.Dispose();
    }

    protected virtual void ConfigureServices(IServiceCollection services)
    {
        // Override in derived classes to add test-specific services
    }

    protected async Task<int> CreateTestUserAsync(string? email = null, string? pesel = null)
    {
        var command = new SignUp(
            email ?? $"test{Guid.NewGuid()}@example.com",
            "Test123!",
            "John",
            "Doe",
            pesel ?? "92010112345", // Valid PESEL
            Gender.Male,
            "1234567", // PWZ
            "Warsaw Medical University",
            3,
            "Cardiology",
            "old",
            "12-345",
            "ul. Testowa 1",
            "Warsaw",
            "Mazowieckie",
            "+48123456789");

        await Mediator.Send(command);
        
        // Get the created user
        var user = await DbContext.Users.FirstOrDefaultAsync(u => u.Email.Value == command.Email);
        return user?.Id.Value ?? throw new InvalidOperationException("User not created");
    }

    protected async Task<int> CreateTestInternshipAsync(int userId)
    {
        var command = new CreateInternship(
            userId,
            "Test Hospital",
            "Cardiology Ward",
            DateTime.UtcNow.Date,
            DateTime.UtcNow.Date.AddMonths(3),
            "Test Supervisor",
            "supervisor@hospital.com",
            "+48987654321");

        return await Mediator.Send(command);
    }

    protected async Task<int> CreateTestMedicalShiftAsync(int internshipId)
    {
        var command = new AddMedicalShift(
            internshipId,
            DateTime.UtcNow.Date,
            8,
            0,
            "Hospital Ward",
            3);

        return await Mediator.Send(command);
    }

    protected async Task ClearDatabaseAsync()
    {
        // Clear all data from the database in the correct order to avoid FK violations
        DbContext.RemoveRange(DbContext.Procedures);
        DbContext.RemoveRange(DbContext.MedicalShifts);
        DbContext.RemoveRange(DbContext.Courses);
        DbContext.RemoveRange(DbContext.Absences);
        DbContext.RemoveRange(DbContext.Recognitions);
        DbContext.RemoveRange(DbContext.Publications);
        DbContext.RemoveRange(DbContext.SelfEducations);
        DbContext.RemoveRange(DbContext.Internships);
        DbContext.RemoveRange(DbContext.Modules);
        DbContext.RemoveRange(DbContext.Specializations);
        DbContext.RemoveRange(DbContext.Users);
        
        await DbContext.SaveChangesAsync();
    }
}
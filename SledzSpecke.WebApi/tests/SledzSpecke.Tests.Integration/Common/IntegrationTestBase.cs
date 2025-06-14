using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using SledzSpecke.Infrastructure.DAL;
using System;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace SledzSpecke.Tests.Integration.Common;

public abstract class IntegrationTestBase : IDisposable
{
    protected readonly WebApplicationFactory<Program> Factory;
    protected readonly HttpClient Client;
    protected readonly IServiceScope Scope;
    protected readonly SledzSpeckeDbContext DbContext;

    protected IntegrationTestBase()
    {
        Factory = new WebApplicationFactory<Program>()
            .WithWebHostBuilder(builder =>
            {
                builder.ConfigureServices(services =>
                {
                    // Remove the existing DbContext registration
                    var descriptor = services.SingleOrDefault(
                        d => d.ServiceType == typeof(DbContextOptions<SledzSpeckeDbContext>));

                    if (descriptor != null)
                    {
                        services.Remove(descriptor);
                    }

                    // Add in-memory database for testing
                    services.AddDbContext<SledzSpeckeDbContext>(options =>
                    {
                        options.UseInMemoryDatabase($"TestDb_{Guid.NewGuid()}");
                    });
                });
            });

        Client = Factory.CreateClient();
        Scope = Factory.Services.CreateScope();
        DbContext = Scope.ServiceProvider.GetRequiredService<SledzSpeckeDbContext>();
        
        // Ensure database is created
        DbContext.Database.EnsureCreated();
    }

    protected async Task<T> GetServiceAsync<T>() where T : notnull
    {
        return Scope.ServiceProvider.GetRequiredService<T>();
    }

    protected async Task ClearDatabaseAsync()
    {
        // Clear all data from the database
        DbContext.RemoveRange(DbContext.Procedures);
        DbContext.RemoveRange(DbContext.Internships);
        DbContext.RemoveRange(DbContext.MedicalShifts);
        DbContext.RemoveRange(DbContext.Courses);
        DbContext.RemoveRange(DbContext.Absences);
        DbContext.RemoveRange(DbContext.Recognitions);
        DbContext.RemoveRange(DbContext.Publications);
        DbContext.RemoveRange(DbContext.SelfEducations);
        DbContext.RemoveRange(DbContext.Users);
        DbContext.RemoveRange(DbContext.Modules);
        DbContext.RemoveRange(DbContext.Specializations);
        
        await DbContext.SaveChangesAsync();
    }

    public void Dispose()
    {
        Scope?.Dispose();
        Factory?.Dispose();
    }
}
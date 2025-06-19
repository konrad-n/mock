using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using SledzSpecke.Application.Abstractions;
using SledzSpecke.Application.Security;
using SledzSpecke.Application.SpecializationTemplates.Services;
using SledzSpecke.Core.Abstractions;
using SledzSpecke.Core.Repositories;
using SledzSpecke.Core.SpecializationTemplates;
using SledzSpecke.Infrastructure.Auth;
using SledzSpecke.Infrastructure.DAL;
using SledzSpecke.Infrastructure.DAL.Repositories;
using SledzSpecke.Infrastructure.DAL.Seeding;
using SledzSpecke.Infrastructure.Decorators;
using SledzSpecke.Infrastructure.Repositories;
using SledzSpecke.Infrastructure.Security;
using SledzSpecke.Infrastructure.Services;
using SledzSpecke.Infrastructure.Time;
using System.Text;

namespace SledzSpecke.Infrastructure;

/// <summary>
/// Enhanced infrastructure registration following MySpot patterns
/// </summary>
public static class ExtensionsEnhanced
{
    public static IServiceCollection AddInfrastructureEnhanced(this IServiceCollection services, IConfiguration configuration)
    {
        // Core services
        services.AddSingleton<IClock, Clock>();
        services.AddSingleton<IPasswordManager, PasswordManager>();
        services.AddSingleton<IAuthenticator, Authenticator>();
        services.AddScoped<IUserContextService, UserContextService>();

        // Template services
        services.AddSingleton<ISpecializationTemplateService, SpecializationTemplateService>();

        // Unit of Work
        services.AddScoped<IUnitOfWork, UnitOfWork>();

        // Repositories
        services.AddRepositories();

        // Database
        services.AddDatabase(configuration);

        // Authentication & Authorization
        services.AddAuth(configuration);

        // Decorators
        services.AddDecorators();

        return services;
    }

    private static IServiceCollection AddRepositories(this IServiceCollection services)
    {
        services.AddScoped<IUserRepository, RefactoredSqlUserRepository>();
        services.AddScoped<ISpecializationRepository, SqlSpecializationRepository>();
        services.AddScoped<IModuleRepository, SqlModuleRepository>();
        services.AddScoped<IInternshipRepository, SqlInternshipRepositoryEnhanced>();
        services.AddScoped<IProcedureRepository, SqlProcedureRepository>();
        services.AddScoped<IMedicalShiftRepository, SqlMedicalShiftRepository>();
        services.AddScoped<ICourseRepository, SqlCourseRepository>();
        services.AddScoped<IAbsenceRepository, SqlAbsenceRepository>();
        services.AddScoped<IRecognitionRepository, SqlRecognitionRepository>();
        services.AddScoped<IPublicationRepository, SqlPublicationRepository>();
        services.AddScoped<ISelfEducationRepository, SqlSelfEducationRepository>();
        services.AddScoped<ISpecializationTemplateRepository, SpecializationTemplateRepository>();
        services.AddScoped<IDataSeeder, DataSeeder>();
        
        // Specialization Template Services
        services.AddScoped<ISpecializationTemplateImportService, SpecializationTemplateImportService>();

        return services;
    }

    private static IServiceCollection AddDatabase(this IServiceCollection services, IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("DefaultConnection");

        services.AddDbContext<SledzSpeckeDbContext>(options =>
        {
            options.UseNpgsql(connectionString);

            // Enable sensitive data logging in development
            if (Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") == "Development")
            {
                options.EnableSensitiveDataLogging();
                options.EnableDetailedErrors();
            }
        });

        // Add health check for database (requires AspNetCore.HealthChecks.Npgsql package)
        // services.AddHealthChecks()
        //     .AddNpgSql(connectionString!, name: "database");

        return services;
    }

    private static IServiceCollection AddAuth(this IServiceCollection services, IConfiguration configuration)
    {
        var authOptions = configuration.GetSection("auth").Get<AuthOptions>();
        services.Configure<AuthOptions>(configuration.GetSection("auth"));

        services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            .AddJwtBearer(options =>
            {
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    ValidIssuer = authOptions?.Issuer,
                    ValidAudience = authOptions?.Audience,
                    IssuerSigningKey = new SymmetricSecurityKey(
                        Encoding.UTF8.GetBytes(authOptions?.SigningKey ?? string.Empty)),
                    ClockSkew = TimeSpan.Zero
                };

                options.Events = new JwtBearerEvents
                {
                    OnAuthenticationFailed = context =>
                    {
                        if (context.Exception.GetType() == typeof(SecurityTokenExpiredException))
                        {
                            context.Response.Headers["Token-Expired"] = "true";
                        }
                        return Task.CompletedTask;
                    }
                };
            });

        services.AddAuthorization(options =>
        {
            options.AddPolicy("RequireUser", policy => policy.RequireRole("user"));
            options.AddPolicy("RequireAdmin", policy => policy.RequireRole("admin"));
            options.AddPolicy("RequireSupervisor", policy => policy.RequireRole("supervisor"));
            options.AddPolicy("AdminOnly", policy => policy.RequireRole("admin"));
        });

        return services;
    }

    private static IServiceCollection AddDecorators(this IServiceCollection services)
    {
        // Note: This is a simplified version. 
        // In a real implementation, you'd use Scrutor or a similar library for automatic decoration

        // Example of manual decoration for a specific handler:
        // services.Decorate<ICommandHandler<AddProcedure, int>, LoggingCommandHandlerDecorator<AddProcedure, int>>();
        // services.Decorate<ICommandHandler<AddProcedure, int>, UnitOfWorkCommandHandlerDecorator<AddProcedure, int>>();

        return services;
    }

    // Extension method for automatic handler decoration
    public static IServiceCollection AddHandlerDecorators(this IServiceCollection services)
    {
        // This would require Scrutor package:
        // services.Scan(scan => scan
        //     .FromAssembliesOf(typeof(ICommandHandler<>))
        //     .AddClasses(classes => classes.AssignableTo(typeof(ICommandHandler<>)))
        //     .AsImplementedInterfaces()
        //     .WithScopedLifetime()
        //     .DecorateAllWith(typeof(LoggingCommandHandlerDecorator<>))
        //     .DecorateAllWith(typeof(UnitOfWorkCommandHandlerDecorator<>)));

        return services;
    }
}
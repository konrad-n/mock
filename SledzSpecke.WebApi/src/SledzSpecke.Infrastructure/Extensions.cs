using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using SledzSpecke.Application.Abstractions;
using SledzSpecke.Application.Security;
using SledzSpecke.Core.Abstractions;
using SledzSpecke.Core.Repositories;
using SledzSpecke.Infrastructure.Auth;
using SledzSpecke.Infrastructure.DAL;
using SledzSpecke.Infrastructure.DAL.Repositories;
using SledzSpecke.Infrastructure.DAL.Seeding;
using SledzSpecke.Infrastructure.Repositories;
using SledzSpecke.Infrastructure.Security;
using SledzSpecke.Infrastructure.Services;
using SledzSpecke.Infrastructure.Time;
using SledzSpecke.Infrastructure.Exceptions;
using SledzSpecke.Infrastructure.Options;
using SledzSpecke.Infrastructure.Events;
using SledzSpecke.Infrastructure.Export;
using SledzSpecke.Core.DomainServices;
using System.Text;

namespace SledzSpecke.Infrastructure;

public static class Extensions
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        // Core services
        services.AddControllersWithViews()
            .AddJsonOptions(options =>
            {
                options.JsonSerializerOptions.PropertyNamingPolicy = null;
                options.JsonSerializerOptions.WriteIndented = true;
            });

        services.AddHttpContextAccessor();
        services.AddEndpointsApiExplorer();
        services.AddSwaggerGen(swagger =>
        {
            swagger.SwaggerDoc("v1", new OpenApiInfo
            {
                Title = "SledzSpecke API",
                Version = "v1",
                Description = "Medical Specialization Tracking System API - SMK Compliant",
                Contact = new OpenApiContact
                {
                    Name = "SledzSpecke Team",
                    Email = "support@sledzspecke.pl"
                }
            });

            // Add JWT authentication
            swagger.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
            {
                In = ParameterLocation.Header,
                Description = "Please enter JWT with Bearer prefix",
                Name = "Authorization",
                Type = SecuritySchemeType.ApiKey,
                Scheme = "Bearer"
            });

            swagger.AddSecurityRequirement(new OpenApiSecurityRequirement
            {
                {
                    new OpenApiSecurityScheme
                    {
                        Reference = new OpenApiReference
                        {
                            Type = ReferenceType.SecurityScheme,
                            Id = "Bearer"
                        }
                    },
                    Array.Empty<string>()
                }
            });

            // Include XML comments if available
            var xmlFile = $"{System.Reflection.Assembly.GetExecutingAssembly().GetName().Name}.xml";
            var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
            if (File.Exists(xmlPath))
            {
                swagger.IncludeXmlComments(xmlPath);
            }

            // Include API XML comments
            var apiXmlFile = "SledzSpecke.Api.xml";
            var apiXmlPath = Path.Combine(AppContext.BaseDirectory, apiXmlFile);
            if (File.Exists(apiXmlPath))
            {
                swagger.IncludeXmlComments(apiXmlPath);
            }

            // Custom schema IDs
            swagger.CustomSchemaIds(type => type.FullName);
        });

        // Register middleware
        services.AddSingleton<ExceptionHandlingMiddleware>();

        // Infrastructure services
        services.AddSingleton<IClock, Clock>();
        services.AddSingleton<IPasswordManager, PasswordManager>();
        services.AddSingleton<IAuthenticator, Authenticator>();
        services.AddScoped<IUserContextService, UserContextService>();
        services.AddScoped<IUserContext, UserContext>();
        
        // Domain event dispatcher
        services.AddScoped<IDomainEventDispatcher, DomainEventDispatcher>();

        // Template services
        services.AddSingleton<ISpecializationTemplateService, SpecializationTemplateService>();

        // Unit of Work
        services.AddScoped<IUnitOfWork, UnitOfWork>();

        // Application services
        services.AddScoped<INotificationService, NotificationService>();
        services.AddScoped<IStatisticsService, RealStatisticsService>();
        services.AddScoped<IProjectionService, ProjectionService>();
        services.AddScoped<IMilestoneService, MilestoneService>();
        services.AddScoped<IPdfGenerationService, PdfGenerationService>();
        services.AddScoped<IValidationService, ValidationService>();
        
        // Cache service
        services.AddMemoryCache();
        services.AddSingleton<ICacheService, InMemoryCacheService>();

        // Using refactored repository with Specification pattern
        services.AddScoped<IUserRepository, RefactoredSqlUserRepository>();
        services.AddScoped<ISpecializationRepository, RefactoredSqlSpecializationRepository>();
        services.AddScoped<IModuleRepository, RefactoredSqlModuleRepository>();
        services.AddScoped<IInternshipRepository, RefactoredSqlInternshipRepository>();
        services.AddScoped<IProcedureRepository, RefactoredSqlProcedureRepository>();
        services.AddScoped<IMedicalShiftRepository, RefactoredSqlMedicalShiftRepository>();
        services.AddScoped<ICourseRepository, RefactoredSqlCourseRepository>();
        services.AddScoped<IAbsenceRepository, RefactoredSqlAbsenceRepository>();
        services.AddScoped<IRecognitionRepository, RefactoredSqlRecognitionRepository>();
        services.AddScoped<IPublicationRepository, RefactoredSqlPublicationRepository>();
        services.AddScoped<ISelfEducationRepository, RefactoredSqlSelfEducationRepository>();
        services.AddScoped<IEducationalActivityRepository, RefactoredEducationalActivityRepository>();
        services.AddScoped<IFileMetadataRepository, RefactoredFileMetadataRepository>();
        services.AddScoped<IAdditionalSelfEducationDaysRepository, RefactoredSqlAdditionalSelfEducationDaysRepository>();
        services.AddScoped<IFileStorageService, FileStorageService>();
        services.AddScoped<IDataSeeder, DataSeeder>();
        
        // Export services
        services.AddScoped<ISpecializationExportService, SmkExportService>();
        services.AddScoped<Export.ISmkExcelGenerator, Export.SmkExcelGenerator>();
        services.AddScoped<ISmkValidator, SmkValidator>();
        
        // Application layer export services
        services.AddScoped<Application.Abstractions.ISmkExcelGenerator, Services.ApplicationSmkExcelGenerator>();
        
        // Configure file storage options
        services.Configure<FileStorageOptions>(configuration.GetSection("FileStorage"));
        
        // Register background services
        services.AddHostedService<FileCleanupService>();

        services.AddDbContext<SledzSpeckeDbContext>(options =>
            options.UseNpgsql(configuration.GetConnectionString("DefaultConnection")));

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
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(authOptions?.SigningKey ?? string.Empty))
                };
            });

        services.AddAuthorization();

        return services;
    }

    public static WebApplication UseInfrastructure(this WebApplication app)
    {
        if (app.Environment.IsDevelopment())
        {
            app.UseSwagger();
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "SledzSpecke API v1");
                c.RoutePrefix = "swagger";
            });
        }

        app.UseHttpsRedirection();
        app.UseMiddleware<ExceptionHandlingMiddleware>();
        app.UseAuthentication();
        app.UseAuthorization();
        app.MapControllers();

        return app;
    }

    public static async Task<WebApplication> InitializeDatabaseAsync(this WebApplication app)
    {
        using var scope = app.Services.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<SledzSpeckeDbContext>();
        var seeder = scope.ServiceProvider.GetRequiredService<IDataSeeder>();
        var logger = scope.ServiceProvider.GetRequiredService<ILogger<SledzSpeckeDbContext>>();

        try
        {
            logger.LogInformation("Starting database initialization...");

            // Ensure database is created
            await context.Database.EnsureCreatedAsync();

            // Check if we need to run migrations
            var pendingMigrations = await context.Database.GetPendingMigrationsAsync();
            if (pendingMigrations.Any())
            {
                logger.LogInformation($"Applying {pendingMigrations.Count()} pending migrations...");
                await context.Database.MigrateAsync();
                logger.LogInformation("Migrations applied successfully.");
            }

            // Seed specialization templates if no data exists
            var hasSpecializations = await context.Specializations.AnyAsync();
            if (!hasSpecializations)
            {
                logger.LogInformation("Seeding specialization templates...");
                await seeder.SeedSpecializationTemplatesAsync();
            }

            // Seed basic data
            var hasUsers = await context.Users.AnyAsync();
            if (!hasUsers)
            {
                await seeder.SeedBasicDataAsync();
            }

            logger.LogInformation("Database initialization completed successfully.");
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "An error occurred during database initialization.");
            throw;
        }

        return app;
    }
}
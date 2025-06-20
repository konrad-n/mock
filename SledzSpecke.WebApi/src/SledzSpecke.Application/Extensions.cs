using Microsoft.Extensions.DependencyInjection;
using SledzSpecke.Application.Abstractions;
using SledzSpecke.Application.Validation;
using SledzSpecke.Application.Services;
using SledzSpecke.Application.DomainServices;
using SledzSpecke.Core.DomainServices;
using SledzSpecke.Application.Extensions;
using SledzSpecke.Application.Pipeline;
using System.Reflection;
using FluentValidation;
using SledzSpecke.Application.Queries;
using SledzSpecke.Application.Queries.Handlers;
using SledzSpecke.Application.DTO;
using SledzSpecke.Application.Features.MedicalShifts.DTOs;

namespace SledzSpecke.Application;

public static partial class ApplicationExtensions
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        var applicationAssembly = typeof(ICommandHandler<>).Assembly;
        
        // Register MediatR for domain events
        services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(applicationAssembly));

        // Auto-register all command handlers using Scrutor
        services.Scan(s => s.FromAssemblies(applicationAssembly)
            .AddClasses(c => c.AssignableTo(typeof(ICommandHandler<>)))
            .AsImplementedInterfaces()
            .WithScopedLifetime());

        services.Scan(s => s.FromAssemblies(applicationAssembly)
            .AddClasses(c => c.AssignableTo(typeof(ICommandHandler<,>)))
            .AsImplementedInterfaces()
            .WithScopedLifetime());

        // Auto-register all Result-based command handlers
        services.Scan(s => s.FromAssemblies(applicationAssembly)
            .AddClasses(c => c.AssignableTo(typeof(IResultCommandHandler<>))
                .Where(t => !t.Name.Contains("Decorator")))
            .AsImplementedInterfaces()
            .WithScopedLifetime());

        services.Scan(s => s.FromAssemblies(applicationAssembly)
            .AddClasses(c => c.AssignableTo(typeof(IResultCommandHandler<,>))
                .Where(t => !t.Name.Contains("Decorator")))
            .AsImplementedInterfaces()
            .WithScopedLifetime());

        // Auto-register all query handlers
        services.Scan(s => s.FromAssemblies(applicationAssembly)
            .AddClasses(c => c.AssignableTo(typeof(IQueryHandler<,>)))
            .AsImplementedInterfaces()
            .WithScopedLifetime());

        // Auto-register all Result-based query handlers
        services.Scan(s => s.FromAssemblies(applicationAssembly)
            .AddClasses(c => c.AssignableTo(typeof(IResultQueryHandler<,>))
                .Where(t => !t.Name.Contains("Decorator")))
            .AsImplementedInterfaces()
            .WithScopedLifetime());

        // Register specific services
        services.AddScoped<ISpecializationValidationService, SpecializationValidationService>();
        services.AddScoped<IYearCalculationService, YearCalculationService>();
        
        // Register Domain Services
        services.AddScoped<ISMKSynchronizationService, SimplifiedSMKSynchronizationService>();
        services.AddScoped<IModuleCompletionService, SimplifiedModuleCompletionService>();
        services.AddScoped<IModuleProgressionService, ModuleProgressionService>();
        services.AddScoped<IDurationCalculationService, DurationCalculationService>();
        services.AddScoped<IMedicalShiftValidationService, MedicalShiftValidationService>();
        services.AddScoped<IProcedureValidationService, ProcedureValidationService>();
        // TODO: Add implementations for remaining domain services
        // services.AddScoped<ISpecializationDurationService, SpecializationDurationService>();
        // services.AddScoped<IProcedureAllocationService, ProcedureAllocationService>();
        // services.AddScoped<IMedicalEducationComplianceService, MedicalEducationComplianceService>();
        services.AddScoped<IProgressCalculationService, ProgressCalculationService>();
        
        // Add validation
        services.AddValidation();
        
        // Register FluentValidation validators
        services.AddValidatorsFromAssembly(applicationAssembly);
        
        // Add decorators for cross-cutting concerns
        services.AddDecorators();
        
        // Add decorators for Result-based handlers
        services.AddResultDecorators();
        
        // Add message pipeline
        services.AddMessagePipeline();
        services.AddEnhancedMessagePipeline();

        // Register adapters for Result-based handlers to work with ICommandHandler interface
        // This bridges the gap between controllers expecting ICommandHandler and handlers implementing IResultCommandHandler
        services.AddResultToCommandHandlerAdapters();

        // Override specific query handlers with simple implementations
        services.AddScoped<IQueryHandler<GetDashboardOverview, DashboardOverviewDto>, GetDashboardOverviewHandlerSimple>();
        services.AddScoped<IQueryHandler<GetUserMedicalShifts, IEnumerable<MedicalShiftDto>>, GetUserMedicalShiftsHandlerSimple>();

        return services;
    }
}
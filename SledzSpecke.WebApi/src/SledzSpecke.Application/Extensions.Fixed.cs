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
    public static IServiceCollection AddApplicationFixed(this IServiceCollection services)
    {
        var applicationAssembly = typeof(ICommandHandler<>).Assembly;
        
        // Register MediatR for domain events
        services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(applicationAssembly));

        // Auto-register all command handlers using Scrutor - EXCLUDING decorators
        services.Scan(s => s.FromAssemblies(applicationAssembly)
            .AddClasses(c => c.AssignableTo(typeof(ICommandHandler<>))
                .Where(t => !t.Name.Contains("Decorator")))
            .AsImplementedInterfaces()
            .WithScopedLifetime());

        services.Scan(s => s.FromAssemblies(applicationAssembly)
            .AddClasses(c => c.AssignableTo(typeof(ICommandHandler<,>))
                .Where(t => !t.Name.Contains("Decorator")))
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

        // Auto-register all query handlers - EXCLUDING decorators and Simple handlers to avoid conflicts
        services.Scan(s => s.FromAssemblies(applicationAssembly)
            .AddClasses(c => c.AssignableTo(typeof(IQueryHandler<,>))
                .Where(t => !t.Name.Contains("Decorator") && !t.Name.Contains("Simple")))
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
        services.AddScoped<IProgressCalculationService, ProgressCalculationService>();
        
        // Add validation
        services.AddValidation();
        
        // Register FluentValidation validators
        services.AddValidatorsFromAssembly(applicationAssembly);
        
        // SKIP DECORATORS FOR NOW - they're causing circular dependencies
        // services.AddDecorators();
        
        // Add decorators for Result-based handlers
        services.AddResultDecorators();
        
        // Add message pipeline
        services.AddMessagePipeline();
        services.AddEnhancedMessagePipeline();

        // Register adapters for Result-based handlers to work with ICommandHandler interface
        services.AddResultToCommandHandlerAdapters();

        // Register our simple handlers explicitly AFTER everything else
        services.AddScoped<IQueryHandler<GetDashboardOverview, DashboardOverviewDto>, GetDashboardOverviewHandlerSimple>();
        services.AddScoped<IQueryHandler<GetUserMedicalShifts, IEnumerable<MedicalShiftDto>>, GetUserMedicalShiftsHandlerSimple>();

        // Explicitly register procedure realization handlers since they don't have Result adapters
        services.AddScoped<ICommandHandler<Commands.AddProcedureRealizationCommand>, Commands.Handlers.AddProcedureRealizationHandler>();
        services.AddScoped<ICommandHandler<Commands.UpdateProcedureRealizationCommand>, Commands.Handlers.UpdateProcedureRealizationHandler>();
        services.AddScoped<ICommandHandler<Commands.DeleteProcedureRealizationCommand>, Commands.Handlers.DeleteProcedureRealizationHandler>();

        return services;
    }
}
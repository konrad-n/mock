using Microsoft.Extensions.DependencyInjection;
using SledzSpecke.Application.Abstractions;
using SledzSpecke.Application.Validation;
using SledzSpecke.Application.Services;
using SledzSpecke.Application.DomainServices;
using SledzSpecke.Core.DomainServices;
using System.Reflection;
using FluentValidation;

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

        // Override specific handlers if needed (enhanced versions)
        // Commented out - using Result-based handlers now
        // services.AddScoped<ICommandHandler<Commands.SignUp>, Commands.Handlers.SignUpHandlerEnhanced>();
        // services.AddScoped<ICommandHandler<Commands.AddMedicalShift, int>, MedicalShifts.Handlers.AddMedicalShiftHandlerEnhanced>();
        // services.AddScoped<ICommandHandler<Commands.UpdateMedicalShift>, MedicalShifts.Handlers.UpdateMedicalShiftHandlerEnhanced>();
        // services.AddScoped<ICommandHandler<Commands.DeleteMedicalShift>, MedicalShifts.Handlers.DeleteMedicalShiftHandlerEnhanced>();
        
        // Register Result-based handlers (temporary override until all handlers are migrated)
        services.AddScoped<IResultCommandHandler<Commands.CreateCourse, int>, Commands.Handlers.CreateCourseHandler>();
        services.AddScoped<IResultCommandHandler<Commands.ChangePassword>, Commands.Handlers.ChangePasswordHandler>();
        services.AddScoped<IResultCommandHandler<Commands.UpdateUserProfile>, Commands.Handlers.UpdateUserProfileHandler>();
        services.AddScoped<IResultCommandHandler<Commands.DeleteCourse>, Commands.Handlers.DeleteCourseHandler>();
        services.AddScoped<IResultCommandHandler<Commands.UpdateCourse>, Commands.Handlers.UpdateCourseHandler>();

        return services;
    }
}
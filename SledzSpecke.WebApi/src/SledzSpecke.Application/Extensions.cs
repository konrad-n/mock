using Microsoft.Extensions.DependencyInjection;
using SledzSpecke.Application.Abstractions;
using SledzSpecke.Application.Validation;
using SledzSpecke.Application.Services;
using System.Reflection;

namespace SledzSpecke.Application;

public static partial class Extensions
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
        services.AddScoped<IProgressCalculationService, ProgressCalculationService>();
        
        // Add validation
        services.AddValidation();
        
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
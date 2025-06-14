using Microsoft.Extensions.DependencyInjection;
using SledzSpecke.Application.Abstractions;
using SledzSpecke.Application.Services;
using System.Reflection;

namespace SledzSpecke.Application;

public static class Extensions
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        var applicationAssembly = typeof(ICommandHandler<>).Assembly;

        // Auto-register all command handlers using Scrutor
        services.Scan(s => s.FromAssemblies(applicationAssembly)
            .AddClasses(c => c.AssignableTo(typeof(ICommandHandler<>)))
            .AsImplementedInterfaces()
            .WithScopedLifetime());

        services.Scan(s => s.FromAssemblies(applicationAssembly)
            .AddClasses(c => c.AssignableTo(typeof(ICommandHandler<,>)))
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

        // Override specific handlers if needed (enhanced versions)
        services.AddScoped<ICommandHandler<Commands.SignUp>, Commands.Handlers.SignUpHandlerEnhanced>();
        services.AddScoped<ICommandHandler<Commands.AddMedicalShift, int>, MedicalShifts.Handlers.AddMedicalShiftHandlerEnhanced>();
        services.AddScoped<ICommandHandler<Commands.UpdateMedicalShift>, MedicalShifts.Handlers.UpdateMedicalShiftHandlerEnhanced>();
        services.AddScoped<ICommandHandler<Commands.DeleteMedicalShift>, MedicalShifts.Handlers.DeleteMedicalShiftHandlerEnhanced>();

        return services;
    }
}
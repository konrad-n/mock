using Microsoft.Extensions.DependencyInjection;
using SledzSpecke.Application.Features.MedicalShifts.Commands.AddMedicalShift;
using SledzSpecke.Application.Features.MedicalShifts.Commands.UpdateMedicalShift;
using SledzSpecke.Application.Features.MedicalShifts.Commands.DeleteMedicalShift;
using SledzSpecke.Core.DomainServices;

namespace SledzSpecke.Application.Features.MedicalShifts;

public static class MedicalShiftExtensions
{
    public static IServiceCollection AddMedicalShiftsFeature(this IServiceCollection services)
    {
        // Register commands
        services.Scan(scan => scan
            .FromAssemblyOf<AddMedicalShift>()
            .AddClasses(classes => classes.InNamespace("SledzSpecke.Application.Features.MedicalShifts.Commands"))
            .AsImplementedInterfaces()
            .WithScopedLifetime());

        // Register queries
        services.Scan(scan => scan
            .FromAssemblyOf<AddMedicalShift>()
            .AddClasses(classes => classes.InNamespace("SledzSpecke.Application.Features.MedicalShifts.Queries"))
            .AsImplementedInterfaces()
            .WithScopedLifetime());

        // Domain services are already registered in Core, but we ensure they're available
        services.AddScoped<IMedicalShiftValidationService, MedicalShiftValidationService>();
        services.AddScoped<IDurationCalculationService, DurationCalculationService>();

        return services;
    }
}
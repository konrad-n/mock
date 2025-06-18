using Microsoft.Extensions.DependencyInjection;
using SledzSpecke.Application.Features.MedicalShifts.Commands.AddMedicalShift;
using SledzSpecke.Application.Features.MedicalShifts.Commands.UpdateMedicalShift;
using SledzSpecke.Application.Features.MedicalShifts.Commands.DeleteMedicalShift;
using SledzSpecke.Core.DomainServices;
using Scrutor;

namespace SledzSpecke.Application.Features.MedicalShifts;

public static class MedicalShiftExtensions
{
    public static IServiceCollection AddMedicalShiftsFeature(this IServiceCollection services)
    {
        // Register command handlers
        services.AddScoped<AddMedicalShiftHandler>();
        services.AddScoped<UpdateMedicalShiftHandler>();
        services.AddScoped<DeleteMedicalShiftHandler>();
        
        // Register query handlers individually
        services.Scan(scan => scan
            .FromAssemblyOf<AddMedicalShift>()
            .AddClasses(classes => classes
                .Where(c => c.Namespace != null && c.Namespace.StartsWith("SledzSpecke.Application.Features.MedicalShifts.Queries"))
                .Where(c => c.Name.EndsWith("Handler")))
            .AsImplementedInterfaces()
            .WithScopedLifetime());

        // Domain services are already registered in Core, but we ensure they're available
        services.AddScoped<IMedicalShiftValidationService, MedicalShiftValidationService>();
        services.AddScoped<IDurationCalculationService, DurationCalculationService>();

        return services;
    }
}
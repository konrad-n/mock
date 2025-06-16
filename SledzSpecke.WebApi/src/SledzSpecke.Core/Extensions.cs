using Microsoft.Extensions.DependencyInjection;
using SledzSpecke.Core.DomainServices;
using SledzSpecke.Core.Policies;

namespace SledzSpecke.Core;

public static class Extensions
{
    public static IServiceCollection AddCore(this IServiceCollection services)
    {
        // Register policies
        services.AddSingleton<ISmkPolicyFactory, SmkPolicyFactory>();
        
        // Register domain services
        services.AddScoped<IMedicalShiftValidationService, MedicalShiftValidationService>();
        services.AddScoped<IProcedureValidationService, ProcedureValidationService>();
        services.AddScoped<IModuleProgressionService, ModuleProgressionService>();
        services.AddScoped<IDurationCalculationService, DurationCalculationService>();
        services.AddScoped<ICmkpValidationService, CmkpValidationService>();
        services.AddScoped<ISmkComplianceValidator, SmkComplianceValidator>();
        
        return services;
    }
}
using Microsoft.Extensions.DependencyInjection;
using SledzSpecke.Application.Abstractions;
using SledzSpecke.Application.Adapters;
using SledzSpecke.Application.Features.MedicalShifts.Commands.AddMedicalShift;
using SledzSpecke.Application.Features.MedicalShifts.Commands.UpdateMedicalShift;
using SledzSpecke.Application.Features.MedicalShifts.Commands.DeleteMedicalShift;
using SledzSpecke.Application.Commands;

namespace SledzSpecke.Application;

public static partial class ApplicationExtensions
{
    /// <summary>
    /// Registers adapters to bridge Result-based handlers with traditional ICommandHandler interface
    /// </summary>
    public static IServiceCollection AddResultToCommandHandlerAdapters(this IServiceCollection services)
    {
        // Medical Shifts - AddMedicalShift returns int
        services.AddScoped<ICommandHandler<AddMedicalShift, int>>(sp =>
        {
            var resultHandler = sp.GetRequiredService<IResultCommandHandler<AddMedicalShift, int>>();
            return new ResultToCommandHandlerAdapter<AddMedicalShift, int>(resultHandler);
        });

        // Medical Shifts - UpdateMedicalShift returns void
        services.AddScoped<ICommandHandler<UpdateMedicalShift>>(sp =>
        {
            var resultHandler = sp.GetRequiredService<IResultCommandHandler<UpdateMedicalShift>>();
            return new ResultToCommandHandlerAdapter<UpdateMedicalShift>(resultHandler);
        });

        // Medical Shifts - DeleteMedicalShift returns void
        services.AddScoped<ICommandHandler<DeleteMedicalShift>>(sp =>
        {
            var resultHandler = sp.GetRequiredService<IResultCommandHandler<DeleteMedicalShift>>();
            return new ResultToCommandHandlerAdapter<DeleteMedicalShift>(resultHandler);
        });

        // Procedures
        services.AddScoped<ICommandHandler<AddProcedure, int>>(sp =>
        {
            var resultHandler = sp.GetRequiredService<IResultCommandHandler<AddProcedure, int>>();
            return new ResultToCommandHandlerAdapter<AddProcedure, int>(resultHandler);
        });

        services.AddScoped<ICommandHandler<UpdateProcedure>>(sp =>
        {
            var resultHandler = sp.GetRequiredService<IResultCommandHandler<UpdateProcedure>>();
            return new ResultToCommandHandlerAdapter<UpdateProcedure>(resultHandler);
        });

        services.AddScoped<ICommandHandler<DeleteProcedure>>(sp =>
        {
            var resultHandler = sp.GetRequiredService<IResultCommandHandler<DeleteProcedure>>();
            return new ResultToCommandHandlerAdapter<DeleteProcedure>(resultHandler);
        });

        // User Profile
        services.AddScoped<ICommandHandler<UpdateUserProfile>>(sp =>
        {
            var resultHandler = sp.GetRequiredService<IResultCommandHandler<UpdateUserProfile>>();
            return new ResultToCommandHandlerAdapter<UpdateUserProfile>(resultHandler);
        });

        // Add more adapters as needed for other commands...

        return services;
    }
}
using Microsoft.Extensions.DependencyInjection;
using SledzSpecke.Application.Abstractions;
using SledzSpecke.Application.Commands.Handlers;
using SledzSpecke.Application.Queries.Handlers;
using SledzSpecke.Application.Procedures.Handlers;
using SledzSpecke.Application.MedicalShifts.Handlers;
using SledzSpecke.Application.Services;
using System.Reflection;

namespace SledzSpecke.Application;

public static class Extensions
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        var assembly = Assembly.GetExecutingAssembly();
        
        // Register command handlers
        services.AddScoped<ICommandHandler<Commands.SignUp>, SignUpHandler>();
        services.AddScoped<ICommandHandler<Commands.SignIn, DTO.JwtDto>, SignInHandler>();
        services.AddScoped<ICommandHandler<Commands.CreateInternship, int>, CreateInternshipHandler>();
        services.AddScoped<ICommandHandler<Commands.UpdateInternship>, UpdateInternshipHandler>();
        services.AddScoped<ICommandHandler<Commands.ApproveInternship>, ApproveInternshipHandler>();
        services.AddScoped<ICommandHandler<Commands.MarkInternshipAsCompleted>, MarkInternshipAsCompletedHandler>();
        services.AddScoped<ICommandHandler<Commands.CreateAbsence>, CreateAbsenceHandler>();
        services.AddScoped<ICommandHandler<Commands.CreateRecognition>, CreateRecognitionHandler>();
        services.AddScoped<ICommandHandler<Commands.CreatePublication>, CreatePublicationHandler>();
        services.AddScoped<ICommandHandler<Commands.CreateSelfEducation>, CreateSelfEducationHandler>();
        services.AddScoped<ICommandHandler<Commands.AddProcedure, int>, AddProcedureHandler>();
        services.AddScoped<ICommandHandler<Commands.UpdateProcedure>, UpdateProcedureHandler>();
        services.AddScoped<ICommandHandler<Commands.DeleteProcedure>, DeleteProcedureHandler>();
        services.AddScoped<ICommandHandler<Commands.AddMedicalShift, int>, AddMedicalShiftHandler>();
        services.AddScoped<ICommandHandler<Commands.UpdateMedicalShift>, UpdateMedicalShiftHandler>();
        services.AddScoped<ICommandHandler<Commands.DeleteMedicalShift>, DeleteMedicalShiftHandler>();

        // Register query handlers
        services.AddScoped<IQueryHandler<Queries.GetUser, DTO.UserDto>, GetUserHandler>();
        services.AddScoped<IQueryHandler<Queries.GetUsers, IEnumerable<DTO.UserDto>>, GetUsersHandler>();
        services.AddScoped<IQueryHandler<Queries.GetUserPublications, IEnumerable<DTO.PublicationDto>>, GetUserPublicationsHandler>();
        services.AddScoped<IQueryHandler<Queries.GetUserAbsences, IEnumerable<DTO.AbsenceDto>>, GetUserAbsencesHandler>();
        services.AddScoped<IQueryHandler<Queries.GetUserRecognitions, IEnumerable<DTO.RecognitionDto>>, GetUserRecognitionsHandler>();
        services.AddScoped<IQueryHandler<Queries.GetUserSelfEducation, IEnumerable<DTO.SelfEducationDto>>, GetUserSelfEducationHandler>();
        services.AddScoped<IQueryHandler<Queries.GetInternships, IEnumerable<DTO.InternshipDto>>, GetInternshipsHandler>();
        services.AddScoped<IQueryHandler<Queries.GetUserProcedures, IEnumerable<DTO.ProcedureDto>>, GetUserProceduresHandler>();
        services.AddScoped<IQueryHandler<Queries.GetProcedureById, DTO.ProcedureDto>, GetProcedureByIdHandler>();
        services.AddScoped<IQueryHandler<Queries.GetUserMedicalShifts, IEnumerable<DTO.MedicalShiftDto>>, GetUserMedicalShiftsHandler>();
        services.AddScoped<IQueryHandler<Queries.GetMedicalShiftById, DTO.MedicalShiftDto>, GetMedicalShiftByIdHandler>();

        // Register validation services
        services.AddScoped<ISpecializationValidationService, SpecializationValidationService>();
        services.AddScoped<IYearCalculationService, YearCalculationService>();

        return services;
    }
}
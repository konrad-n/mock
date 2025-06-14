using Microsoft.Extensions.DependencyInjection;
using SledzSpecke.Application.Abstractions;
using SledzSpecke.Application.Commands;
using SledzSpecke.Application.Decorators;
using SledzSpecke.Application.Validation;
using SledzSpecke.Application.Validation.Validators;

namespace SledzSpecke.Application.Validation;

public static class ValidationExtensions
{
    /// <summary>
    /// Registers all validators and sets up validation pipeline
    /// </summary>
    public static IServiceCollection AddValidation(this IServiceCollection services)
    {
        // Register validators
        services.AddScoped<IValidator<CreateCourse>, CreateCourseValidator>();
        services.AddScoped<IValidator<ChangePassword>, ChangePasswordValidator>();
        services.AddScoped<IValidator<UpdateUserProfile>, UpdateUserProfileValidator>();
        services.AddScoped<IValidator<SignUp>, SignUpValidator>();
        services.AddScoped<IValidator<SignIn>, SignInValidator>();
        services.AddScoped<IValidator<CreateInternship>, CreateInternshipValidator>();
        services.AddScoped<IValidator<UpdateInternship>, UpdateInternshipValidator>();
        services.AddScoped<IValidator<DeleteInternship>, DeleteInternshipValidator>();
        services.AddScoped<IValidator<ApproveInternship>, ApproveInternshipValidator>();
        services.AddScoped<IValidator<MarkInternshipAsCompleted>, MarkInternshipAsCompletedValidator>();
        services.AddScoped<IValidator<AddMedicalShift>, AddMedicalShiftValidator>();
        services.AddScoped<IValidator<UpdateMedicalShift>, UpdateMedicalShiftValidator>();
        services.AddScoped<IValidator<DeleteMedicalShift>, DeleteMedicalShiftValidator>();
        services.AddScoped<IValidator<UploadFile>, UploadFileValidator>();
        services.AddScoped<IValidator<AddProcedure>, AddProcedureValidator>();
        services.AddScoped<IValidator<UpdateProcedure>, UpdateProcedureValidator>();
        services.AddScoped<IValidator<DeleteProcedure>, DeleteProcedureValidator>();
        services.AddScoped<IValidator<CreatePublication>, CreatePublicationValidator>();
        services.AddScoped<IValidator<UpdatePublication>, UpdatePublicationValidator>();
        services.AddScoped<IValidator<DeletePublication>, DeletePublicationValidator>();
        
        // TODO: Add more validators as they are created
        // services.AddScoped<IValidator<UpdateCourse>, UpdateCourseValidator>();
        // services.AddScoped<IValidator<DeleteCourse>, DeleteCourseValidator>();
        
        return services;
    }

    /// <summary>
    /// Decorates command handlers with validation
    /// </summary>
    public static IServiceCollection AddValidationDecorators(this IServiceCollection services)
    {
        // This would need to be called after registering handlers
        // and would wrap each handler with validation decorator
        
        // Example of manual decoration (in real app, use Scrutor or similar):
        // services.Decorate<IResultCommandHandler<CreateCourse, int>>((handler, provider) =>
        //     new EnhancedValidationDecorator<CreateCourse, int>(handler, provider));
        
        return services;
    }
}
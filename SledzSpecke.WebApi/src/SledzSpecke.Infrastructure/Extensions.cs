using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using SledzSpecke.Application.Abstractions;
using SledzSpecke.Application.Security;
using SledzSpecke.Core.Abstractions;
using SledzSpecke.Core.Repositories;
using SledzSpecke.Infrastructure.Auth;
using SledzSpecke.Infrastructure.DAL;
using SledzSpecke.Infrastructure.DAL.Repositories;
using SledzSpecke.Infrastructure.DAL.Seeding;
using SledzSpecke.Infrastructure.Security;
using SledzSpecke.Infrastructure.Services;
using SledzSpecke.Infrastructure.Time;
using System.Text;

namespace SledzSpecke.Infrastructure;

public static class Extensions
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddSingleton<IClock, Clock>();
        services.AddSingleton<IPasswordManager, PasswordManager>();
        services.AddSingleton<IAuthenticator, Authenticator>();
        services.AddScoped<IUserContextService, UserContextService>();
        
        // Template services
        services.AddSingleton<ISpecializationTemplateService, SpecializationTemplateService>();

        services.AddScoped<IUserRepository, SqlUserRepository>();
        services.AddScoped<ISpecializationRepository, SqlSpecializationRepository>();
        services.AddScoped<IModuleRepository, SqlModuleRepository>();
        services.AddScoped<IInternshipRepository, SqlInternshipRepository>();
        services.AddScoped<IProcedureRepository, SqlProcedureRepository>();
        services.AddScoped<IMedicalShiftRepository, SqlMedicalShiftRepository>();
        services.AddScoped<ICourseRepository, SqlCourseRepository>();
        services.AddScoped<IAbsenceRepository, SqlAbsenceRepository>();
        services.AddScoped<IRecognitionRepository, SqlRecognitionRepository>();
        services.AddScoped<IPublicationRepository, SqlPublicationRepository>();
        services.AddScoped<ISelfEducationRepository, SqlSelfEducationRepository>();
        services.AddScoped<IDataSeeder, DataSeeder>();

        services.AddDbContext<SledzSpeckeDbContext>(options =>
            options.UseNpgsql(configuration.GetConnectionString("DefaultConnection")));

        var authOptions = configuration.GetSection("auth").Get<AuthOptions>();
        services.Configure<AuthOptions>(configuration.GetSection("auth"));

        services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            .AddJwtBearer(options =>
            {
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    ValidIssuer = authOptions?.Issuer,
                    ValidAudience = authOptions?.Audience,
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(authOptions?.SigningKey ?? string.Empty))
                };
            });

        services.AddAuthorization();

        return services;
    }
}
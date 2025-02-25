using Microsoft.Extensions.Logging;
using SledzSpecke.App.Services.Platform;
using SledzSpecke.App.ViewModels.Base;
using SledzSpecke.App.ViewModels.Courses;
using SledzSpecke.App.ViewModels.Dashboard;
using SledzSpecke.App.ViewModels.Duties;
using SledzSpecke.App.ViewModels.Internships;
using SledzSpecke.App.ViewModels.Procedures;
using SledzSpecke.App.ViewModels.Profile;
using SledzSpecke.App.Views.Courses;
using SledzSpecke.App.Views.Dashboard;
using SledzSpecke.App.Views.Duties;
using SledzSpecke.App.Views.Internships;
using SledzSpecke.App.Views.Procedures;
using SledzSpecke.App.Views.Profile;
using SledzSpecke.Core.Interfaces.Services;
using SledzSpecke.Infrastructure.Database.Configuration;
using SledzSpecke.Infrastructure.Database.Context;
using SledzSpecke.Infrastructure.Database.Initialization;
using SledzSpecke.Infrastructure.Database.Migrations;
using SledzSpecke.Infrastructure.Database.Repositories;
using SledzSpecke.Infrastructure.Services;

namespace SledzSpecke.App;

public static class MauiProgram
{
    public static MauiApp CreateMauiApp()
    {
        var builder = MauiApp.CreateBuilder();
        builder
            .UseMauiApp<App>()
            .ConfigureFonts(fonts =>
            {
                fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
                fonts.AddFont("FluentUI.ttf", "FluentUI");
            });

        // Rejestracja serwisów platformowych
        builder.Services.AddSingleton<IPermissionService, PermissionService>();
        builder.Services.AddSingleton<IFileSystemService, FileSystemService>();
        
        // Rejestracja serwisów biznesowych
        builder.Services.AddSingleton<IProcedureService, ProcedureService>();
        builder.Services.AddSingleton<IDutyService, DutyService>();
        builder.Services.AddSingleton<ICourseService, CourseService>();
        builder.Services.AddSingleton<IInternshipService, InternshipService>();
        builder.Services.AddSingleton<IUserService, UserService>();

        // Rejestracja ViewModels
        // Dashboard
        builder.Services.AddTransient<DashboardViewModel>();
        
        // Procedury
        builder.Services.AddTransient<ProceduresViewModel>();
        builder.Services.AddTransient<ProcedureAddViewModel>();
        builder.Services.AddTransient<ProcedureEditViewModel>();
        
        // Dyżury
        builder.Services.AddTransient<DutiesViewModel>();
        builder.Services.AddTransient<DutyAddViewModel>();
        builder.Services.AddTransient<DutyEditViewModel>();
        
        // Kursy
        builder.Services.AddTransient<CoursesViewModel>();
        builder.Services.AddTransient<CourseAddViewModel>();
        builder.Services.AddTransient<CourseEditViewModel>();
        
        // Staże
        builder.Services.AddTransient<InternshipsViewModel>();
        builder.Services.AddTransient<InternshipAddViewModel>();
        builder.Services.AddTransient<InternshipEditViewModel>();
        
        // Profil i ustawienia
        builder.Services.AddTransient<ProfileViewModel>();
        builder.Services.AddTransient<SettingsViewModel>();

        // Rejestracja Views
        // Dashboard
        builder.Services.AddTransient<DashboardPage>();
        
        // Procedury
        builder.Services.AddTransient<ProceduresPage>();
        builder.Services.AddTransient<ProcedureAddPage>();
        builder.Services.AddTransient<ProcedureEditPage>();
        
        // Dyżury
        builder.Services.AddTransient<DutiesPage>();
        builder.Services.AddTransient<DutyAddPage>();
        builder.Services.AddTransient<DutyEditPage>();
        
        // Kursy
        builder.Services.AddTransient<CoursesPage>();
        builder.Services.AddTransient<CourseAddPage>();
        builder.Services.AddTransient<CourseEditPage>();
        builder.Services.AddTransient<CourseDetailsPage>();
        
        // Staże
        builder.Services.AddTransient<InternshipsPage>();
        builder.Services.AddTransient<InternshipAddPage>();
        builder.Services.AddTransient<InternshipEditPage>();
        builder.Services.AddTransient<InternshipDetailsPage>();
        
        // Profil i ustawienia
        builder.Services.AddTransient<ProfilePage>();
        builder.Services.AddTransient<SettingsPage>();

        // Repositoria
        builder.Services.AddScoped<IUserRepository, UserRepository>();
        builder.Services.AddScoped<ISpecializationRepository, SpecializationRepository>();
        builder.Services.AddScoped<IProcedureRepository, ProcedureRepository>();
        builder.Services.AddScoped<IDutyRepository, DutyRepository>();
        builder.Services.AddScoped<ICourseRepository, CourseRepository>();
        builder.Services.AddScoped<IInternshipRepository, InternshipRepository>();

        // Baza danych
        builder.Services.AddSingleton<IMigrationRunner, MigrationRunner>();
        builder.Services.AddSingleton<IApplicationDbContext>(provider =>
        {
            var migrationRunner = provider.GetRequiredService<IMigrationRunner>();
            var fileSystemService = provider.GetRequiredService<IFileSystemService>();
            return new ApplicationDbContext(
                DatabaseConfig.GetDatabasePath(fileSystemService.GetAppDataDirectory()),
                migrationRunner);
        });
        
        // Inicjalizacja i seed danych
        builder.Services.AddSingleton<DatabaseInitializer>();

#if DEBUG
        builder.Logging.AddDebug();
#endif

        return builder.Build();
    }
}

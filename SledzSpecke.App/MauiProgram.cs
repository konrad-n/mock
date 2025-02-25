using Microsoft.Extensions.Logging;
using SledzSpecke.App.Views.Dashboard;
using SledzSpecke.App.Views.Procedures;
using SledzSpecke.App.Views.Duties;
using SledzSpecke.App.ViewModels.Dashboard;
using SledzSpecke.App.ViewModels.Procedures;
using SledzSpecke.App.ViewModels.Duties;
using SledzSpecke.App.Views.Profile;
using SledzSpecke.Core.Interfaces.Services;
using SledzSpecke.Infrastructure.Database.Configuration;
using SledzSpecke.Infrastructure.Database.Context;
using SledzSpecke.Infrastructure.Database.Repositories;
using SledzSpecke.Infrastructure.Database.Migrations;
using SledzSpecke.Infrastructure.Services;
using SledzSpecke.App.Services.Platform;
using SledzSpecke.Core.Services.SpecializationSync;
using SledzSpecke.Core.Services.Notifications;
using SledzSpecke.App.Services.Export;
using SledzSpecke.Core.Services.SMK;
using SledzSpecke.App.Views.MultipleSpecialization;
using SledzSpecke.App.ViewModels.MultipleSpecialization;
using SledzSpecke.App.Views.Statistics;
using SledzSpecke.App.ViewModels.Statistics;
using SledzSpecke.App.Services.Accessibility;
using SledzSpecke.App.Controls;

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

        // Rejestracja serwisów platformy
        builder.Services.AddSingleton<IPermissionService, PermissionService>();
        builder.Services.AddSingleton<IFileSystemService, FileSystemService>();

        // Rejestracja serwisów biznesowych
        builder.Services.AddSingleton<IProcedureService, ProcedureService>();
        builder.Services.AddSingleton<IDutyService, DutyService>();
        builder.Services.AddSingleton<ICourseService, CourseService>();
        builder.Services.AddSingleton<IInternshipService, InternshipService>();
        builder.Services.AddSingleton<IUserService, UserService>();
        
        // Nowe serwisy specjalizacji
        builder.Services.AddSingleton<ISpecializationService, SpecializationService>();
        builder.Services.AddSingleton<ISpecializationSyncService, SledzSpecke.Infrastructure.Services.SpecializationSyncService>();
        builder.Services.AddSingleton<INotificationService, SledzSpecke.Infrastructure.Services.NotificationService>();
        builder.Services.AddSingleton<IPdfExportService, PdfExportService>();
        
        // Serwis SMK
        builder.Services.AddSingleton<ISMKIntegrationService, SMKIntegrationService>();
        builder.Services.AddSingleton<SMKConfiguration>(provider => new SMKConfiguration 
        { 
            BaseUrl = "https://api.smk.gov.pl", 
            ApiKey = "demo_key" 
        });
        
        // Serwisy dostępności i integracji z kalendarzem
        builder.Services.AddSingleton<IAccessibilityService, AccessibilityService>();
        builder.Services.AddSingleton<ICalendarService, DeviceCalendarService>();
        builder.Services.AddSingleton<CalendarIntegration>();

        // Rejestracja ViewModels
        builder.Services.AddTransient<DashboardViewModel>();
        builder.Services.AddTransient<ProceduresViewModel>();
        builder.Services.AddTransient<DutiesViewModel>();
        
        // Procedury
        builder.Services.AddTransient<ProcedureAddViewModel>();
        builder.Services.AddTransient<ProcedureEditViewModel>();
        
        // Dyżury
        builder.Services.AddTransient<DutyAddViewModel>();
        builder.Services.AddTransient<DutyEditViewModel>();
        
        // Profil i Ustawienia
        builder.Services.AddTransient<ProfileViewModel>();
        builder.Services.AddTransient<SettingsViewModel>();
        
        // Specjalizacje
        builder.Services.AddTransient<SpecializationSwitcherViewModel>();
        builder.Services.AddTransient<SpecializationStatsViewModel>();

        // Rejestracja stron
        builder.Services.AddTransient<DashboardPage>();
        builder.Services.AddTransient<ProceduresPage>();
        builder.Services.AddTransient<DutiesPage>();
        
        // Procedury
        builder.Services.AddTransient<ProcedureAddPage>();
        builder.Services.AddTransient<ProcedureEditPage>();
        
        // Dyżury
        builder.Services.AddTransient<DutyAddPage>();
        builder.Services.AddTransient<DutyEditPage>();
        
        // Ustawienia i Profil
        builder.Services.AddTransient<SettingsPage>();
        builder.Services.AddTransient<ProfilePage>();
        
        // Specjalizacje
        builder.Services.AddTransient<SpecializationSwitcherPage>();
        builder.Services.AddTransient<SpecializationStatsPage>();

        // Repositoria
        builder.Services.AddScoped<IUserRepository, UserRepository>();
        builder.Services.AddScoped<ISpecializationRepository, SpecializationRepository>();
        builder.Services.AddScoped<IProcedureRepository, ProcedureRepository>();
        builder.Services.AddScoped<IDutyRepository, DutyRepository>();
        builder.Services.AddScoped<ICourseRepository, CourseRepository>();
        builder.Services.AddScoped<IInternshipRepository, InternshipRepository>();
        builder.Services.AddScoped<INotificationRepository, NotificationRepository>();

        // Inicjalizacja bazy danych
        builder.Services.AddSingleton<IMigrationRunner, MigrationRunner>();
        builder.Services.AddSingleton<IApplicationDbContext>(provider =>
        {
            var migrationRunner = provider.GetRequiredService<IMigrationRunner>();
            var fileSystemService = provider.GetRequiredService<IFileSystemService>();
            return new ApplicationDbContext(
                DatabaseConfig.GetDatabasePath(fileSystemService.GetAppDataDirectory()),
                migrationRunner);
        });

#if DEBUG
        builder.Logging.AddDebug();
#endif

        return builder.Build();
    }
}

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
using SledzSpecke.App.Services.Export;
using SledzSpecke.Core.Services.SMK;
using SledzSpecke.App.Views.MultipleSpecialization;
using SledzSpecke.App.Views.Statistics;
using SledzSpecke.App.ViewModels.Statistics;
using SledzSpecke.App.Controls;
using SledzSpecke.App.ViewModels.Profile;
using SledzSpecke.App.Services;

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

        // Register platform services
        builder.Services.AddSingleton<IPermissionService, PermissionService>();
        builder.Services.AddSingleton<IFileSystemService, FileSystemService>();
        builder.Services.AddSingleton<ISettingsService, SettingsService>();
        builder.Services.AddSingleton<IDataSyncService, DataSyncService>();

        // Register business services
        builder.Services.AddSingleton<IProcedureService, ProcedureService>();
        builder.Services.AddSingleton<IDutyService, DutyService>();
        builder.Services.AddSingleton<ICourseService, CourseService>();
        builder.Services.AddSingleton<IInternshipService, InternshipService>();
        builder.Services.AddSingleton<IUserService, UserService>();

        // New specialization services
        builder.Services.AddSingleton<ISpecializationService, SpecializationService>();
        builder.Services.AddSingleton<ISpecializationSyncService, SpecializationSyncService>();
        builder.Services.AddSingleton<INotificationService, NotificationService>();
        builder.Services.AddSingleton<IPdfExportService, PdfExportService>();

        // SMK Service
        builder.Services.AddSingleton<ISMKIntegrationService, SMKIntegrationService>();
        builder.Services.AddSingleton<SMKConfiguration>(provider => new SMKConfiguration
        {
            BaseUrl = "https://api.smk.gov.pl",
            ApiKey = "demo_key"
        });

        // Calendar integration
        builder.Services.AddSingleton<CalendarIntegration>();

        // Register ViewModels
        builder.Services.AddTransient<DashboardViewModel>();
        builder.Services.AddTransient<ProceduresViewModel>();
        builder.Services.AddTransient<DutiesViewModel>();

        // Procedures
        builder.Services.AddTransient<ProcedureAddViewModel>();
        builder.Services.AddTransient<ProcedureEditViewModel>();

        // Duties
        builder.Services.AddTransient<DutyAddViewModel>();
        builder.Services.AddTransient<DutyEditViewModel>();

        // Profile and Settings
        builder.Services.AddTransient<ProfileViewModel>();
        builder.Services.AddTransient<SettingsViewModel>();

        // Specializations
        builder.Services.AddTransient<SpecializationStatsViewModel>();

        // Register pages
        builder.Services.AddTransient<DashboardPage>();
        builder.Services.AddTransient<ProceduresPage>();
        builder.Services.AddTransient<DutiesPage>();

        // Procedures
        builder.Services.AddTransient<ProcedureAddPage>();
        builder.Services.AddTransient<ProcedureEditPage>();

        // Duties
        builder.Services.AddTransient<DutyAddPage>();
        builder.Services.AddTransient<DutyEditPage>();

        // Settings and Profile
        builder.Services.AddTransient<SettingsPage>();
        builder.Services.AddTransient<ProfilePage>();

        // Specializations
        builder.Services.AddTransient<SpecializationSwitcherPage>();
        builder.Services.AddTransient<SpecializationStatsPage>();

        // Repositories
        builder.Services.AddScoped<IUserRepository, UserRepository>();
        builder.Services.AddScoped<ISpecializationRepository, SpecializationRepository>();
        builder.Services.AddScoped<IProcedureRepository, ProcedureRepository>();
        builder.Services.AddScoped<IDutyRepository, DutyRepository>();
        builder.Services.AddScoped<ICourseRepository, CourseRepository>();
        builder.Services.AddScoped<IInternshipRepository, InternshipRepository>();
        builder.Services.AddScoped<INotificationRepository, NotificationRepository>();

        // Database initialization
        builder.Services.AddSingleton<IMigrationRunner, MigrationRunner>();
        builder.Services.AddSingleton<IApplicationDbContext>(provider =>
        {
            var migrationRunner = provider.GetRequiredService<IMigrationRunner>();
            var fileSystemService = provider.GetRequiredService<IFileSystemService>();
            return new ApplicationDbContext(
                DatabaseConfig.GetDatabasePath(fileSystemService.GetAppDataDirectory()),
                migrationRunner);
        });

        // builder.Services.AddSingleton<IBiometricAuthService, BiometricAuthService>();
        // builder.Services.AddSingleton<IDocumentScanService, DocumentScanService>();

#if DEBUG
        builder.Logging.AddDebug();
#endif

        return builder.Build();
    }
}

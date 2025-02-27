using Microsoft.Extensions.Logging;
using SledzSpecke.App.Services;
using SledzSpecke.App.Views;
using SledzSpecke.Infrastructure.Database;
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

        // Register services
        RegisterServices(builder.Services);

        // Register pages
        RegisterPages(builder.Services);

#if DEBUG
        builder.Logging.AddDebug();
#endif

        var app = builder.Build();
        return app;
    }

    private static void RegisterServices(IServiceCollection services)
    {
        // Register file system service
        services.AddSingleton<IFileSystemService, MauiFileSystemService>();

        // Register database service
        services.AddSingleton<DatabaseService>();

        // Register application services
        services.AddSingleton<DataManager>();
        services.AddSingleton<SpecializationService>();
        services.AddSingleton<DutyShiftService>();
        services.AddSingleton<SelfEducationService>();
        services.AddSingleton<ExportService>();
        services.AddSingleton<NotificationService>();
        services.AddSingleton<AppSettings>();
    }

    private static void RegisterPages(IServiceCollection services)
    {
        // Register main pages
        services.AddTransient<DashboardPage>();
        services.AddTransient<SettingsPage>();

        // Register feature pages
        services.AddTransient<CoursesPage>();
        services.AddTransient<CourseDetailsPage>();
        services.AddTransient<InternshipsPage>();
        services.AddTransient<InternshipDetailsPage>();
        services.AddTransient<ProceduresPage>();
        services.AddTransient<ProcedureDetailsPage>();
        services.AddTransient<ProcedureEntryPage>();
        services.AddTransient<DutyShiftsPage>();
        services.AddTransient<DutyShiftDetailsPage>();
        services.AddTransient<SelfEducationPage>();
        services.AddTransient<SelfEducationDetailsPage>();
        services.AddTransient<SMKExportPage>();
    }
}
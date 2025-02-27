using Microsoft.Extensions.Logging;
using SledzSpecke.App.Views;

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

        RegisterPages(builder.Services);

#if DEBUG
        builder.Logging.AddDebug();
#endif

        var app = builder.Build();
        return app;
    }

    private static void RegisterPages(IServiceCollection services)
    {
        services.AddTransient<CourseDetailsPage>();
        services.AddTransient<CoursesPage>();
        services.AddTransient<DashboardPage>();
        services.AddTransient<DutyShiftDetailsPage>();
        services.AddTransient<DutyShiftsPage>();
        services.AddTransient<InternshipsPage>();
        services.AddTransient<ProcedureDetailsPage>();
        services.AddTransient<ProcedureEntryPage>();
        services.AddTransient<SelfEducationDetailsPage>();
        services.AddTransient<SelfEducationPage>();
        services.AddTransient<SettingsPage>();
        services.AddTransient<SMKExportPage>();
    }
}

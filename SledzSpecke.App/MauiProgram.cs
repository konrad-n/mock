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

        RegisterDatabaseServices(builder.Services);
        RegisterServices(builder.Services);
        RegisterViewModels(builder.Services);
        RegisterPages(builder.Services);

#if DEBUG
        builder.Logging.AddDebug();
#endif

        var app = builder.Build();
        return app;
    }

    private static void RegisterDatabaseServices(IServiceCollection services)
    {
    }

    private static void RegisterServices(IServiceCollection services)
    {
    }

    private static void RegisterViewModels(IServiceCollection services)
    {
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

public class ErrorApp : Application
{
    public ErrorApp()
    {
        MainPage = new ContentPage
        {
            BackgroundColor = Colors.Red,
            Content = new Label
            {
                Text = "App failed to start - check debug output",
                TextColor = Colors.White,
                HorizontalOptions = LayoutOptions.Center,
                VerticalOptions = LayoutOptions.Center
            }
        };
    }
}

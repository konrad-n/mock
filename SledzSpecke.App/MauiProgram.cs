using Microsoft.Extensions.Logging;
using SledzSpecke.App.Services.Platform;
using SledzSpecke.App.Services.Stubs;
using SledzSpecke.App.Views.Dashboard;
using SledzSpecke.App.ViewModels.Dashboard;
using SledzSpecke.App.Views.Procedures;
using SledzSpecke.App.Views.Duties;
using SledzSpecke.App.ViewModels.Procedures;
using SledzSpecke.App.ViewModels.Duties;
using SledzSpecke.Core.Interfaces.Services;

namespace SledzSpecke.App;

public static class MauiProgram
{
    public static MauiApp CreateMauiApp()
    {
        try
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

            // Register stub services
            builder.Services.AddSingleton<IProcedureService, StubProcedureService>();
            builder.Services.AddSingleton<ICourseService, StubCourseService>();
            builder.Services.AddSingleton<IInternshipService, StubInternshipService>();
            builder.Services.AddSingleton<IDutyService, StubDutyService>();
            builder.Services.AddSingleton<IUserService, StubUserService>();

            // Register ViewModels
            builder.Services.AddTransient<DashboardViewModel>();
            builder.Services.AddTransient<ProceduresViewModel>();
            builder.Services.AddTransient<DutiesViewModel>();

            // Register Pages
            builder.Services.AddTransient<DashboardPage>();
            builder.Services.AddTransient<ProceduresPage>();
            builder.Services.AddTransient<DutiesPage>();

#if DEBUG
            builder.Logging.AddDebug();
#endif

            return builder.Build();
        }
        catch (Exception ex)
        {
            // Log the exception
            Console.WriteLine($"ERROR IN STARTUP: {ex.Message}");
            Console.WriteLine(ex.StackTrace);

            // Return a minimal app that can display an error
            var builder = MauiApp.CreateBuilder();
            builder.UseMauiApp<ErrorApp>();
            return builder.Build();
        }
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

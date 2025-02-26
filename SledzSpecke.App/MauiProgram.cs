using Microsoft.Extensions.Logging;
using SledzSpecke.App.Services.Platform;
using SledzSpecke.App.Services.Stubs;
using SledzSpecke.App.ViewModels.Courses;
using SledzSpecke.App.ViewModels.Dashboard;
using SledzSpecke.App.ViewModels.Duties;
using SledzSpecke.App.ViewModels.Internships;
using SledzSpecke.App.ViewModels.Procedures;
using SledzSpecke.App.ViewModels.Profile;
using SledzSpecke.App.ViewModels.Statistics;
using SledzSpecke.App.Views.Dashboard;
using SledzSpecke.App.Views.Duties;
using SledzSpecke.App.Views.Procedures;
using SledzSpecke.App.Views.Profile;
using SledzSpecke.App.Views.Statistics;
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
            builder.Services.AddSingleton<ISpecializationService, StubSpecializationService>();
            builder.Services.AddSingleton<ISettingsService, StubSettingsService>();
            builder.Services.AddSingleton<IDataSyncService, StubDataSyncService>();

            // Register ViewModels
            RegisterViewModels(builder.Services);

            // Register Pages
            RegisterPages(builder.Services);

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

    private static void RegisterViewModels(IServiceCollection services)
    {
        // Dashboard
        services.AddTransient<DashboardViewModel>();

        // Procedures
        services.AddTransient<ProceduresViewModel>();
        services.AddTransient<ProcedureAddViewModel>();
        services.AddTransient<ProcedureEditViewModel>();

        // Duties
        services.AddTransient<DutiesViewModel>();
        services.AddTransient<DutyAddViewModel>();
        services.AddTransient<DutyEditViewModel>();

        // Courses
        services.AddTransient<CoursesViewModel>();

        // Internships
        services.AddTransient<InternshipsViewModel>();

        // Profile
        services.AddTransient<ProfileViewModel>();
        services.AddTransient<ProfileEditViewModel>();
        services.AddTransient<SettingsViewModel>();

        // Statistics
        services.AddTransient<SpecializationStatsViewModel>();
    }

    private static void RegisterPages(IServiceCollection services)
    {
        // Dashboard
        services.AddTransient<DashboardPage>();

        // Procedures
        services.AddTransient<ProceduresPage>();
        services.AddTransient<ProcedureAddPage>();
        services.AddTransient<ProcedureEditPage>();

        // Duties
        services.AddTransient<DutiesPage>();
        services.AddTransient<DutyAddPage>();
        services.AddTransient<DutyEditPage>();

        // Profile
        services.AddTransient<ProfilePage>();
        services.AddTransient<ProfileEditPage>();
        services.AddTransient<SettingsPage>();

        // Statistics
        services.AddTransient<SpecializationStatsPage>();
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

using Microsoft.Extensions.Logging;
using SledzSpecke.App.Services.Platform;
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
using SledzSpecke.Infrastructure.Database.Configuration;
using SledzSpecke.Infrastructure.Database.Context;
using SledzSpecke.Infrastructure.Database.Migrations;
using SledzSpecke.Infrastructure.Database.Repositories;
using SledzSpecke.Infrastructure.Database.Initialization;
using SledzSpecke.Infrastructure.Services;
using SQLite;
using SledzSpecke.App.Services;
using SledzSpecke.App.Views.Specialization;
using SledzSpecke.App.ViewModels.Specializations;

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
            builder.Services.AddSingleton<IFileSystemService, FileSystemService>();

            // Register databases services
            RegisterDatabaseServices(builder.Services);

            // Register services
            RegisterServices(builder.Services);

            // Register ViewModels
            RegisterViewModels(builder.Services);

            // Register Pages
            RegisterPages(builder.Services);

#if DEBUG
            builder.Logging.AddDebug();
#endif

            var app = builder.Build();
            return app;
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

    private static void RegisterDatabaseServices(IServiceCollection services)
    {
        System.Diagnostics.Debug.WriteLine("Starting RegisterDatabaseServices");

        // First register IFileSystemService before anything else
        services.AddSingleton<IFileSystemService, FileSystemService>();

        // Store the database path in a variable for reuse
        var dbPath = string.Empty;
        services.AddSingleton(provider => {
            if (string.IsNullOrEmpty(dbPath))
            {
                var fileSystemService = provider.GetRequiredService<IFileSystemService>();
                dbPath = DatabaseConfig.GetDatabasePath(fileSystemService.GetAppDataDirectory());
            }
            return dbPath;
        });

        // Create a factory function for getting the connection
        services.AddSingleton<Func<SQLiteAsyncConnection>>(provider => {
            var path = provider.GetRequiredService<string>();
            return () => new SQLiteAsyncConnection(path);
        });

        // Register MigrationRunner with the connection factory
        services.AddSingleton<IMigrationRunner, MigrationRunner>();

        // Now register the ApplicationDbContext
        services.AddSingleton<IApplicationDbContext>(provider => {
            var dbPath = provider.GetRequiredService<string>();
            var migrationRunner = provider.GetRequiredService<IMigrationRunner>();
            return new ApplicationDbContext(dbPath, migrationRunner);
        });

        // Add DatabaseInitializer
        services.AddSingleton<DatabaseInitializer>();

        // Repositories
        services.AddSingleton<IUserRepository, UserRepository>();
        services.AddSingleton<ISpecializationRepository, SpecializationRepository>();
        services.AddSingleton<IProcedureRepository, ProcedureRepository>();
        services.AddSingleton<IDutyRepository, DutyRepository>();
        services.AddSingleton<ICourseRepository, CourseRepository>();
        services.AddSingleton<IInternshipRepository, InternshipRepository>();
        services.AddSingleton<INotificationRepository, NotificationRepository>();
    }

    private static void RegisterServices(IServiceCollection services)
    {
        services.AddSingleton<IProcedureService, ProcedureService>();
        services.AddSingleton<ISpecializationRequirementsProvider, SpecializationRequirementsProvider>();
        services.AddSingleton<ICourseService, CourseService>();
        services.AddSingleton<IInternshipService, InternshipService>();
        services.AddSingleton<IDutyService, DutyService>();
        services.AddSingleton<IUserService, UserService>();
        services.AddSingleton<ISpecializationService, SpecializationService>();
        services.AddSingleton<ISettingsService, SettingsService>();
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

        // Specialization
        services.AddTransient<SpecializationProgressViewModel>();
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

        // Specialization
        services.AddTransient<SpecializationProgressPage>();
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

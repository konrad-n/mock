using Microsoft.Extensions.Logging;
using SledzSpecke.App.Features.Absences.Views;
using SledzSpecke.App.Features.Authentication.ViewModels;
using SledzSpecke.App.Features.Authentication.Views;
using SledzSpecke.App.Features.Courses.Views;
using SledzSpecke.App.Features.Dashboard.Views;
using SledzSpecke.App.Features.Duties.Views;
using SledzSpecke.App.Features.Internships.Views;
using SledzSpecke.App.Features.Procedures.Views;
using SledzSpecke.App.Features.SelfEducations.Views;
using SledzSpecke.App.Features.Settings.ViewModels;
using SledzSpecke.App.Features.Settings.Views;
using SledzSpecke.App.Features.SMKExport.Views;
using SledzSpecke.App.Services;
using SledzSpecke.App.Services.Implementations;
using SledzSpecke.Infrastructure.Database;
using SledzSpecke.Infrastructure.Services;

namespace SledzSpecke.App
{
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

#if DEBUG
            builder.Logging.AddDebug();
#endif

            var app = builder.Build();

            // Utwórz scope dla głównego okna aplikacji
            using (var scope = app.Services.CreateScope())
            {
                // Inicjalizuj App z wszystkimi zależnościami
                var mainPage = scope.ServiceProvider.GetRequiredService<App>();
            }

            return app;
        }

        private static void RegisterServices(IServiceCollection services)
        {
            services.AddSingleton<App>();

            // Register ViewModels
            services.AddTransient<LoginViewModel>();
            services.AddTransient<SettingsViewModel>();

            // Register pages
            services.AddSingleton<NavigationPage>();
            services.AddTransient<LoginPage>();
            services.AddTransient<RegistrationPage>();
            services.AddTransient<DashboardPage>();
            services.AddTransient<SettingsPage>();
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
            services.AddTransient<AbsenceManagementPage>();
            services.AddTransient<AbsenceDetailsPage>();

            // Register services
            services.AddSingleton<IFileSystemService, MauiFileSystemService>();
            services.AddSingleton<IDatabaseService, DatabaseService>();
            services.AddSingleton<IDataManager, DataManager>();
            services.AddSingleton<ISpecializationService, SpecializationService>();
            services.AddSingleton<IDutyShiftService, DutyShiftService>();
            services.AddSingleton<ISelfEducationService, SelfEducationService>();
            services.AddSingleton<IExportService, ExportService>();
            services.AddSingleton<INotificationService, NotificationService>();
            services.AddSingleton<IAppSettings, AppSettings>();
            services.AddSingleton<IAuthenticationService, AuthenticationService>();
            services.AddSingleton<ISpecializationDateCalculator, SpecializationDateCalculator>();

            // Register shell last
            services.AddSingleton<AppShell>();
        }
    }
}
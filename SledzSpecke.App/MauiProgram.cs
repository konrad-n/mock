using Microsoft.Extensions.Logging;
using SledzSpecke.App.Helpers;
using SledzSpecke.App.Services.Authentication;
using SledzSpecke.App.Services.Database;
using SledzSpecke.App.Services.Dialog;
using SledzSpecke.App.Services.FileSystem;
using SledzSpecke.App.Services.MedicalShifts;
using SledzSpecke.App.Services.Notification;
using SledzSpecke.App.Services.Recognition;
using SledzSpecke.App.Services.Settings;
using SledzSpecke.App.Services.SmkStrategy;
using SledzSpecke.App.Services.Specialization;
using SledzSpecke.App.Services.Storage;
using SledzSpecke.App.ViewModels.Authentication;
using SledzSpecke.App.ViewModels.Dashboard;
using SledzSpecke.App.ViewModels.MedicalShifts;
using SledzSpecke.App.Views.Absences;
using SledzSpecke.App.Views.Authentication;
using SledzSpecke.App.Views.Courses;
using SledzSpecke.App.Views.Dashboard;
using SledzSpecke.App.Views.Export;
using SledzSpecke.App.Views.Internships;
using SledzSpecke.App.Views.MedicalShifts;
using SledzSpecke.App.Views.Procedures;
using SledzSpecke.App.Views.Publications;
using SledzSpecke.App.Views.SelfEducation;
using SledzSpecke.App.Views.Settings;

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
                });

            RegisterServices(builder.Services);
            RegisterViewModels(builder.Services);
            RegisterViews(builder.Services);

#if DEBUG
            builder.Logging.AddDebug();
#endif

            return builder.Build();
        }

        private static void RegisterServices(IServiceCollection services)
        {
            services.AddSingleton<IFileAccessHelper, FileAccessHelper>();
            services.AddSingleton<App>();
            services.AddSingleton<NavigationPage>();
            services.AddSingleton<AppShell>();
            services.AddSingleton<SplashPage>();
            services.AddSingleton<IDatabaseService, DatabaseService>();
            services.AddSingleton<IFileSystemService, FileSystemService>();
            services.AddSingleton<ISecureStorageService, SecureStorageService>();
            services.AddSingleton<IAuthService, AuthService>();
            services.AddSingleton<IDialogService, DialogService>();
            services.AddSingleton<ISpecializationService, SpecializationService>();
            services.AddSingleton<INotificationService, NotificationService>();
            services.AddSingleton<IRecognitionService, RecognitionService>(); // TODO: PUSTA KLASA!!! WAŻNE: TRZEBA JĄ ZAIMPLEMENTOWAĆ!!!
            services.AddSingleton<ISettingsService, SettingsService>(); // TODO: PUSTA KLASA!!! WAŻNE: TRZEBA JĄ ZAIMPLEMENTOWAĆ!!!
            services.AddSingleton<ISecureStorageService, SecureStorageService>(); // TODO: PUSTA KLASA!!! WAŻNE: TRZEBA JĄ ZAIMPLEMENTOWAĆ!!!
            services.AddSingleton<IMedicalShiftsService, MedicalShiftsService>();

            // Dodajemy ModuleInitializer jako usługę
            services.AddTransient<ModuleInitializer>();

            services.AddTransient<ISmkVersionStrategy>(provider =>
            {
                var authService = provider.GetService<IAuthService>();
                var user = authService?.GetCurrentUserAsync().GetAwaiter().GetResult();

                if (user != null)
                {
                    return SmkStrategyFactory.CreateStrategy(user.SmkVersion);
                }

                return new NewSmkStrategy();
            });
        }

        private static void RegisterViewModels(IServiceCollection services)
        {
            services.AddTransient<LoginViewModel>();
            services.AddTransient<RegisterViewModel>();
            services.AddTransient<DashboardViewModel>();

            services.AddTransient<MedicalShiftsSelectorViewModel>();
            services.AddTransient<OldSMKMedicalShiftsListViewModel>();
            services.AddTransient<NewSMKMedicalShiftsListViewModel>();
            services.AddTransient<AddEditOldSMKMedicalShiftViewModel>();
        }

        private static void RegisterViews(IServiceCollection services)
        {
            services.AddTransient<LoginPage>();
            services.AddTransient<RegisterPage>();
            services.AddTransient<DashboardPage>();

            services.AddTransient(sp => new ProceduresListPage(
                sp.GetRequiredService<IAuthService>()));
            services.AddTransient<MedicalShiftsSelectorPage>();

            // Zaktualizowana rejestracja z nowym konstruktorem
            services.AddTransient(sp => new OldSMKMedicalShiftsPage(
                sp.GetRequiredService<OldSMKMedicalShiftsListViewModel>(),
                sp.GetRequiredService<IMedicalShiftsService>()));

            services.AddTransient<NewSMKMedicalShiftsPage>();
            services.AddTransient<AddEditOldSMKMedicalShiftPage>();
            services.AddTransient<InternshipsListPage>();
            services.AddTransient<CoursesListPage>();
            services.AddTransient<SelfEducationListPage>();
            services.AddTransient<PublicationsListPage>();

            services.AddTransient<AbsencesListPage>();
            services.AddTransient<ExportPage>();
            services.AddTransient<ExportPreviewPage>();
            services.AddTransient<SettingsPage>();
        }
    }
}
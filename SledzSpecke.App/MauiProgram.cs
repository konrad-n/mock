using Microsoft.Extensions.Logging;
using SledzSpecke.App.Helpers;
using SledzSpecke.App.Services.Authentication;
using SledzSpecke.App.Services.Database;
using SledzSpecke.App.Services.Dialog;
using SledzSpecke.App.Services.FileSystem;
using SledzSpecke.App.Services.Notification;
using SledzSpecke.App.Services.SmkStrategy;
using SledzSpecke.App.Services.Specialization;
using SledzSpecke.App.Services.Storage;
using SledzSpecke.App.ViewModels.Authentication;
using SledzSpecke.App.ViewModels.MedicalShifts;
using SledzSpecke.App.Views.Authentication;
using SledzSpecke.App.Views.MedicalShifts;

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
            // Helpers
            services.AddSingleton<IFileAccessHelper, FileAccessHelper>();

            // Core app services
            services.AddSingleton<App>();
            services.AddSingleton<NavigationPage>();
            services.AddSingleton<AppShell>();
            services.AddSingleton<SplashPage>();

            // Database and storage services
            services.AddSingleton<IDatabaseService, DatabaseService>();
            services.AddSingleton<IFileSystemService, FileSystemService>();
            services.AddSingleton<ISecureStorageService, SecureStorageService>();

            // Business services
            services.AddSingleton<ISpecializationService, SpecializationService>();
            services.AddSingleton<INotificationService, NotificationService>();
            services.AddSingleton<IAuthService, AuthService>();

            // UI related services
            services.AddSingleton<IDialogService, DialogService>();

            // Strategy services - register based on user preferences
            // For now, we'll register New SMK strategy as default
            // Strategy services - rejestrujemy fabrykę zamiast bezpośrednio strategii
            services.AddTransient<ISmkVersionStrategy>(provider =>
            {
                // Próba uzyskania aktualnego użytkownika
                var authService = provider.GetService<IAuthService>();
                var user = authService?.GetCurrentUserAsync().GetAwaiter().GetResult();

                // Jeśli użytkownik istnieje, zwróć strategię zgodną z jego wersją SMK
                if (user != null)
                {
                    return SmkStrategyFactory.CreateStrategy(user.SmkVersion);
                }

                // Domyślnie nowa wersja SMK
                return new NewSmkStrategy();
            });
        }

        private static void RegisterViewModels(IServiceCollection services)
        {
            // Rejestracja ViewModeli autentykacji
            services.AddTransient<LoginViewModel>();
            services.AddTransient<RegisterViewModel>();

            // Pozostałe ViewModele
            services.AddTransient<MedicalShiftsListViewModel>();
            services.AddTransient<MedicalShiftDetailsViewModel>();
            services.AddTransient<AddEditMedicalShiftViewModel>();
        }

        private static void RegisterViews(IServiceCollection services)
        {
            // Rejestracja widoków autentykacji
            services.AddTransient<LoginPage>();
            services.AddTransient<RegisterPage>();

            // Pozostałe widoki
            services.AddTransient<MedicalShiftsListPage>();
            services.AddTransient<MedicalShiftDetailsPage>();
            services.AddTransient<AddEditMedicalShiftPage>();
        }
    }
}
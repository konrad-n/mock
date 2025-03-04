using Microsoft.Extensions.Logging;
using SledzSpecke.App.Services.Database;
using SledzSpecke.App.Services.Dialog;
using SledzSpecke.App.Services.FileSystem;
using SledzSpecke.App.Services.Notification;
using SledzSpecke.App.Services.SmkStrategy;
using SledzSpecke.App.Services.Specialization;
using SledzSpecke.App.Services.Storage;
using SledzSpecke.App.ViewModels.MedicalShifts;
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
            // Core app services
            services.AddSingleton<App>();
            services.AddSingleton<NavigationPage>();
            services.AddSingleton<AppShell>();

            // Database and storage services
            services.AddSingleton<IDatabaseService, DatabaseService>();
            services.AddSingleton<IFileSystemService, FileSystemService>();
            services.AddSingleton<ISecureStorageService, SecureStorageService>();

            // Business services
            services.AddSingleton<ISpecializationService, SpecializationService>();
            services.AddSingleton<INotificationService, NotificationService>();

            // UI related services
            services.AddSingleton<IDialogService, DialogService>();

            // Strategy services - register based on user preferences
            // For now, we'll register New SMK strategy as default
            services.AddSingleton<ISmkVersionStrategy, NewSmkStrategy>();
        }

        private static void RegisterViewModels(IServiceCollection services)
        {
            services.AddTransient<MedicalShiftsListViewModel>();
            services.AddTransient<MedicalShiftDetailsViewModel>();
            services.AddTransient<AddEditMedicalShiftViewModel>();
        }

        private static void RegisterViews(IServiceCollection services)
        {
            services.AddTransient<MedicalShiftsListPage>();
            services.AddTransient<MedicalShiftDetailsPage>();
            services.AddTransient<AddEditMedicalShiftPage>();
        }
    }
}
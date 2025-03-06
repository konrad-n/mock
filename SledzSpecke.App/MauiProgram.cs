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
using SledzSpecke.App.ViewModels.Export;
using SledzSpecke.App.ViewModels.Internships;
using SledzSpecke.App.ViewModels.MedicalShifts;
using SledzSpecke.App.ViewModels.Procedures;
using SledzSpecke.App.ViewModels.SelfEducation;
using SledzSpecke.App.Views.Authentication;
using SledzSpecke.App.Views.Export;
using SledzSpecke.App.Views.Internships;
using SledzSpecke.App.Views.MedicalShifts;
using SledzSpecke.App.Views.Procedures;
using SledzSpecke.App.Views.SelfEducation;

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
            services.AddSingleton<ISpecializationService, SpecializationService>();
            services.AddSingleton<INotificationService, NotificationService>();
            services.AddSingleton<IAuthService, AuthService>();
            services.AddSingleton<IDialogService, DialogService>();
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
            services.AddTransient<MedicalShiftsListViewModel>();
            services.AddTransient<MedicalShiftDetailsViewModel>();
            services.AddTransient<AddEditMedicalShiftViewModel>();
            services.AddTransient<SelfEducationListViewModel>();
            services.AddTransient<SelfEducationDetailsViewModel>();
            services.AddTransient<AddEditSelfEducationViewModel>();
            services.AddTransient<ExportViewModel>();
            services.AddTransient<ExportPreviewViewModel>();
            services.AddTransient<InternshipsListViewModel>();
            services.AddTransient<InternshipDetailsViewModel>();
            services.AddTransient<AddEditInternshipViewModel>();
            services.AddTransient<ProceduresListViewModel>();
        }

        private static void RegisterViews(IServiceCollection services)
        {
            services.AddTransient<LoginPage>();
            services.AddTransient<RegisterPage>();
            services.AddTransient<MedicalShiftsListPage>();
            services.AddTransient<MedicalShiftDetailsPage>();
            services.AddTransient<AddEditMedicalShiftPage>();
            services.AddTransient<SelfEducationListPage>();
            services.AddTransient<SelfEducationDetailsPage>();
            services.AddTransient<AddEditSelfEducationPage>();
            services.AddTransient<ExportPage>();
            services.AddTransient<ExportPreviewPage>();
            services.AddTransient<InternshipsListPage>();
            services.AddTransient<InternshipDetailsPage>();
            services.AddTransient<AddEditInternshipPage>();
            services.AddTransient<ProcedureDetailsPage>();
        }
    }
}
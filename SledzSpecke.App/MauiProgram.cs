using Microsoft.Extensions.Logging;
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

#if DEBUG
            builder.Logging.AddDebug();
#endif

            return builder.Build();
        }

        private static void RegisterServices(IServiceCollection services)
        {
            services.AddSingleton<App>();

            services.AddSingleton<NavigationPage>();

            services.AddSingleton<AppShell>();
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

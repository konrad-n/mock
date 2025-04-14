using Microsoft.Extensions.Logging;
using SledzSpecke.App.Helpers;
using SledzSpecke.App.Services.Authentication;
using SledzSpecke.App.Services.Database;
using SledzSpecke.App.Services.Dialog;
using SledzSpecke.App.Services.FileSystem;
using SledzSpecke.App.Services.MedicalShifts;
using SledzSpecke.App.Services.Procedures;
using SledzSpecke.App.Services.Specialization;
using SledzSpecke.App.Services.Storage;
using SledzSpecke.App.ViewModels.Authentication;
using SledzSpecke.App.ViewModels.Dashboard;
using SledzSpecke.App.ViewModels.Internships;
using SledzSpecke.App.ViewModels.MedicalShifts;
using SledzSpecke.App.ViewModels.Procedures;
using SledzSpecke.App.Views.Authentication;
using SledzSpecke.App.Views.Dashboard;
using SledzSpecke.App.Views.Export;
using SledzSpecke.App.Views.Internships;
using SledzSpecke.App.Views.MedicalShifts;
using SledzSpecke.App.Views.Procedures;

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
            services.AddSingleton<ISecureStorageService, SecureStorageService>();
            services.AddSingleton<IMedicalShiftsService, MedicalShiftsService>();
            services.AddSingleton<IProcedureService, ProcedureService>();
            services.AddTransient<ModuleInitializer>();
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
            services.AddTransient<AddEditOldSMKProcedureViewModel>();
            services.AddTransient<OldSMKProceduresListViewModel>();
            services.AddTransient<NewSMKProceduresListViewModel>();
            services.AddTransient<ProcedureSelectorViewModel>();
            services.AddTransient<ProcedureGroupViewModel>();
            services.AddTransient<ProcedureRequirementViewModel>();
            services.AddTransient<AddEditNewSMKProcedureViewModel>();
            services.AddTransient<InternshipsSelectorViewModel>();
            services.AddTransient<NewSMKInternshipsListViewModel>();
            services.AddTransient<AddEditInternshipViewModel>();
            services.AddTransient<OldSMKInternshipsListViewModel>();
            services.AddTransient<AddEditRealizedInternshipViewModel>();
        }

        private static void RegisterViews(IServiceCollection services)
        {
            services.AddTransient<LoginPage>();
            services.AddTransient<RegisterPage>();
            services.AddTransient<DashboardPage>();
            services.AddTransient<MedicalShiftsSelectorPage>();
            services.AddTransient(sp => new OldSMKMedicalShiftsPage(
                sp.GetRequiredService<OldSMKMedicalShiftsListViewModel>(),
                sp.GetRequiredService<IMedicalShiftsService>()));
            services.AddTransient<NewSMKMedicalShiftsPage>();
            services.AddTransient<AddEditOldSMKMedicalShiftPage>();
            services.AddTransient(sp => new OldSMKProceduresListPage(
                sp.GetRequiredService<OldSMKProceduresListViewModel>(),
                sp.GetRequiredService<IProcedureService>()));
            services.AddTransient<AddEditOldSMKProcedurePage>();
            services.AddTransient<NewSMKProceduresListPage>();
            services.AddTransient<ProcedureSelectorPage>();
            services.AddTransient<AddEditNewSMKProcedurePage>();
            services.AddTransient<ExportPage>();
            services.AddTransient<ExportPreviewPage>();
            services.AddTransient<InternshipsSelectorPage>();
            services.AddTransient<NewSMKInternshipsListPage>();
            services.AddTransient<AddEditInternshipPage>();
            services.AddTransient<OldSMKInternshipsListPage>();
            services.AddTransient<AddEditRealizedInternshipPage>();
        }
    }
}
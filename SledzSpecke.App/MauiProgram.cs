// --------------------------------------------------------------------------------------------------------------------
// <copyright file="MauiProgram.cs" company="SledzSpecke">
//   Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>
// <summary>
//   Punkt wejścia dla aplikacji MAUI. Konfiguruje usługi i zależności.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

using Microsoft.Extensions.Logging;
using SledzSpecke.App.Features.Absences.ViewModels;
using SledzSpecke.App.Features.Absences.Views;
using SledzSpecke.App.Features.Authentication.ViewModels;
using SledzSpecke.App.Features.Authentication.Views;
using SledzSpecke.App.Features.Courses.ViewModels;
using SledzSpecke.App.Features.Courses.Views;
using SledzSpecke.App.Features.Dashboard.ViewModels;
using SledzSpecke.App.Features.Dashboard.Views;
using SledzSpecke.App.Features.Duties.ViewModels;
using SledzSpecke.App.Features.Duties.Views;
using SledzSpecke.App.Features.Internships.ViewModels;
using SledzSpecke.App.Features.Internships.Views;
using SledzSpecke.App.Features.Procedures.ViewModels;
using SledzSpecke.App.Features.Procedures.Views;
using SledzSpecke.App.Features.SelfEducations.ViewModels;
using SledzSpecke.App.Features.SelfEducations.Views;
using SledzSpecke.App.Features.Settings.ViewModels;
using SledzSpecke.App.Features.Settings.Views;
using SledzSpecke.App.Features.SMKExport.ViewModels;
using SledzSpecke.App.Features.SMKExport.Views;
using SledzSpecke.App.Services.Implementations;
using SledzSpecke.App.Services.Interfaces;
using SledzSpecke.Infrastructure.Database;
using SledzSpecke.Infrastructure.Services;

namespace SledzSpecke.App
{
    /// <summary>
    /// Klasa konfigurująca aplikację MAUI.
    /// </summary>
    public static class MauiProgram
    {
        /// <summary>
        /// Tworzy i konfiguruje aplikację MAUI.
        /// </summary>
        /// <returns>Skonfigurowana instancja aplikacji MAUI.</returns>
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

            // Register services
            RegisterServices(builder.Services);

#if DEBUG
            builder.Logging.AddDebug();
#endif

            return builder.Build();
        }

        /// <summary>
        /// Rejestruje usługi w kontenerze DI.
        /// </summary>
        /// <param name="services">Kolekcja usług DI.</param>
        private static void RegisterServices(IServiceCollection services)
        {
            services.AddSingleton<App>();

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
            services.AddSingleton<INavigationService, NavigationService>();

            // Register ViewModels
            services.AddTransient<LoginViewModel>();
            services.AddTransient<SettingsViewModel>();
            services.AddTransient<DashboardViewModel>();
            services.AddTransient<CoursesViewModel>();
            services.AddTransient<ProceduresViewModel>();
            services.AddTransient<DutyShiftsViewModel>();
            services.AddTransient<SelfEducationViewModel>();
            services.AddTransient<InternshipsViewModel>();
            services.AddTransient<AbsenceManagementViewModel>();
            services.AddTransient<ProcedureEntryViewModel>();
            services.AddTransient<DutyShiftDetailsViewModel>();
            services.AddTransient<SMKExportViewModel>();
            services.AddTransient<AbsenceDetailsViewModel>();
            services.AddTransient<RegistrationViewModel>();
            services.AddTransient<CourseDetailsViewModel>();
            services.AddTransient<InternshipDetailsViewModel>();
            services.AddTransient<ProcedureDetailsViewModel>();
            services.AddTransient<SelfEducationDetailsViewModel>();

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

            // Register shell last
            services.AddSingleton<AppShell>();
        }
    }
}
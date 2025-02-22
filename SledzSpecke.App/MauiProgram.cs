using Microsoft.Extensions.Logging;
using SledzSpecke.App.Views.Dashboard;
using SledzSpecke.App.Views.Procedures;
using SledzSpecke.App.Views.Duties;
using SledzSpecke.App.ViewModels.Dashboard;
using SledzSpecke.App.ViewModels.Procedures;
using SledzSpecke.App.ViewModels.Duties;
using SledzSpecke.App.Views.Profile;
using SledzSpecke.Core.Interfaces.Services;
using SledzSpecke.Infrastructure.Database.Configuration;
using SledzSpecke.Infrastructure.Database.Context;
using SledzSpecke.Infrastructure.Database.Repositories;
using SledzSpecke.Infrastructure.Database.Migrations;
using SledzSpecke.Infrastructure.Services;
using SledzSpecke.App.Services.Platform;

namespace SledzSpecke.App;

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

        // Rejestracja serwisów
        builder.Services.AddSingleton<IPermissionService, PermissionService>();
        builder.Services.AddSingleton<IProcedureService, ProcedureService>();
        builder.Services.AddSingleton<IDutyService, DutyService>();

        // Rejestracja ViewModels
        builder.Services.AddTransient<DashboardViewModel>();
        builder.Services.AddTransient<ProceduresViewModel>();
        builder.Services.AddTransient<DutiesViewModel>();
        // Procedury
        builder.Services.AddTransient<ProcedureAddViewModel>();
        builder.Services.AddTransient<ProcedureEditViewModel>();
        // Dyżury
        builder.Services.AddTransient<DutyAddViewModel>();
        builder.Services.AddTransient<DutyEditViewModel>();

        // Rejestracja stron
        builder.Services.AddTransient<DashboardPage>();
        builder.Services.AddTransient<ProceduresPage>();
        builder.Services.AddTransient<DutiesPage>();
        // Procedury
        builder.Services.AddTransient<ProcedureAddPage>();
        builder.Services.AddTransient<ProcedureEditPage>();
        // Dyżury
        builder.Services.AddTransient<DutyAddPage>();
        builder.Services.AddTransient<DutyEditPage>();
        // Ustawienia
        builder.Services.AddTransient<SettingsPage>();
        // Profil
        builder.Services.AddTransient<ProfilePage>();

        // Repositoria
        builder.Services.AddScoped<IUserRepository, UserRepository>();
        builder.Services.AddScoped<ISpecializationRepository, SpecializationRepository>();
        builder.Services.AddScoped<IProcedureRepository, ProcedureRepository>();
        builder.Services.AddScoped<IDutyRepository, DutyRepository>();

        // Dodaj rejestrację serwisu
        builder.Services.AddSingleton<IFileSystemService, FileSystemService>();

        // I użyj go przy tworzeniu kontekstu
        builder.Services.AddSingleton<IMigrationRunner, MigrationRunner>();
        builder.Services.AddSingleton<IApplicationDbContext>(provider =>
        {
            var migrationRunner = provider.GetRequiredService<IMigrationRunner>();
            var fileSystemService = provider.GetRequiredService<IFileSystemService>();
            return new ApplicationDbContext(
                DatabaseConfig.GetDatabasePath(fileSystemService.GetAppDataDirectory()),
                migrationRunner);
        });


#if DEBUG
        builder.Logging.AddDebug();
#endif

        return builder.Build();
    }
}
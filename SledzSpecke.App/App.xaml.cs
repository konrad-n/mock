using Microsoft.Extensions.Logging;
using SledzSpecke.App.Features.Authentication.Views;
using SledzSpecke.App.Services.Interfaces;
using SledzSpecke.Infrastructure.Database;

namespace SledzSpecke.App
{
    public partial class App : Application
    {
        private readonly ILogger<App> _logger;
        private readonly IServiceProvider _serviceProvider;
        private readonly INotificationService _notificationService;
        private readonly IAppSettings _appSettings;
        private readonly IDatabaseService _databaseService;
        private readonly IAuthenticationService _authenticationService;

        public App(
            IServiceProvider serviceProvider,
            ILogger<App> logger,
            INotificationService notificationService,
            IAppSettings appSettings,
            IDatabaseService databaseService,
            IAuthenticationService authenticationService)
        {
            this.InitializeComponent();

            this._serviceProvider = serviceProvider;
            this._logger = logger;
            this._notificationService = notificationService;
            this._appSettings = appSettings;
            this._databaseService = databaseService;
            this._authenticationService = authenticationService;

            try
            {
                this.MainPage = new NavigationPage(this._serviceProvider.GetRequiredService<LoginPage>());
                _ = this.InitializeAsync();
            }
            catch (Exception ex)
            {
                this._logger.LogError(ex, "Error in App constructor");
                this.MainPage = new NavigationPage(this._serviceProvider.GetRequiredService<LoginPage>());
            }
        }

        private async Task InitializeAsync()
        {
            try
            {
                this._logger.LogInformation("Starting app initialization");

                await this._databaseService.InitAsync();
                this._logger.LogDebug("Database initialized");

                var userSeeded = await this._authenticationService.SeedTestUserAsync();
                this._logger.LogDebug("Test user seeded: {Result}", userSeeded);

                await this._appSettings.LoadAsync();
                this._logger.LogDebug("Settings loaded");

                bool useDarkTheme = this._appSettings.GetSetting<bool>("UseDarkTheme");
                Application.Current.UserAppTheme = useDarkTheme ? AppTheme.Dark : AppTheme.Light;
                this._logger.LogDebug("Theme applied: {Theme}", useDarkTheme ? "Dark" : "Light");

                MainThread.BeginInvokeOnMainThread(() =>
                {
                    this.MainPage = new NavigationPage(this._serviceProvider.GetRequiredService<LoginPage>());
                });

                this._logger.LogInformation("Application initialized successfully");
            }
            catch (Exception ex)
            {
                this._logger.LogError(ex, "Error initializing application");
                MainThread.BeginInvokeOnMainThread(() =>
                {
                    this.MainPage = new NavigationPage(this._serviceProvider.GetRequiredService<LoginPage>());
                });
            }
        }

        protected override void OnStart()
        {
            try
            {
                _ = this._notificationService.CheckAndScheduleNotificationsAsync();
            }
            catch (Exception ex)
            {
                this._logger.LogError(ex, "Error in OnStart");
            }
        }

        protected override void OnSleep()
        {
            try
            {
                _ = this._appSettings.SaveAsync();
            }
            catch (Exception ex)
            {
                this._logger.LogError(ex, "Error in OnSleep");
            }
        }

        protected override void OnResume()
        {
            try
            {
                _ = this._notificationService.CheckAndScheduleNotificationsAsync();
            }
            catch (Exception ex)
            {
                this._logger.LogError(ex, "Error in OnResume");
            }
        }

        // Tymczasowa metoda pomocnicza do uzyskania serwisu
        // Docelowo należy usunąć tę metodę, gdy wszystkie strony będą używać DI
        public static TService GetService<TService>(IElement element) where TService : class
        {
            if (element.Handler?.MauiContext == null)
                throw new InvalidOperationException("Element handler is not initialized");

            var services = element.Handler.MauiContext.Services;
            var service = services.GetService<TService>();

            if (service == null)
                throw new InvalidOperationException($"Service {typeof(TService).Name} not found");

            return service;
        }
    }
}
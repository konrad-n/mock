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
            InitializeComponent();

            _serviceProvider = serviceProvider;
            _logger = logger;
            _notificationService = notificationService;
            _appSettings = appSettings;
            _databaseService = databaseService;
            _authenticationService = authenticationService;

            try
            {
                MainPage = new NavigationPage(_serviceProvider.GetRequiredService<LoginPage>());
                _ = InitializeAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in App constructor");
                MainPage = new NavigationPage(_serviceProvider.GetRequiredService<LoginPage>());
            }
        }

        private async Task InitializeAsync()
        {
            try
            {
                _logger.LogInformation("Starting app initialization");

                await _databaseService.InitAsync();
                _logger.LogDebug("Database initialized");

                var userSeeded = await _authenticationService.SeedTestUserAsync();
                _logger.LogDebug("Test user seeded: {Result}", userSeeded);

                await _appSettings.LoadAsync();
                _logger.LogDebug("Settings loaded");

                bool useDarkTheme = _appSettings.GetSetting<bool>("UseDarkTheme");
                Application.Current.UserAppTheme = useDarkTheme ? AppTheme.Dark : AppTheme.Light;
                _logger.LogDebug("Theme applied: {Theme}", useDarkTheme ? "Dark" : "Light");

                MainThread.BeginInvokeOnMainThread(() =>
                {
                    MainPage = new NavigationPage(_serviceProvider.GetRequiredService<LoginPage>());
                });

                _logger.LogInformation("Application initialized successfully");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error initializing application");
                MainThread.BeginInvokeOnMainThread(() =>
                {
                    MainPage = new NavigationPage(_serviceProvider.GetRequiredService<LoginPage>());
                });
            }
        }

        protected override void OnStart()
        {
            try
            {
                _ = _notificationService.CheckAndScheduleNotificationsAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in OnStart");
            }
        }

        protected override void OnSleep()
        {
            try
            {
                _ = _appSettings.SaveAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in OnSleep");
            }
        }

        protected override void OnResume()
        {
            try
            {
                _ = _notificationService.CheckAndScheduleNotificationsAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in OnResume");
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
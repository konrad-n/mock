using Microsoft.Extensions.Logging;
using SledzSpecke.App.Features.Authentication.Views;
using SledzSpecke.App.Services.Interfaces;
using SledzSpecke.Infrastructure.Database;

namespace SledzSpecke.App
{
    public partial class App : Application
    {
        private readonly ILogger<App> logger;
        private readonly IServiceProvider serviceProvider;
        private readonly INotificationService notificationService;
        private readonly IAppSettings appSettings;
        private readonly IDatabaseService databaseService;
        private readonly IAuthenticationService authenticationService;

        public App(
            IServiceProvider serviceProvider,
            ILogger<App> logger,
            INotificationService notificationService,
            IAppSettings appSettings,
            IDatabaseService databaseService,
            IAuthenticationService authenticationService)
        {
            this.InitializeComponent();

            this.serviceProvider = serviceProvider;
            this.logger = logger;
            this.notificationService = notificationService;
            this.appSettings = appSettings;
            this.databaseService = databaseService;
            this.authenticationService = authenticationService;

            try
            {
                this.MainPage = new NavigationPage(this.serviceProvider.GetRequiredService<LoginPage>());
                _ = this.InitializeAsync();
            }
            catch (Exception ex)
            {
                this.logger.LogError(ex, "Error in App constructor");
                this.MainPage = new NavigationPage(this.serviceProvider.GetRequiredService<LoginPage>());
            }
        }

        protected override void OnStart()
        {
            try
            {
                _ = this.notificationService.CheckAndScheduleNotificationsAsync();
            }
            catch (Exception ex)
            {
                this.logger.LogError(ex, "Error in OnStart");
            }
        }

        protected override void OnSleep()
        {
            try
            {
                _ = this.appSettings.SaveAsync();
            }
            catch (Exception ex)
            {
                this.logger.LogError(ex, "Error in OnSleep");
            }
        }

        protected override void OnResume()
        {
            try
            {
                _ = this.notificationService.CheckAndScheduleNotificationsAsync();
            }
            catch (Exception ex)
            {
                this.logger.LogError(ex, "Error in OnResume");
            }
        }

        private async Task InitializeAsync()
        {
            try
            {
                this.logger.LogInformation("Starting app initialization");

                await this.databaseService.InitAsync();
                this.logger.LogDebug("Database initialized");

                var userSeeded = await this.authenticationService.SeedTestUserAsync();
                this.logger.LogDebug("Test user seeded: {Result}", userSeeded);

                await this.appSettings.LoadAsync();
                this.logger.LogDebug("Settings loaded");

                bool useDarkTheme = this.appSettings.GetSetting<bool>("UseDarkTheme");
                Application.Current.UserAppTheme = useDarkTheme ? AppTheme.Dark : AppTheme.Light;
                this.logger.LogDebug("Theme applied: {Theme}", useDarkTheme ? "Dark" : "Light");

                MainThread.BeginInvokeOnMainThread(() =>
                {
                    this.MainPage = new NavigationPage(this.serviceProvider.GetRequiredService<LoginPage>());
                });

                this.logger.LogInformation("Application initialized successfully");
            }
            catch (Exception ex)
            {
                this.logger.LogError(ex, "Error initializing application");
                MainThread.BeginInvokeOnMainThread(() =>
                {
                    this.MainPage = new NavigationPage(this.serviceProvider.GetRequiredService<LoginPage>());
                });
            }
        }
    }
}
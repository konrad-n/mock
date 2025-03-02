// --------------------------------------------------------------------------------------------------------------------
// <copyright file="App.xaml.cs" company="SledzSpecke">
//   Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>
// <summary>
//   Punkt wejścia aplikacji.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

using Microsoft.Extensions.Logging;
using SledzSpecke.App.Features.Authentication.Views;
using SledzSpecke.App.Services.Interfaces;
using SledzSpecke.Infrastructure.Database;

namespace SledzSpecke.App
{
    /// <summary>
    /// Główna klasa aplikacji.
    /// </summary>
    public partial class App : Application
    {
        private readonly ILogger<App> logger;
        private readonly IServiceProvider serviceProvider;
        private readonly INotificationService notificationService;
        private readonly IAppSettings appSettings;
        private readonly IDatabaseService databaseService;
        private readonly IAuthenticationService authenticationService;

        /// <summary>
        /// Initializes a new instance of the <see cref="App"/> class.
        /// </summary>
        /// <param name="serviceProvider">Dostawca usług DI.</param>
        /// <param name="logger">Logger aplikacji.</param>
        /// <param name="notificationService">Serwis powiadomień.</param>
        /// <param name="appSettings">Ustawienia aplikacji.</param>
        /// <param name="databaseService">Serwis bazy danych.</param>
        /// <param name="authenticationService">Serwis uwierzytelniania.</param>
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
                this.CreateStartupWindow();
                _ = this.InitializeAsync();
            }
            catch (Exception ex)
            {
                this.logger.LogError(ex, "Error in App constructor");
                this.CreateStartupWindow();
            }
        }

        /// <summary>
        /// Wywoływane przy rozpoczęciu pracy aplikacji.
        /// </summary>
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

        /// <summary>
        /// Wywoływane gdy aplikacja przechodzi w stan uśpienia.
        /// </summary>
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

        /// <summary>
        /// Wywoływane gdy aplikacja wznawia działanie.
        /// </summary>
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

        /// <summary>
        /// Tworzy okno startowe aplikacji.
        /// </summary>
        private void CreateStartupWindow()
        {
            if (Application.Current?.Windows.Count == 0)
            {
                Window window = new (new NavigationPage(this.serviceProvider.GetRequiredService<LoginPage>()));
                this.Windows.Add(window);
            }
            else if (Application.Current?.Windows.Count > 0)
            {
                Application.Current.Windows[0].Page = new NavigationPage(this.serviceProvider.GetRequiredService<LoginPage>());
            }
        }

        /// <summary>
        /// Inicjalizuje aplikację asynchronicznie.
        /// </summary>
        /// <returns>Task reprezentujący operację asynchroniczną.</returns>
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
                    if (Application.Current?.Windows.Count > 0)
                    {
                        Application.Current.Windows[0].Page = new NavigationPage(this.serviceProvider.GetRequiredService<LoginPage>());
                    }
                });

                this.logger.LogInformation("Application initialized successfully");
            }
            catch (Exception ex)
            {
                this.logger.LogError(ex, "Error initializing application");
                MainThread.BeginInvokeOnMainThread(() =>
                {
                    if (Application.Current?.Windows.Count > 0)
                    {
                        Application.Current.Windows[0].Page = new NavigationPage(this.serviceProvider.GetRequiredService<LoginPage>());
                    }
                });
            }
        }
    }
}
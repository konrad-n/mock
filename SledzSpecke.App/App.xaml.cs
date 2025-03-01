using Microsoft.Extensions.Logging;
using SledzSpecke.App.Features.Authentication.Views;
using SledzSpecke.App.Services;
using SledzSpecke.Infrastructure.Database;

namespace SledzSpecke.App
{
    public partial class App : Application
    {
        // Statyczne serwisy jako properties (Tymczasowo!!!)
        public static IAuthenticationService AuthenticationService => Current.Handler?.MauiContext?.Services.GetRequiredService<IAuthenticationService>();
        public static IDatabaseService DatabaseService => Current.Handler?.MauiContext?.Services.GetRequiredService<IDatabaseService>();
        public static IDataManager DataManager => Current.Handler?.MauiContext?.Services.GetRequiredService<IDataManager>();
        public static ISpecializationService SpecializationService => Current.Handler?.MauiContext?.Services.GetRequiredService<ISpecializationService>();
        public static IDutyShiftService DutyShiftService => Current.Handler?.MauiContext?.Services.GetRequiredService<IDutyShiftService>();
        public static ISelfEducationService SelfEducationService => Current.Handler?.MauiContext?.Services.GetRequiredService<ISelfEducationService>();
        public static IExportService ExportService => Current.Handler?.MauiContext?.Services.GetRequiredService<IExportService>();
        public static INotificationService NotificationService => Current.Handler?.MauiContext?.Services.GetRequiredService<INotificationService>();
        public static IAppSettings AppSettings => Current.Handler?.MauiContext?.Services.GetRequiredService<IAppSettings>();
        public static ISpecializationDateCalculator SpecializationDateCalculator => Current.Handler?.MauiContext?.Services.GetRequiredService<ISpecializationDateCalculator>();


        private readonly ILogger<App> _logger;
        private readonly IServiceProvider _serviceProvider;

        public App(
            IServiceProvider serviceProvider,
            ILogger<App> logger)
        {
            InitializeComponent();

            _serviceProvider = serviceProvider;
            _logger = logger;

            try
            {
                MainPage = new NavigationPage(_serviceProvider.GetRequiredService<LoginPage>());
                InitializeAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in App constructor");
                MainPage = new NavigationPage(_serviceProvider.GetRequiredService<LoginPage>());
            }
        }

        private async void InitializeAsync()
        {
            try
            {
                _logger.LogInformation("Starting app initialization");

                await DatabaseService.InitAsync();
                _logger.LogDebug("Database initialized");

                var userSeeded = await AuthenticationService.SeedTestUserAsync();
                _logger.LogDebug("Test user seeded: {Result}", userSeeded);

                await AppSettings.LoadAsync();
                _logger.LogDebug("Settings loaded");

                bool useDarkTheme = AppSettings.GetSetting<bool>("UseDarkTheme");
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
                _ = NotificationService.CheckAndScheduleNotificationsAsync();
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
                _ = AppSettings.SaveAsync();
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
                _ = NotificationService.CheckAndScheduleNotificationsAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in OnResume");
            }
        }

        // Dodaj metody pomocnicze do dostępu do serwisów
        public static IServiceProvider GetServiceProvider(IElement element)
        {
            return ((IServiceProvider)element.Handler?.MauiContext?.Services)
                ?? throw new InvalidOperationException("Unable to get ServiceProvider");
        }

        public static T GetService<T>(IElement element) where T : class
        {
            return GetServiceProvider(element).GetService<T>()
                ?? throw new InvalidOperationException($"Service {typeof(T).Name} not found");
        }
    }
}
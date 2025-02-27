using Microsoft.Extensions.Logging;
using SledzSpecke.App.Services;
using SledzSpecke.App.Views.Auth;
using SledzSpecke.Infrastructure.Database;

namespace SledzSpecke.App
{
    public partial class App : Application
    {
        public static DatabaseService DatabaseService { get; private set; }
        public static DataManager DataManager { get; private set; }
        public static ExportService ExportService { get; private set; }
        public static NotificationService NotificationService { get; private set; }
        public static AppSettings AppSettings { get; private set; }
        public static SpecializationService SpecializationService { get; private set; }
        public static DutyShiftService DutyShiftService { get; private set; }
        public static SelfEducationService SelfEducationService { get; private set; }
        public static AuthenticationService AuthenticationService { get; private set; }

        private static readonly ILogger<App> _logger = LoggerFactory.Create(builder =>
            builder.AddDebug()).CreateLogger<App>();

        public App()
        {
            InitializeComponent();

            try
            {
                // Create file system service
                var fileSystemService = new MauiFileSystemService();

                // Initialize services
                DatabaseService = new DatabaseService(fileSystemService, LoggerFactory.Create(builder =>
                    builder.AddDebug()).CreateLogger<DatabaseService>());

                DataManager = new DataManager(DatabaseService, LoggerFactory.Create(builder =>
                    builder.AddDebug()).CreateLogger<DataManager>());

                ExportService = new ExportService(DatabaseService, LoggerFactory.Create(builder =>
                    builder.AddDebug()).CreateLogger<ExportService>());

                NotificationService = new NotificationService(DatabaseService, LoggerFactory.Create(builder =>
                    builder.AddDebug()).CreateLogger<NotificationService>());

                AppSettings = new AppSettings(DatabaseService, LoggerFactory.Create(builder =>
                    builder.AddDebug()).CreateLogger<AppSettings>());

                SpecializationService = new SpecializationService(DatabaseService, LoggerFactory.Create(builder =>
                    builder.AddDebug()).CreateLogger<SpecializationService>());

                DutyShiftService = new DutyShiftService(DatabaseService, LoggerFactory.Create(builder =>
                    builder.AddDebug()).CreateLogger<DutyShiftService>());

                SelfEducationService = new SelfEducationService(DatabaseService, LoggerFactory.Create(builder =>
                    builder.AddDebug()).CreateLogger<SelfEducationService>());

                AuthenticationService = new AuthenticationService(DatabaseService, LoggerFactory.Create(builder =>
                    builder.AddDebug()).CreateLogger<AuthenticationService>());

                // Set a default MainPage immediately to prevent exceptions
                MainPage = new NavigationPage(new LoginPage());

                // Then initialize app async
                InitializeAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in App constructor");
                // Set a default page in case of exception
                MainPage = new NavigationPage(new LoginPage());
            }
        }

        private async void InitializeAsync()
        {
            try
            {
                _logger.LogInformation("Starting app initialization");

                // Initialize database
                await DatabaseService.InitAsync();
                _logger.LogDebug("Database initialized");

                // Seed test user
                var userSeeded = await AuthenticationService.SeedTestUserAsync();
                _logger.LogDebug("Test user seeded: {Result}", userSeeded);

                // Load settings
                await AppSettings.LoadAsync();
                _logger.LogDebug("Settings loaded");

                // Apply theme setting
                bool useDarkTheme = AppSettings.GetSetting<bool>("UseDarkTheme");
                Application.Current.UserAppTheme = useDarkTheme ? AppTheme.Dark : AppTheme.Light;
                _logger.LogDebug("Theme applied: {Theme}", useDarkTheme ? "Dark" : "Light");

                // Set initial page (using MainThread to ensure UI updates happen on the main thread)
                MainThread.BeginInvokeOnMainThread(() =>
                {
                    MainPage = new NavigationPage(new LoginPage());
                });

                _logger.LogInformation("Application initialized successfully");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error initializing application");
                // Ensure we have a valid page in case of error
                MainThread.BeginInvokeOnMainThread(() =>
                {
                    MainPage = new NavigationPage(new LoginPage() { BackgroundColor = Colors.White });
                });
            }
        }

        protected override void OnStart()
        {
            try
            {
                // Check for notifications when app starts
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
                // Save settings when app goes to sleep
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
                // Check for notifications when app resumes
                _ = NotificationService.CheckAndScheduleNotificationsAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in OnResume");
            }
        }
    }
}
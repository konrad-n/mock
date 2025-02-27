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

        private async void InitializeAsync()
        {
            try
            {
                // Initialize database
                await DatabaseService.InitAsync();

                // Seed test user
                await AuthenticationService.SeedTestUserAsync();

                // Load settings
                await AppSettings.LoadAsync();

                // Apply theme setting
                bool useDarkTheme = AppSettings.GetSetting<bool>("UseDarkTheme");
                Application.Current.UserAppTheme = useDarkTheme ? AppTheme.Dark : AppTheme.Light;

                // Check if user is already logged in
                // TODO: Implement auto-login with token system

                // Set initial page
                MainPage = new NavigationPage(new LoginPage());

                _logger.LogInformation("Application initialized successfully");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error initializing application");
                // Show error page or dialog
                // MainPage is already set in constructor, so we don't need to set it again
            }
        }

        protected override void OnStart()
        {
            // Check for notifications when app starts
            _ = NotificationService.CheckAndScheduleNotificationsAsync();
        }

        protected override void OnSleep()
        {
            // Save settings when app goes to sleep
            _ = AppSettings.SaveAsync();
        }

        protected override void OnResume()
        {
            // Check for notifications when app resumes
            _ = NotificationService.CheckAndScheduleNotificationsAsync();
        }
    }
}
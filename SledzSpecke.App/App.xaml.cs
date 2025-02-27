using Microsoft.Extensions.Logging;
using SledzSpecke.App.Services;
using SledzSpecke.Infrastructure.Database;

namespace SledzSpecke.App;

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

    private static readonly ILogger<App> _logger = LoggerFactory.Create(builder =>
        builder.AddDebug()).CreateLogger<App>();

    public App()
    {
        InitializeComponent();

        // Initialize services
        DatabaseService = new DatabaseService(LoggerFactory.Create(builder =>
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

        // Asynchronously load settings
        _ = InitializeAsync();

        MainPage = new AppShell();
    }

    private async Task InitializeAsync()
    {
        try
        {
            // Load settings
            await AppSettings.LoadAsync();

            // Apply theme setting
            bool useDarkTheme = AppSettings.GetSetting<bool>("UseDarkTheme");
            Application.Current.UserAppTheme = useDarkTheme ? AppTheme.Dark : AppTheme.Light;

            // Ensure specialization is loaded
            await SpecializationService.GetSpecializationAsync();

            _logger.LogInformation("Application initialized successfully");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error initializing application");
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
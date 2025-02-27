using SledzSpecke.App.Services;

namespace SledzSpecke.App;

public partial class App : Application
{
    public static DataManager DataManager { get; private set; }
    public static ExportService ExportService { get; private set; }
    public static NotificationService NotificationService { get; private set; }
    public static AppSettings AppSettings { get; private set; }
    public static SpecializationService SpecializationService { get; private set; }

    public App()
    {
        InitializeComponent();

        // Inicjalizacja usług
        DataManager = new DataManager();
        ExportService = new ExportService(DataManager);
        NotificationService = new NotificationService(DataManager);
        AppSettings = new AppSettings();

        // Initialize the SpecializationService
        SpecializationService = new SpecializationService();

        // Asynchroniczne ładowanie ustawień
        _ = InitializeSettingsAsync();

        MainPage = new AppShell();
    }

    private async Task InitializeSettingsAsync()
    {
        try
        {
            await AppSettings.LoadAsync();
            bool useDarkTheme = AppSettings.GetSetting<bool>("UseDarkTheme");
            if (useDarkTheme)
            {
                Application.Current.UserAppTheme = AppTheme.Dark;
            }
            else
            {
                Application.Current.UserAppTheme = AppTheme.Light;
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error initializing settings: {ex.Message}");
        }
    }

    protected override void OnStart()
    {
        // Initialize the specialization at app start
        _ = SpecializationService.GetSpecializationAsync();

        // Sprawdzenie i planowanie powiadomień przy uruchomieniu aplikacji
        _ = NotificationService.CheckAndScheduleNotificationsAsync();
    }

    protected override void OnSleep()
    {
        // Zapisanie ustawień i danych przy wstrzymaniu aplikacji
        _ = AppSettings.SaveAsync();
    }

    protected override void OnResume()
    {
        // Sprawdzenie i planowanie powiadomień przy wznowieniu aplikacji
        _ = NotificationService.CheckAndScheduleNotificationsAsync();
    }
}
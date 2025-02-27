using System;
using Microsoft.Maui.Controls;
using SledzSpecke.Services;

namespace SledzSpecke
{
    public partial class App : Application
    {
        public static DataManager DataManager { get; private set; }
        public static ExportService ExportService { get; private set; }
        public static NotificationService NotificationService { get; private set; }
        public static AppSettings AppSettings { get; private set; }



        public App()
        {
            InitializeComponent();

            // Inicjalizacja usług
            DataManager = new DataManager();
            ExportService = new ExportService(DataManager);
            NotificationService = new NotificationService(DataManager);
            AppSettings = new AppSettings();

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
}
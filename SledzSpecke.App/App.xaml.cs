using SledzSpecke.App.Services.Database;
using SledzSpecke.App.Services.FileSystem;
using SledzSpecke.App.Services.Storage;

namespace SledzSpecke.App
{

    public partial class App : Application
    {
        public App()
        {
            this.InitializeComponent();
            Helpers.Constants.SetFileSystemService(new FileSystemService());
            Helpers.SettingsHelper.SetSecureStorageService(new SecureStorageService());
        }

        protected override async void OnStart()
        {
            base.OnStart();

            Task.Run(async () =>
            {
                try
                {
                    var dbService = IPlatformApplication.Current.Services.GetRequiredService<IDatabaseService>();
                    await dbService.MigrateShiftDataForModulesAsync();
                }
                catch (Exception ex)
                {
                    System.Diagnostics.Debug.WriteLine($"Błąd podczas migracji: {ex.Message}");
                }
            });
        }

        protected override Window CreateWindow(IActivationState? activationState)
        {
            var splashPage = IPlatformApplication.Current.Services.GetRequiredService<SplashPage>();
            return new Window(splashPage);
        }
    }
}
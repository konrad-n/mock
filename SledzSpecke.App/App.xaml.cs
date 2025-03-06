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

        protected override Window CreateWindow(IActivationState? activationState)
        {
            var splashPage = IPlatformApplication.Current.Services.GetRequiredService<SplashPage>();
            return new Window(splashPage);
        }
    }
}
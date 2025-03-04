namespace SledzSpecke.App
{
    public partial class App : Application
    {
        public App()
        {
            this.InitializeComponent();

            // Initialize helpers with service implementations
            Helpers.Constants.SetFileSystemService(new Services.FileSystem.FileSystemService());
            Helpers.Settings.SetSecureStorageService(new Services.Storage.SecureStorageService());
        }

        protected override Window CreateWindow(IActivationState? activationState)
        {
            return new Window(new SplashPage());
        }
    }
}
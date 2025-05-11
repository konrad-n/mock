using SledzSpecke.App.Services.Database;
using SledzSpecke.App.Services.FileSystem;
using SledzSpecke.App.Services.Storage;
using SledzSpecke.App.Services.Exceptions;

namespace SledzSpecke.App
{
    public partial class App : Application
    {
        public App()
        {
            this.InitializeComponent();
            Helpers.Constants.SetFileSystemService(new FileSystemService());
            Helpers.SettingsHelper.SetSecureStorageService(new SecureStorageService());

            // Dodajemy obsługę nieobsłużonych wyjątków
            AppDomain.CurrentDomain.UnhandledException += OnUnhandledException;
            TaskScheduler.UnobservedTaskException += OnUnobservedTaskException;

            // Obsługa platformowo-specyficznych wyjątków
#if ANDROID
            Android.Runtime.AndroidEnvironment.UnhandledExceptionRaiser += OnAndroidUnhandledException;
#endif
#if IOS
            ObjCRuntime.Runtime.MarshalManagedException += OnIOSMarshalManagedException;
#endif
        }

        protected override async void OnStart()
        {
            base.OnStart();

            Task.Run(async () =>
            {
                var dbService = IPlatformApplication.Current.Services.GetRequiredService<IDatabaseService>();
                await dbService.MigrateShiftDataForModulesAsync();
                await dbService.MigrateInternshipDataAsync();
            });
        }

        protected override Window CreateWindow(IActivationState? activationState)
        {
            var splashPage = IPlatformApplication.Current.Services.GetRequiredService<SplashPage>();
            return new Window(splashPage);
        }

        // Nowe metody do obsługi nieobsłużonych wyjątków
        private void OnUnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            var exception = e.ExceptionObject as Exception;
            LogAndHandleException(exception, "Nieobsłużony wyjątek aplikacji");
        }

        private void OnUnobservedTaskException(object sender, UnobservedTaskExceptionEventArgs e)
        {
            LogAndHandleException(e.Exception, "Nieobsłużony wyjątek zadania");
            e.SetObserved(); // Oznacza wyjątek jako obsłużony
        }

#if ANDROID
        private void OnAndroidUnhandledException(object sender, Android.Runtime.RaiseThrowableEventArgs e)
        {
            LogAndHandleException(new Exception(e.Exception.ToString()), "Nieobsłużony wyjątek Android");
            e.Handled = true; // Oznacza wyjątek jako obsłużony
        }
#endif

#if IOS
        private void OnIOSMarshalManagedException(object sender, ObjCRuntime.MarshalManagedExceptionEventArgs e)
        {
            LogAndHandleException(e.Exception, "Nieobsłużony wyjątek iOS");
            e.ExceptionMode = ObjCRuntime.MarshalManagedExceptionMode.UnwindNativeCode; // Obsłuż wyjątek
        }
#endif

        private void LogAndHandleException(Exception exception, string source)
        {
            try
            {
                var exceptionHandler = IPlatformApplication.Current.Services.GetService<IExceptionHandlerService>();

                if (exceptionHandler != null)
                {
                    // Asynchronicznie obsługujemy wyjątek, ale nie możemy czekać (await) 
                    // w kontekście tych handlerów
                    Task.Run(() => exceptionHandler.HandleExceptionAsync(exception, source)).Wait();
                }
                else
                {
                    // Awaryjne logowanie, jeśli serwis nie jest dostępny
                    System.Diagnostics.Debug.WriteLine($"KRYTYCZNY BŁĄD ({source}): {exception.Message}");
                    System.Diagnostics.Debug.WriteLine($"StackTrace: {exception.StackTrace}");
                }
            }
            catch (Exception logEx)
            {
                // Ostateczne zabezpieczenie - w przypadku błędu podczas logowania
                System.Diagnostics.Debug.WriteLine($"BŁĄD PODCZAS LOGOWANIA WYJĄTKU: {logEx.Message}");
                System.Diagnostics.Debug.WriteLine($"ORYGINALNY WYJĄTEK: {exception?.Message}");
            }
        }
    }
}
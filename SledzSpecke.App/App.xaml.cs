using Microsoft.Maui.ApplicationModel;

namespace SledzSpecke.App
{
    public partial class App : Application
    {
        private readonly IPermissionService _permissionService;

        public App(IPermissionService permissionService)
        {
            InitializeComponent();
            _permissionService = permissionService;
        }

        protected override Window CreateWindow(IActivationState? activationState)
        {
            return new Window(new AppShell());
        }

        protected override async void OnStart()
        {
            base.OnStart();
            await CheckCriticalPermissions();
        }

        private async Task CheckCriticalPermissions()
        {
            // Sprawdź i poproś o krytyczne uprawnienia
            var permissionResults = await _permissionService.RequestPermissionsAsync(
                typeof(Permissions.StorageRead),
                typeof(Permissions.StorageWrite)
            );

            // Sprawdź wyniki i obsłuż odmowy
            foreach (var (permissionType, status) in permissionResults)
            {
                if (status == PermissionStatus.Unknown)
                {
                    var window = Windows.FirstOrDefault();
                    if (window?.Page != null)
                    {
                        await window.Page.DisplayAlert(
                            "Błąd uprawnień",
                            "Nie można sprawdzić uprawnień aplikacji. Spróbuj ponownie uruchomić aplikację lub sprawdź ustawienia systemu.",
                            "OK");
                        return;
                    }
                }

                if (status != PermissionStatus.Granted)
                {
                    string message = permissionType switch
                    {
                        Type t when t == typeof(Permissions.StorageRead) =>
                            "Aplikacja potrzebuje dostępu do pamięci urządzenia, aby przechowywać dane.",
                        Type t when t == typeof(Permissions.StorageWrite) =>
                            "Aplikacja potrzebuje możliwości zapisu danych w pamięci urządzenia.",
                        Type t when t == typeof(Permissions.Camera) =>
                            "Aplikacja potrzebuje dostępu do aparatu, aby skanować kody QR.",
                        _ => "Aplikacja potrzebuje tego uprawnienia do poprawnego działania."
                    };

                    var permissionGranted = await HandlePermissionDenial(permissionType, message);
                    if (!permissionGranted)
                    {
                        var window = Windows.FirstOrDefault();
                        if (window?.Page != null)
                        {
                            await window.Page.DisplayAlert(
                                "Brak wymaganych uprawnień",
                                "Aplikacja nie może działać bez wymaganych uprawnień.",
                                "OK");
                        }
                    }
                }
            }
        }

        private async Task<bool> HandlePermissionDenial(Type permissionType, string message)
        {
            // Użyj refleksji do utworzenia generycznego wywołania HandleDeniedPermissionAsync
            var method = typeof(IPermissionService)
                .GetMethod(nameof(IPermissionService.HandleDeniedPermissionAsync));

            if (method is null)
            {
                return false;
            }

            var genericMethod = method.MakeGenericMethod(permissionType);

            try
            {
                var result = await (Task<bool>)genericMethod.Invoke(
                    _permissionService,
                    new[] { message })!;
                return result;
            }
            catch
            {
                return false;
            }
        }
    }
}
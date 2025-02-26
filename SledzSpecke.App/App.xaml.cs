using SledzSpecke.Infrastructure.Database.Initialization;

namespace SledzSpecke.App;

public partial class App : Application
{
    private readonly DatabaseInitializer _initializer;

    public App(DatabaseInitializer initializer)
    {
        InitializeComponent();
        _initializer = initializer;
        MainPage = new AppShell();
    }

    protected override async void OnStart()
    {
        base.OnStart();
        await InitializeDatabaseAsync();
    }

    private async Task InitializeDatabaseAsync()
    {
        try
        {
            await _initializer.InitializeAsync();
            System.Diagnostics.Debug.WriteLine("Database initialization completed");
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"Database initialization error: {ex.Message}");
            System.Diagnostics.Debug.WriteLine(ex.StackTrace);
            // We'll continue even if database initialization fails
            // This ensures the app can still run with stub services
        }
    }
}

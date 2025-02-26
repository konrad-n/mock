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
        await _initializer.InitializeAsync();
        System.Diagnostics.Debug.WriteLine("Database initialization completed");
    }
}

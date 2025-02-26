namespace SledzSpecke.App;

public partial class SplashPage : ContentPage
{
    public SplashPage()
    {
        InitializeComponent();
    }

    protected override async void OnNavigatedTo(NavigatedToEventArgs args)
    {
        base.OnNavigatedTo(args);

        try
        {
            await InitializeAppAsync();

            // Change this to navigate to a route that exists in your AppShell
            await Shell.Current.GoToAsync("//dashboard");
            // If that doesn't work, try:
            // await Shell.Current.GoToAsync("//Dashboard");
            // Or check AppShell.xaml.cs for exact route names
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Navigation error: {ex.Message}");
            await DisplayAlert("Error", "Failed to initialize the app", "OK");
        }
    }

    private async Task InitializeAppAsync()
    {
        try
        {
            // Your initialization code here
            await Task.Delay(2000);

            // Try navigating to the dashboard instead of MainPage
            await Shell.Current.GoToAsync("//dashboard");
        }
        catch (Exception ex)
        {
            await DisplayAlert("Error", $"Initialization failed: {ex.Message}", "OK");
        }
    }
}
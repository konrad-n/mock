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
            // Change this to an existing route like Dashboard
            await Shell.Current.GoToAsync("//Dashboard");
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"Initialization error: {ex.Message}");
            await DisplayAlert("Błąd", "Nie udało się zainicjalizować aplikacji", "OK");
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
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
            await Shell.Current.GoToAsync("//dashboard");
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
            // Simulate loading for now
            await Task.Delay(2000);
        }
        catch (Exception ex)
        {
            await DisplayAlert("Error", $"Initialization failed: {ex.Message}", "OK");
        }
    }
}
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
            await Shell.Current.GoToAsync("//MainPage");
        }
        catch (Exception ex)
        {
            // Log the exception or handle it appropriately
            System.Diagnostics.Debug.WriteLine($"Initialization error: {ex.Message}");
            await DisplayAlert("Błąd", "Nie udało się zainicjalizować aplikacji", "OK");
        }
    }

    private async Task InitializeAppAsync()
    {
        await Task.Delay(2000); // Symulacja ładowania
    }
}
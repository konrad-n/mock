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
            await Shell.Current.GoToAsync("//dashboard");
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"Initialization error: {ex.Message}");
            await DisplayAlert("Błąd", "Nie udało się zainicjalizować aplikacji", "OK");
        }
    }
}

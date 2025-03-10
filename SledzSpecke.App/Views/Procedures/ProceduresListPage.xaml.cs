using SledzSpecke.App.Services.Authentication;

namespace SledzSpecke.App.Views.Procedures
{
    public partial class ProceduresListPage : ContentPage
    {
        private readonly IAuthService authService;

        public ProceduresListPage(IAuthService authService)
        {
            this.InitializeComponent();
            this.authService = authService;
        }

        protected override async void OnAppearing()
        {
            base.OnAppearing();

            try
            {
                // Przekierowanie do selektora procedur, który zdecyduje o właściwym widoku
                await Shell.Current.GoToAsync("ProcedureSelector");
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Błąd podczas nawigacji do selektora procedur: {ex.Message}");
                // Obsługa błędu, jeśli nawigacja się nie powiedzie
                await DisplayAlert("Błąd", "Nie udało się otworzyć widoku procedur. Spróbuj ponownie później.", "OK");
            }
        }
    }
}
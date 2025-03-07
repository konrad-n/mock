using SledzSpecke.App.Models.Enums;
using SledzSpecke.App.Services.Authentication;

namespace SledzSpecke.App.Views.MedicalShifts
{
    public partial class MedicalShiftsSelectorPage : ContentPage
    {
        private readonly IAuthService authService;

        public MedicalShiftsSelectorPage(IAuthService authService)
        {
            this.InitializeComponent();
            this.authService = authService;
        }

        protected override async void OnAppearing()
        {
            base.OnAppearing();

            try
            {
                // Pobierz aktualnego użytkownika
                var user = await this.authService.GetCurrentUserAsync();

                if (user != null)
                {
                    // Przekieruj w zależności od wersji SMK
                    if (user.SmkVersion == SmkVersion.Old)
                    {
                        await Shell.Current.GoToAsync("OldSMKMedicalShifts");
                    }
                    else
                    {
                        await Shell.Current.GoToAsync("NewSMKMedicalShifts");
                    }
                }
                else
                {
                    // Jeśli nie ma użytkownika, pozostań na tej stronie i pokaż komunikat
                    await DisplayAlert("Błąd", "Nie można określić wersji SMK. Skontaktuj się z administratorem.", "OK");
                }
            }
            catch (System.Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Błąd podczas przekierowania: {ex.Message}");
                await DisplayAlert("Błąd", "Wystąpił problem podczas ładowania danych. Spróbuj ponownie.", "OK");
            }
        }
    }
}
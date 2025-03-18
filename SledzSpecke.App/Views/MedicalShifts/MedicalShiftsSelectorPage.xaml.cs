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
            var user = await this.authService.GetCurrentUserAsync();

            if (user != null)
            {
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
                await DisplayAlert("Błąd", "Nie można określić wersji SMK. Skontaktuj się z administratorem.", "OK");
            }
        }
    }
}
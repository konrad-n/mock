using SledzSpecke.App.Models.Enums;
using SledzSpecke.App.Services.Authentication;

namespace SledzSpecke.App.Views.Internships
{
    public partial class InternshipsSelectorPage : ContentPage
    {
        private readonly IAuthService authService;

        public InternshipsSelectorPage(IAuthService authService)
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
                    await Shell.Current.GoToAsync("/OldSMKInternships");
                }
                else
                {
                    await Shell.Current.GoToAsync("/NewSMKInternships");
                }
            }
            else
            {
                await DisplayAlert("Błąd", "Nie można określić wersji SMK. Skontaktuj się z administratorem.", "OK");
            }
        }
    }
}
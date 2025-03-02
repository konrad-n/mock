using SledzSpecke.App.Features.Authentication.ViewModels;

namespace SledzSpecke.App.Features.Authentication.Views
{
    public partial class LoginPage
    {
        private LoginViewModel viewModel;

        public LoginPage()
        {
            this.InitializeComponent();
        }

        protected override async Task InitializePageAsync()
        {
            try
            {
                this.viewModel = this.GetRequiredService<LoginViewModel>();
                System.Diagnostics.Debug.WriteLine($"Debug mode: Email = {this.viewModel.Email}, Password = {this.viewModel.Password}");
                this.BindingContext = this.viewModel;
            }
            catch (Exception ex)
            {
                await this.DisplayAlert("Błąd", "Nie udało się zainicjalizować strony logowania.", "OK");
                System.Diagnostics.Debug.WriteLine($"Error in LoginPage: {ex}");
            }
        }
    }
}
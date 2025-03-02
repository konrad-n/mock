using SledzSpecke.App.Features.Authentication.ViewModels;

namespace SledzSpecke.App.Features.Authentication.Views
{
    public partial class LoginPage
    {
        private LoginViewModel _viewModel;

        public LoginPage()
        {
            this.InitializeComponent();
        }

        protected override async Task InitializePageAsync()
        {
            try
            {
                this._viewModel = this.GetRequiredService<LoginViewModel>();

                // Dodaj debug log
                System.Diagnostics.Debug.WriteLine($"Debug mode: Email = {this._viewModel.Email}, Password = {this._viewModel.Password}");

                this.BindingContext = this._viewModel;
            }
            catch (Exception ex)
            {
                await this.DisplayAlert("Błąd", "Nie udało się zainicjalizować strony logowania.", "OK");
                System.Diagnostics.Debug.WriteLine($"Error in LoginPage: {ex}");
            }
        }
    }
}
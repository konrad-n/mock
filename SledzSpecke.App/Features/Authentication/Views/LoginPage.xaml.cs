using SledzSpecke.App.Features.Authentication.ViewModels;

namespace SledzSpecke.App.Features.Authentication.Views
{
    public partial class LoginPage
    {
        private LoginViewModel _viewModel;

        public LoginPage()
        {
            InitializeComponent();
        }

        protected override async Task InitializePageAsync()
        {
            try
            {
                _viewModel = GetRequiredService<LoginViewModel>();

                // Dodaj debug log
                System.Diagnostics.Debug.WriteLine($"Debug mode: Email = {_viewModel.Email}, Password = {_viewModel.Password}");

                BindingContext = _viewModel;
            }
            catch (Exception ex)
            {
                await DisplayAlert("Błąd", "Nie udało się zainicjalizować strony logowania.", "OK");
                System.Diagnostics.Debug.WriteLine($"Error in LoginPage: {ex}");
            }
        }
    }
}
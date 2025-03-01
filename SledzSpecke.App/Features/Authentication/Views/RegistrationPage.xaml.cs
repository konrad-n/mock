using SledzSpecke.App.Common.Views;
using SledzSpecke.App.Features.Authentication.ViewModels;

namespace SledzSpecke.App.Features.Authentication.Views
{
    public partial class RegistrationPage : BaseContentPage
    {
        private RegistrationViewModel _viewModel;

        public RegistrationPage()
        {
            InitializeComponent();
        }

        protected override async Task InitializePageAsync()
        {
            try
            {
                _viewModel = GetRequiredService<RegistrationViewModel>();
                BindingContext = _viewModel;
                await _viewModel.InitializeAsync();
            }
            catch (Exception ex)
            {
                await DisplayAlert("Błąd", "Nie udało się zainicjalizować strony rejestracji.", "OK");
                System.Diagnostics.Debug.WriteLine($"Error in RegistrationPage: {ex}");
            }
        }

        private async void OnRegisterClicked(object sender, EventArgs e)
        {
            if (_viewModel != null)
            {
                await _viewModel.RegisterAsync();
            }
        }
    }
}
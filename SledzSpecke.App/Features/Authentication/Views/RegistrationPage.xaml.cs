using SledzSpecke.App.Common.Views;
using SledzSpecke.App.Features.Authentication.ViewModels;

namespace SledzSpecke.App.Features.Authentication.Views
{
    public partial class RegistrationPage : BaseContentPage
    {
        private RegistrationViewModel _viewModel;

        public RegistrationPage()
        {
            this.InitializeComponent();
        }

        protected override async Task InitializePageAsync()
        {
            try
            {
                this._viewModel = this.GetRequiredService<RegistrationViewModel>();
                this.BindingContext = this._viewModel;
                await this._viewModel.InitializeAsync();
            }
            catch (Exception ex)
            {
                await this.DisplayAlert("Błąd", "Nie udało się zainicjalizować strony rejestracji.", "OK");
                System.Diagnostics.Debug.WriteLine($"Error in RegistrationPage: {ex}");
            }
        }

        private async void OnRegisterClicked(object sender, EventArgs e)
        {
            if (this._viewModel != null)
            {
                await this._viewModel.RegisterAsync();
            }
        }
    }
}
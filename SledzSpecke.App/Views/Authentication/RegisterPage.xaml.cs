using SledzSpecke.App.ViewModels.Authentication;

namespace SledzSpecke.App.Views.Authentication
{
    public partial class RegisterPage : ContentPage
    {
        private readonly RegisterViewModel viewModel;

        public RegisterPage(RegisterViewModel viewModel)
        {
            this.InitializeComponent();
            this.BindingContext = viewModel;
            this.viewModel = viewModel;
        }

        protected override async void OnAppearing()
        {
            base.OnAppearing();

            await this.viewModel.InitializeAsync();
        }
    }
}
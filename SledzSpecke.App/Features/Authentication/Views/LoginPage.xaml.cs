using SledzSpecke.App.Features.Authentication.ViewModels;

namespace SledzSpecke.App.Views.Auth
{
    public partial class LoginPage : ContentPage
    {
        public LoginPage(LoginViewModel viewModel)
        {
            InitializeComponent();
            BindingContext = viewModel;
        }
    }
}
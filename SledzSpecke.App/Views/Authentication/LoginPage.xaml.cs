using SledzSpecke.App.ViewModels.Authentication;

namespace SledzSpecke.App.Views.Authentication
{
    public partial class LoginPage : ContentPage
    {
        public LoginPage(LoginViewModel viewModel)
        {
            this.InitializeComponent();
            this.BindingContext = viewModel;
        }
    }
}
using SledzSpecke.App.ViewModels.Authentication;

namespace SledzSpecke.App.Views.Authentication
{
    public partial class RegisterPage : ContentPage
    {
        public RegisterPage(RegisterViewModel viewModel)
        {
            this.InitializeComponent();
            this.BindingContext = viewModel;
        }
    }
}
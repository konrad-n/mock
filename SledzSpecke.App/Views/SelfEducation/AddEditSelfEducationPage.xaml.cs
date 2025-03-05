using SledzSpecke.App.ViewModels.SelfEducation;
using SledzSpecke.App.Models;

namespace SledzSpecke.App.Views.SelfEducation
{
    public partial class AddEditSelfEducationPage : ContentPage
    {
        private readonly AddEditSelfEducationViewModel viewModel;

        public AddEditSelfEducationPage(AddEditSelfEducationViewModel viewModel)
        {
            this.InitializeComponent();
            this.BindingContext = viewModel;
            this.viewModel = viewModel;
        }
    }
}
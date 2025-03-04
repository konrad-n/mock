using SledzSpecke.App.ViewModels.SelfEducation;

namespace SledzSpecke.App.Views.SelfEducation
{
    public partial class SelfEducationDetailsPage : ContentPage
    {
        private readonly SelfEducationDetailsViewModel viewModel;

        public SelfEducationDetailsPage(SelfEducationDetailsViewModel viewModel)
        {
            this.InitializeComponent();
            this.BindingContext = viewModel;
            this.viewModel = viewModel;
        }
    }
}
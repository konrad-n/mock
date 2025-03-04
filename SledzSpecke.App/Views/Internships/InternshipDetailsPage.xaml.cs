using SledzSpecke.App.ViewModels.Internships;

namespace SledzSpecke.App.Views.Internships
{
    public partial class InternshipDetailsPage : ContentPage
    {
        private readonly InternshipDetailsViewModel viewModel;

        public InternshipDetailsPage(InternshipDetailsViewModel viewModel)
        {
            this.InitializeComponent();
            this.BindingContext = viewModel;
            this.viewModel = viewModel;
        }
    }
}
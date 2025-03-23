using SledzSpecke.App.ViewModels.Internships;

namespace SledzSpecke.App.Views.Internships
{
    public partial class AddEditInternshipPage : ContentPage
    {
        private readonly AddEditInternshipViewModel viewModel;

        public AddEditInternshipPage(AddEditInternshipViewModel viewModel)
        {
            this.InitializeComponent();
            this.viewModel = viewModel;
            this.BindingContext = this.viewModel;
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();
            this.viewModel.InitializeAsync().ConfigureAwait(false);
        }
    }
}
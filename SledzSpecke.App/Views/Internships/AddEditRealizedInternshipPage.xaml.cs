using SledzSpecke.App.ViewModels.Internships;

namespace SledzSpecke.App.Views.Internships
{
    public partial class AddEditRealizedInternshipPage : ContentPage
    {
        private readonly AddEditRealizedInternshipViewModel viewModel;

        public AddEditRealizedInternshipPage(AddEditRealizedInternshipViewModel viewModel)
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
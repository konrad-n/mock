using SledzSpecke.App.ViewModels.Internships;

namespace SledzSpecke.App.Views.Internships
{
    public partial class AddEditNewSMKInternshipPage : ContentPage
    {
        private readonly AddEditNewSMKInternshipViewModel viewModel;

        public AddEditNewSMKInternshipPage(AddEditNewSMKInternshipViewModel viewModel)
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
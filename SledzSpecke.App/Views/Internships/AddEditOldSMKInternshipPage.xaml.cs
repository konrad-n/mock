using SledzSpecke.App.ViewModels.Internships;

namespace SledzSpecke.App.Views.Internships
{
    public partial class AddEditOldSMKInternshipPage : ContentPage
    {
        private readonly AddEditOldSMKInternshipViewModel viewModel;

        public AddEditOldSMKInternshipPage(AddEditOldSMKInternshipViewModel viewModel)
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
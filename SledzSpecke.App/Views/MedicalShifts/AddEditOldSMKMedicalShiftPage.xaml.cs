using SledzSpecke.App.ViewModels.MedicalShifts;

namespace SledzSpecke.App.Views.MedicalShifts
{
    public partial class AddEditOldSMKMedicalShiftPage : ContentPage
    {
        private readonly AddEditOldSMKMedicalShiftViewModel viewModel;

        public AddEditOldSMKMedicalShiftPage(AddEditOldSMKMedicalShiftViewModel viewModel)
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
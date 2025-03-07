using SledzSpecke.App.ViewModels.MedicalShifts;

namespace SledzSpecke.App.Views.MedicalShifts
{
    public partial class AddEditOldSMKMedicalShiftPage : ContentPage
    {
        public AddEditOldSMKMedicalShiftPage(AddEditOldSMKMedicalShiftViewModel viewModel)
        {
            this.InitializeComponent();
            this.BindingContext = viewModel;
        }
    }
}
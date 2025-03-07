using SledzSpecke.App.ViewModels.MedicalShifts;

namespace SledzSpecke.App.Views.MedicalShifts
{
    public partial class AddEditMedicalShiftPage : ContentPage
    {
        public AddEditMedicalShiftPage(AddEditMedicalShiftViewModel viewModel)
        {
            this.InitializeComponent();
            this.BindingContext = viewModel;
        }
    }
}
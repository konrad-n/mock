using SledzSpecke.App.ViewModels.MedicalShifts;

namespace SledzSpecke.App.Views.MedicalShifts
{
    public partial class MedicalShiftsSelectorPage : ContentPage
    {
        public MedicalShiftsSelectorPage(MedicalShiftsSelectorViewModel viewModel)
        {
            this.InitializeComponent();
            this.BindingContext = viewModel;
        }
    }
}
using SledzSpecke.App.ViewModels.MedicalShifts;

namespace SledzSpecke.App.Views.MedicalShifts
{
    public partial class MedicalShiftsListPage : ContentPage
    {
        private readonly MedicalShiftsListViewModel viewModel;

        public MedicalShiftsListPage(MedicalShiftsListViewModel viewModel)
        {
            this.InitializeComponent();
            this.viewModel = viewModel;
            this.BindingContext = this.viewModel;
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();

            // Odśwież dane przy każdym pokazaniu strony
            this.viewModel.RefreshCommand.Execute(null);
        }
    }
}
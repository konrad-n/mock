using SledzSpecke.App.ViewModels.Procedures;

namespace SledzSpecke.App.Views.Procedures
{
    public partial class AddEditOldSMKProcedurePage : ContentPage
    {
        private readonly AddEditOldSMKProcedureViewModel viewModel;

        public AddEditOldSMKProcedurePage(AddEditOldSMKProcedureViewModel viewModel)
        {
            this.InitializeComponent();
            this.viewModel = viewModel;
            this.BindingContext = this.viewModel;
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();

            // Wywołaj inicjalizację ViewModelu
            this.viewModel.InitializeAsync().ConfigureAwait(false);
        }
    }
}
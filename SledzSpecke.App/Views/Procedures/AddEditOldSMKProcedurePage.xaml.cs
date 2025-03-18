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
            this.BtnSave.Clicked += this.OnSaveClicked;
        }

        private async void OnSaveClicked(object sender, EventArgs e)
        {
            await this.viewModel.OnSaveAsync();
        }

        protected override async void OnAppearing()
        {
            base.OnAppearing();

            await this.viewModel.InitializeAsync();
        }
    }
}
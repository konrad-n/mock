using SledzSpecke.App.ViewModels.Procedures;

namespace SledzSpecke.App.Views.Procedures
{
    public partial class ProcedureSelectorPage : ContentPage
    {
        private readonly ProcedureSelectorViewModel viewModel;

        public ProcedureSelectorPage(ProcedureSelectorViewModel viewModel)
        {
            this.InitializeComponent();
            this.viewModel = viewModel;
            this.BindingContext = this.viewModel;
        }

        protected override async void OnAppearing()
        {
            base.OnAppearing();
            await this.viewModel.InitializeAsync();
        }
    }
}
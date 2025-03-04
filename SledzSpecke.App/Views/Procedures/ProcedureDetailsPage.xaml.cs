using SledzSpecke.App.ViewModels.Procedures;

namespace SledzSpecke.App.Views.Procedures
{
    public partial class ProcedureDetailsPage : ContentPage
    {
        private readonly ProcedureDetailsViewModel viewModel;

        public ProcedureDetailsPage(ProcedureDetailsViewModel viewModel)
        {
            this.InitializeComponent();
            this.BindingContext = viewModel;
            this.viewModel = viewModel;
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();

            // Można dodać dodatkową logikę inicjalizacyjną jeśli potrzeba
        }
    }
}
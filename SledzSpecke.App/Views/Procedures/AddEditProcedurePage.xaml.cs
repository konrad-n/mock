using SledzSpecke.App.ViewModels.Procedures;

namespace SledzSpecke.App.Views.Procedures
{
    public partial class AddEditProcedurePage : ContentPage
    {
        private readonly AddEditProcedureViewModel viewModel;

        public AddEditProcedurePage(AddEditProcedureViewModel viewModel)
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
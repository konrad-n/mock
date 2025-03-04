using SledzSpecke.App.ViewModels.Procedures;

namespace SledzSpecke.App.Views.Procedures
{
    public partial class ProceduresListPage : ContentPage
    {
        private readonly ProceduresListViewModel viewModel;

        public ProceduresListPage(ProceduresListViewModel viewModel)
        {
            this.InitializeComponent();
            this.BindingContext = viewModel;
            this.viewModel = viewModel;
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();

            try
            {
                // Odśwież listę procedur przy każdym wejściu na stronę
                this.viewModel.RefreshCommand.Execute(null);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Błąd podczas odświeżania listy procedur: {ex.Message}");
            }
        }
    }
}
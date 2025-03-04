using SledzSpecke.App.ViewModels.Internships;

namespace SledzSpecke.App.Views.Internships
{
    public partial class InternshipsListPage : ContentPage
    {
        private readonly InternshipsListViewModel viewModel;

        public InternshipsListPage(InternshipsListViewModel viewModel)
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
                // Odśwież listę staży przy każdym wejściu na stronę
                this.viewModel.RefreshCommand.Execute(null);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Błąd podczas odświeżania listy staży: {ex.Message}");
            }
        }
    }
}
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
            // Bezpośrednie podpięcie zdarzenia do przycisku Zapisz
            this.BtnSave.Clicked += this.OnSaveClicked;
        }

        private async void OnSaveClicked(object sender, EventArgs e)
        {
            System.Diagnostics.Debug.WriteLine("OnSaveClicked: Kliknięto przycisk Zapisz");
            try
            {
                await this.viewModel.OnSaveAsync();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Błąd podczas zapisywania: {ex.Message}");
                await DisplayAlert("Błąd", $"Wystąpił błąd podczas zapisywania: {ex.Message}", "OK");
            }
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();

            // Wywołaj inicjalizację ViewModelu
            this.viewModel.InitializeAsync().ConfigureAwait(false);
        }
    }
}
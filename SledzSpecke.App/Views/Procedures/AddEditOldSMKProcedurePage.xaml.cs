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

        protected override async void OnAppearing()
        {
            base.OnAppearing();

            try
            {
                // Kluczowa zmiana - używamy await bez ConfigureAwait(false)
                // aby upewnić się, że kontynuacja dzieje się na głównym wątku UI
                await this.viewModel.InitializeAsync();
                System.Diagnostics.Debug.WriteLine("OnAppearing: Zainicjalizowano ViewModel pomyślnie");
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"OnAppearing: Błąd podczas inicjalizacji ViewModelu: {ex.Message}");
                await DisplayAlert("Błąd", $"Wystąpił błąd podczas inicjalizacji: {ex.Message}", "OK");
            }
        }
    }
}
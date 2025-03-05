using SledzSpecke.App.Models;
using SledzSpecke.App.ViewModels.Export;

namespace SledzSpecke.App.Views.Export
{
    [QueryProperty(nameof(SerializedOptions), "options")]
    public partial class ExportPreviewPage : ContentPage
    {
        private readonly ExportPreviewViewModel viewModel;
        private string serializedOptions;

        public ExportPreviewPage(ExportPreviewViewModel viewModel)
        {
            this.InitializeComponent();
            this.BindingContext = viewModel;
            this.viewModel = viewModel;
        }

        // Właściwość dla przekazywania opcji eksportu w formacie JSON
        public string SerializedOptions
        {
            get => this.serializedOptions;
            set
            {
                this.serializedOptions = value;
                this.InitializePageAsync();
            }
        }

        private async void InitializePageAsync()
        {
            try
            {
                if (string.IsNullOrEmpty(this.serializedOptions))
                {
                    // Jeśli nie przekazano opcji, wracamy do strony eksportu
                    await Shell.Current.GoToAsync("..");
                    return;
                }

                // Deserializujemy opcje eksportu
                var options = System.Text.Json.JsonSerializer.Deserialize<ExportOptions>(this.serializedOptions);
                if (options == null)
                {
                    throw new Exception("Nie udało się zdekodować opcji eksportu");
                }

                // Inicjalizujemy ViewModel z opcjami
                await this.viewModel.InitializeAsync(options);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Błąd inicjalizacji strony podglądu: {ex.Message}");
                await this.DisplayAlert("Błąd", "Wystąpił problem podczas inicjalizacji podglądu eksportu.", "OK");
                await Shell.Current.GoToAsync("..");
            }
        }
    }
}

using SledzSpecke.App.Models;
using SledzSpecke.App.Services.Procedures;
using SledzSpecke.App.ViewModels.Procedures;

namespace SledzSpecke.App.Views.Procedures
{
    public partial class OldSMKProceduresListPage : ContentPage
    {
        private readonly OldSMKProceduresListViewModel viewModel;
        private readonly IProcedureService procedureService; // Dodane bezpośrednie odwołanie do serwisu

        public OldSMKProceduresListPage(OldSMKProceduresListViewModel viewModel, IProcedureService procedureService)
        {
            this.InitializeComponent();
            this.viewModel = viewModel;
            this.procedureService = procedureService;
            this.BindingContext = this.viewModel;
        }

        protected override void OnNavigatedTo(NavigatedToEventArgs args)
        {
            base.OnNavigatedTo(args);

            // Ustaw właściwość BackButtonBehavior dla Shell'a
            Shell.SetBackButtonBehavior(this, new BackButtonBehavior
            {
                Command = new Command(async () =>
                {
                    await Shell.Current.GoToAsync("///dashboard");
                })
            });
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();

            // Odśwież dane przy każdym pokazaniu strony
            this.viewModel.RefreshCommand.Execute(null);
        }

        protected override void OnDisappearing()
        {
            base.OnDisappearing();

            // Jeśli ViewModel implementuje IDisposable, wywołaj Dispose()
            if (this.viewModel is IDisposable disposable)
            {
                disposable.Dispose();
            }
        }

        // Obsługa zdarzenia Clicked przycisku edycji procedury
        private async void OnEditButtonClicked(object sender, EventArgs e)
        {
            try
            {
                if (sender is Button button && button.BindingContext is RealizedProcedureOldSMK procedure)
                {
                    System.Diagnostics.Debug.WriteLine($"OnEditButtonClicked: Edycja procedury ID={procedure.ProcedureId}");

                    // Użyj bezpośredniej nawigacji zamiast komendy w ViewModel
                    try
                    {
                        var navigationParameter = new Dictionary<string, object>
                {
                    { "ProcedureId", procedure.ProcedureId.ToString() }
                };

                        // Standardowa nawigacja Shell
                        await Shell.Current.GoToAsync("AddEditOldSMKProcedure", navigationParameter);
                    }
                    catch (Exception ex)
                    {
                        System.Diagnostics.Debug.WriteLine($"OnEditButtonClicked: Błąd nawigacji: {ex.Message}");
                        await DisplayAlert("Błąd", $"Wystąpił problem podczas edycji procedury: {ex.Message}", "OK");
                    }
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"OnEditButtonClicked: Ogólny błąd: {ex.Message}");
                await DisplayAlert("Błąd", $"Wystąpił problem podczas edycji procedury: {ex.Message}", "OK");
            }
        }

        // Obsługa zdarzenia Clicked przycisku usunięcia procedury
        private async void OnDeleteButtonClicked(object sender, EventArgs e)
        {
            try
            {
                if (sender is Button button && button.BindingContext is RealizedProcedureOldSMK procedure)
                {
                    System.Diagnostics.Debug.WriteLine($"OnDeleteButtonClicked: Usuwanie procedury ID={procedure.ProcedureId}");

                    // Pytamy użytkownika o potwierdzenie
                    bool confirm = await DisplayAlert(
                        "Potwierdzenie",
                        "Czy na pewno chcesz usunąć tę procedurę?",
                        "Tak",
                        "Nie");

                    if (confirm)
                    {
                        System.Diagnostics.Debug.WriteLine($"OnDeleteButtonClicked: Potwierdzono usunięcie procedury ID={procedure.ProcedureId}");

                        // Znajdź ProcedureGroupViewModel, który zawiera tę procedurę
                        if (button.Parent?.Parent?.BindingContext is RealizedProcedureOldSMK &&
                            button.Parent?.Parent?.Parent?.Parent?.Parent?.Parent?.Parent?.Parent?.BindingContext is ProcedureGroupViewModel groupViewModel)
                        {
                            // Wywołaj metodę usuwania z ViewModel
                            await groupViewModel.OnDeleteProcedure(procedure);
                        }
                        else
                        {
                            // Awaryjnie - użyj bezpośrednio serwisu procedur
                            bool success = await this.procedureService.DeleteOldSMKProcedureAsync(procedure.ProcedureId);

                            if (success)
                            {
                                System.Diagnostics.Debug.WriteLine($"OnDeleteButtonClicked: Pomyślnie usunięto procedurę");

                                // Odśwież dane
                                this.viewModel.RefreshCommand.Execute(null);

                                await DisplayAlert("Sukces", "Procedura została usunięta.", "OK");
                            }
                            else
                            {
                                System.Diagnostics.Debug.WriteLine($"OnDeleteButtonClicked: Nie udało się usunąć procedury");
                                await DisplayAlert("Błąd", "Nie udało się usunąć procedury. Spróbuj ponownie.", "OK");
                            }
                        }
                    }
                    else
                    {
                        System.Diagnostics.Debug.WriteLine($"OnDeleteButtonClicked: Anulowano usunięcie procedury");
                    }
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"OnDeleteButtonClicked: Błąd podczas usuwania procedury: {ex.Message}");
                await DisplayAlert("Błąd", $"Wystąpił problem podczas usuwania procedury: {ex.Message}", "OK");
            }
        }
    }
}
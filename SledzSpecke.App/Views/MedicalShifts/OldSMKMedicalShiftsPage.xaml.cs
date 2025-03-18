using SledzSpecke.App.Models;
using SledzSpecke.App.Services.MedicalShifts;
using SledzSpecke.App.ViewModels.MedicalShifts;

namespace SledzSpecke.App.Views.MedicalShifts
{
    public partial class OldSMKMedicalShiftsPage : ContentPage
    {
        private readonly OldSMKMedicalShiftsListViewModel viewModel;
        private readonly IMedicalShiftsService medicalShiftsService; // Dodane dla bezpośredniego dostępu w razie problemów z ViewModel

        public OldSMKMedicalShiftsPage(OldSMKMedicalShiftsListViewModel viewModel, IMedicalShiftsService medicalShiftsService)
        {
            this.InitializeComponent();
            this.viewModel = viewModel;
            this.medicalShiftsService = medicalShiftsService;
            this.BindingContext = this.viewModel;

            // Rejestracja zdarzeń dla zmiany kolekcji lat
            this.viewModel.PropertyChanged += this.ViewModel_PropertyChanged;
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

        protected override async void OnAppearing()
        {
            base.OnAppearing();
            System.Diagnostics.Debug.WriteLine("OldSMKMedicalShiftsPage.OnAppearing rozpoczęte");

            // Zapobiegamy wielokrotnemu wywołaniu w tej samej sesji
            if (!isFirstLoad)
            {
                System.Diagnostics.Debug.WriteLine("Pomijam ładowanie - to nie jest pierwsze ładowanie strony");
                return;
            }

            isFirstLoad = false;

            try
            {
                // Dajemy czas na załadowanie UI
                await Task.Delay(100);

                // Wywołujemy LoadYearsAsync bezpośrednio
                var method = this.viewModel.GetType().GetMethod("LoadYearsAsync",
                    System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);

                if (method != null)
                {
                    System.Diagnostics.Debug.WriteLine("OldSMKMedicalShiftsPage.OnAppearing: Bezpośrednio wywołuję LoadYearsAsync");
                    await (Task)method.Invoke(this.viewModel, null);

                    // Po zakończeniu ładowania, aktualizujemy przyciski lat
                    System.Diagnostics.Debug.WriteLine("OldSMKMedicalShiftsPage.OnAppearing: Aktualizuję przyciski lat");
                    this.CreateYearButtons();
                }
                else
                {
                    // Awaryjne wywołanie RefreshCommand
                    System.Diagnostics.Debug.WriteLine("OldSMKMedicalShiftsPage.OnAppearing: Nie znaleziono metody LoadYearsAsync, używam RefreshCommand");
                    this.viewModel.RefreshCommand.Execute(null);
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"OldSMKMedicalShiftsPage.OnAppearing: Błąd: {ex.Message}");
            }
        }

        // Dodaj na górze klasy
        private bool isFirstLoad = true;

        private void ViewModel_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            // Jeśli zmieniła się kolekcja lat lub wybrany rok, odśwież przyciski
            if (e.PropertyName == nameof(this.viewModel.AvailableYears) ||
                e.PropertyName == nameof(this.viewModel.SelectedYear))
            {
                this.CreateYearButtons();
            }
        }

        private void CreateYearButtons()
        {
            try
            {
                // Wyczyść wszystkie przyciski lat
                this.YearsContainer.Children.Clear();

                // Dodaj przyciski lat
                foreach (var year in this.viewModel.AvailableYears)
                {
                    var button = new Button
                    {
                        Text = $"Rok {year}",
                        HeightRequest = 40,
                        WidthRequest = 90,
                        Margin = new Thickness(5),
                        TextColor = Colors.White,
                        BackgroundColor = year == this.viewModel.SelectedYear ? Color.FromArgb("#0D759C") : Color.FromArgb("#547E9E")
                    };

                    // Dodaj komendę do przycisku
                    button.Clicked += (s, e) =>
                    {
                        System.Diagnostics.Debug.WriteLine($"Kliknięto przycisk roku: {year}");
                        this.viewModel.SelectYearCommand.Execute(year);
                    };

                    this.YearsContainer.Children.Add(button);
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Błąd w CreateYearButtons: {ex.Message}");
            }
        }

        // Nowa implementacja bezpośredniej obsługi zdarzeń
        private void OnAddButtonClicked(object sender, EventArgs e)
        {
            try
            {
                System.Diagnostics.Debug.WriteLine("OnAddButtonClicked: Wywołano bezpośrednio");
                this.viewModel.AddShiftCommand.Execute(null);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"OnAddButtonClicked: Błąd: {ex.Message}");
                DisplayAlert("Błąd", $"Wystąpił problem podczas dodawania dyżuru: {ex.Message}", "OK");
            }
        }

        private async void OnEditButtonClicked(object sender, EventArgs e)
        {
            try
            {
                if (sender is Button button && button.BindingContext is RealizedMedicalShiftOldSMK shift)
                {
                    System.Diagnostics.Debug.WriteLine($"OnEditButtonClicked: Edycja dyżuru ID={shift.ShiftId}");

                    // Użyj bezpośredniej nawigacji zamiast komendy w ViewModel
                    try
                    {
                        var navigationParameter = new Dictionary<string, object>
                        {
                            { "ShiftId", shift.ShiftId.ToString() },
                            { "YearParam", shift.Year.ToString() }
                        };

                        // Spróbujmy różne podejścia do nawigacji:
                        // 1. Podejście 1: Standardowa nawigacja Shell
                        await Shell.Current.GoToAsync("AddEditOldSMKMedicalShift", navigationParameter);
                    }
                    catch (Exception ex)
                    {
                        System.Diagnostics.Debug.WriteLine($"OnEditButtonClicked: Błąd nawigacji: {ex.Message}");

                        // 2. Podejście 2: Alternatywna ścieżka nawigacji
                        try
                        {
                            await Shell.Current.GoToAsync($"//medicalshifts/AddEditOldSMKMedicalShift?ShiftId={shift.ShiftId}&YearParam={shift.Year}");
                        }
                        catch (Exception ex2)
                        {
                            System.Diagnostics.Debug.WriteLine($"OnEditButtonClicked: Błąd drugiego podejścia do nawigacji: {ex2.Message}");

                            // 3. Podejście 3: Bezpośrednie tworzenie strony
                            try
                            {
                                var viewModel = IPlatformApplication.Current.Services.GetService<AddEditOldSMKMedicalShiftViewModel>();
                                var page = new AddEditOldSMKMedicalShiftPage(viewModel);

                                // Ustawienie parametrów
                                viewModel.ShiftId = shift.ShiftId.ToString();
                                viewModel.YearParam = shift.Year.ToString();

                                // Nawigacja
                                await Navigation.PushAsync(page);
                            }
                            catch (Exception ex3)
                            {
                                System.Diagnostics.Debug.WriteLine($"OnEditButtonClicked: Błąd trzeciego podejścia: {ex3.Message}");
                                await DisplayAlert("Błąd", "Nie można otworzyć edycji dyżuru. Spróbuj ponownie później.", "OK");
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"OnEditButtonClicked: Ogólny błąd: {ex.Message}");
                await DisplayAlert("Błąd", $"Wystąpił problem podczas edycji dyżuru: {ex.Message}", "OK");
            }
        }

        private async void OnDeleteInvoked(object sender, EventArgs e)
        {
            try
            {
                if (sender is SwipeItem swipeItem && swipeItem.BindingContext is RealizedMedicalShiftOldSMK shift)
                {
                    System.Diagnostics.Debug.WriteLine($"OnDeleteInvoked: Usuwanie dyżuru ID={shift.ShiftId}");

                    // Pytamy użytkownika o potwierdzenie
                    bool confirm = await DisplayAlert(
                        "Potwierdzenie",
                        "Czy na pewno chcesz usunąć ten dyżur?",
                        "Tak",
                        "Nie");

                    if (confirm)
                    {
                        System.Diagnostics.Debug.WriteLine($"OnDeleteInvoked: Potwierdzono usunięcie dyżuru ID={shift.ShiftId}");

                        // Bezpośrednie usunięcie z serwisu
                        bool success = await this.medicalShiftsService.DeleteOldSMKShiftAsync(shift.ShiftId);

                        if (success)
                        {
                            System.Diagnostics.Debug.WriteLine($"OnDeleteInvoked: Dyżur usunięty pomyślnie");

                            // Aktualizacja kolekcji w ViewModel
                            this.viewModel.Shifts.Remove(shift);

                            // Aktualizacja podsumowania
                            if (this.viewModel.SelectedYear > 0)
                            {
                                var summary = await this.medicalShiftsService.GetShiftsSummaryAsync(year: this.viewModel.SelectedYear);
                                this.viewModel.Summary = summary;
                            }

                            await DisplayAlert("Sukces", "Dyżur został usunięty", "OK");
                        }
                        else
                        {
                            System.Diagnostics.Debug.WriteLine($"OnDeleteInvoked: Nie udało się usunąć dyżuru");
                            await DisplayAlert("Błąd", "Nie udało się usunąć dyżuru. Spróbuj ponownie.", "OK");
                        }
                    }
                    else
                    {
                        System.Diagnostics.Debug.WriteLine($"OnDeleteInvoked: Anulowano usunięcie dyżuru");
                    }
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"OnDeleteInvoked: Błąd: {ex.Message}");
                await DisplayAlert("Błąd", $"Wystąpił problem podczas usuwania dyżuru: {ex.Message}", "OK");
            }
        }

        private void OnSwipeEnded(object sender, SwipeEndedEventArgs e)
        {
            System.Diagnostics.Debug.WriteLine($"OnSwipeEnded: SwipeDirection={e.SwipeDirection}");
        }
    }
}
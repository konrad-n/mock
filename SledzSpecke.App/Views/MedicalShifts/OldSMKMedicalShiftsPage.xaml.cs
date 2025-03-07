using SledzSpecke.App.ViewModels.MedicalShifts;

namespace SledzSpecke.App.Views.MedicalShifts
{
    public partial class OldSMKMedicalShiftsPage : ContentPage
    {
        private readonly OldSMKMedicalShiftsListViewModel viewModel;

        public OldSMKMedicalShiftsPage(OldSMKMedicalShiftsListViewModel viewModel)
        {
            this.InitializeComponent();
            this.viewModel = viewModel;
            this.BindingContext = this.viewModel;

            // Rejestracja zdarzeń dla zmiany kolekcji lat
            this.viewModel.PropertyChanged += this.ViewModel_PropertyChanged;
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();

            // Odśwież dane przy każdym pokazaniu strony
            this.viewModel.RefreshCommand.Execute(null);
        }

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
    }
}
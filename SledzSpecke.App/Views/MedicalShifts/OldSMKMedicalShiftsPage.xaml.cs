using SledzSpecke.App.Models;
using SledzSpecke.App.Services.MedicalShifts;
using SledzSpecke.App.ViewModels.MedicalShifts;

namespace SledzSpecke.App.Views.MedicalShifts
{
    public partial class OldSMKMedicalShiftsPage : ContentPage
    {
        private readonly OldSMKMedicalShiftsListViewModel viewModel;
        private readonly IMedicalShiftsService medicalShiftsService;

        public OldSMKMedicalShiftsPage(OldSMKMedicalShiftsListViewModel viewModel, IMedicalShiftsService medicalShiftsService)
        {
            this.InitializeComponent();
            this.viewModel = viewModel;
            this.medicalShiftsService = medicalShiftsService;
            this.BindingContext = this.viewModel;

            this.viewModel.PropertyChanged += this.ViewModel_PropertyChanged;
        }

        protected override void OnNavigatedTo(NavigatedToEventArgs args)
        {
            base.OnNavigatedTo(args);

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

            if (!isFirstLoad)
            {
                return;
            }

            isFirstLoad = false;

            await Task.Delay(100);

            var method = this.viewModel.GetType().GetMethod("LoadYearsAsync",
                System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);

            if (method != null)
            {
                await (Task)method.Invoke(this.viewModel, null);
                this.CreateYearButtons();
            }
            else
            {
                this.viewModel.RefreshCommand.Execute(null);
            }
        }

        private bool isFirstLoad = true;

        private void ViewModel_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(this.viewModel.AvailableYears) ||
                e.PropertyName == nameof(this.viewModel.SelectedYear))
            {
                this.CreateYearButtons();
            }
        }

        private void CreateYearButtons()
        {
            this.YearsContainer.Children.Clear();

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

                button.Clicked += (s, e) =>
                {
                    this.viewModel.SelectYearCommand.Execute(year);
                };

                this.YearsContainer.Children.Add(button);
            }
        }

        private void OnAddButtonClicked(object sender, EventArgs e)
        {
            this.viewModel.AddShiftCommand.Execute(null);
        }

        private async void OnEditButtonClicked(object sender, EventArgs e)
        {
            if (sender is Button button && button.BindingContext is RealizedMedicalShiftOldSMK shift)
            {

                try
                {
                    var navigationParameter = new Dictionary<string, object>
                    {
                        { "ShiftId", shift.ShiftId.ToString() },
                        { "YearParam", shift.Year.ToString() }
                    };

                    await Shell.Current.GoToAsync("AddEditOldSMKMedicalShift", navigationParameter);
                }
                catch (Exception ex)
                {
                    try
                    {
                        await Shell.Current.GoToAsync($"//medicalshifts/AddEditOldSMKMedicalShift?ShiftId={shift.ShiftId}&YearParam={shift.Year}");
                    }
                    catch (Exception ex2)
                    {
                        var viewModel = IPlatformApplication.Current.Services.GetService<AddEditOldSMKMedicalShiftViewModel>();
                        var page = new AddEditOldSMKMedicalShiftPage(viewModel);
                        viewModel.ShiftId = shift.ShiftId.ToString();
                        viewModel.YearParam = shift.Year.ToString();
                        await Navigation.PushAsync(page);
                    }
                }
            }
        }

        private async void OnDeleteInvoked(object sender, EventArgs e)
        {
            if (sender is SwipeItem swipeItem && swipeItem.BindingContext is RealizedMedicalShiftOldSMK shift)
            {
                bool confirm = await DisplayAlert(
                    "Potwierdzenie",
                    "Czy na pewno chcesz usunąć ten dyżur?",
                    "Tak",
                    "Nie");

                if (confirm)
                {
                    bool success = await this.medicalShiftsService.DeleteOldSMKShiftAsync(shift.ShiftId);

                    if (success)
                    {
                        this.viewModel.Shifts.Remove(shift);

                        if (this.viewModel.SelectedYear > 0)
                        {
                            var summary = await this.medicalShiftsService.GetShiftsSummaryAsync(year: this.viewModel.SelectedYear);
                            this.viewModel.Summary = summary;
                        }

                        await DisplayAlert("Sukces", "Dyżur został usunięty", "OK");
                    }
                    else
                    {
                        await DisplayAlert("Błąd", "Nie udało się usunąć dyżuru. Spróbuj ponownie.", "OK");
                    }
                }
            }
        }

        private void OnSwipeEnded(object sender, SwipeEndedEventArgs e)
        {
        }
    }
}
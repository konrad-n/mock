using SledzSpecke.Core.Models;
using SledzSpecke.Core.Models.Enums;

namespace SledzSpecke.App.Views
{
    public partial class DutyShiftsPage : ContentPage
    {
        private List<DutyShift> _dutyShifts;
        private double _totalRequiredHours;

        public DutyShiftsPage()
        {
            InitializeComponent();
            _dutyShifts = new List<DutyShift>();
            LoadDataAsync();
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();
            LoadDataAsync();
        }

        private async void LoadDataAsync()
        {
            try
            {
                // Get all duty shifts from the database
                _dutyShifts = await App.DutyShiftService.GetAllDutyShiftsAsync();

                // Get specialization for required hours
                var specialization = await App.SpecializationService.GetSpecializationAsync();
                _totalRequiredHours = specialization.RequiredDutyHoursPerWeek * (specialization.BaseDurationWeeks / 52.0) * 52;

                // Get weekly average - calculate from data not from service
                double weeklyAverage = CalculateWeeklyAverage();

                // Update UI
                UpdateTotalHours();
                DisplayDutyShifts();

                // Update weekly hours label
                WeeklyHoursLabel.Text = $"{weeklyAverage:F1}h";
            }
            catch (Exception ex)
            {
                await DisplayAlert("Błąd", $"Nie udało się załadować dyżurów: {ex.Message}", "OK");
                System.Diagnostics.Debug.WriteLine($"Error loading duty shifts: {ex}");
            }
        }

        private double CalculateWeeklyAverage()
        {
            try
            {
                if (_dutyShifts == null || _dutyShifts.Count == 0)
                    return 0;

                // Count actual weeks with duties
                var dates = _dutyShifts.Select(d => d.StartDate.Date).Distinct().OrderBy(d => d).ToList();
                if (dates.Count == 0)
                    return 0;

                DateTime firstDate = dates.First();
                DateTime lastDate = dates.Last();

                // Calculate number of weeks between first and last duty
                double weeks = Math.Max(1, (lastDate - firstDate).TotalDays / 7);

                // Calculate total hours
                double totalHours = _dutyShifts.Sum(d => d.DurationHours);

                // Return weekly average
                return totalHours / weeks;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error calculating weekly average: {ex}");
                return 0;
            }
        }

        private void UpdateTotalHours()
        {
            try
            {
                double totalHours = _dutyShifts?.Sum(d => d.DurationHours) ?? 0;
                TotalHoursLabel.Text = $"{totalHours:F1}/{_totalRequiredHours:F0} godzin";

                // Update color based on progress
                if (totalHours >= _totalRequiredHours)
                {
                    TotalHoursLabel.TextColor = Colors.Green;
                }
                else if (totalHours >= _totalRequiredHours * 0.75)
                {
                    TotalHoursLabel.TextColor = Colors.Orange;
                }
                else
                {
                    TotalHoursLabel.TextColor = Colors.DarkBlue;
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error updating total hours: {ex}");
                TotalHoursLabel.Text = $"0.0/{_totalRequiredHours:F0} godzin";
            }
        }

        private void DisplayDutyShifts()
        {
            try
            {
                DutyShiftsLayout.Children.Clear();

                if (_dutyShifts == null || _dutyShifts.Count == 0)
                {
                    var emptyLabel = new Label
                    {
                        Text = "Brak dyżurów. Kliknij '+' aby dodać nowy dyżur.",
                        HorizontalOptions = LayoutOptions.Center,
                        VerticalOptions = LayoutOptions.Center,
                        Margin = new Thickness(0, 20, 0, 0)
                    };
                    DutyShiftsLayout.Children.Add(emptyLabel);
                    return;
                }

                // Group duty shifts by month and year
                var dutyShiftsByMonth = _dutyShifts
                    .OrderByDescending(d => d.StartDate)
                    .GroupBy(d => new { Year = d.StartDate.Year, Month = d.StartDate.Month })
                    .ToList();

                foreach (var monthGroup in dutyShiftsByMonth)
                {
                    // Month header
                    var monthHeader = new Label
                    {
                        Text = $"{GetMonthName(monthGroup.Key.Month)} {monthGroup.Key.Year}",
                        FontSize = 18,
                        FontAttributes = FontAttributes.Bold,
                        Margin = new Thickness(0, 10, 0, 5)
                    };
                    DutyShiftsLayout.Children.Add(monthHeader);

                    // Monthly total hours
                    var totalMonthHours = monthGroup.Sum(d => d.DurationHours);
                    var monthTotalLabel = new Label
                    {
                        Text = $"Łącznie: {totalMonthHours:F1} godzin",
                        FontSize = 14,
                        Margin = new Thickness(0, 0, 0, 10)
                    };
                    DutyShiftsLayout.Children.Add(monthTotalLabel);

                    foreach (var dutyShift in monthGroup)
                    {
                        var frame = new Frame
                        {
                            Padding = new Thickness(10),
                            Margin = new Thickness(0, 0, 0, 10),
                            CornerRadius = 5,
                            BorderColor = dutyShift.Type == DutyType.Independent ? Colors.DarkBlue : Colors.DarkGreen
                        };

                        var dateLabel = new Label
                        {
                            Text = $"{dutyShift.StartDate.ToString("dd.MM.yyyy HH:mm")} - {dutyShift.EndDate.ToString("dd.MM.yyyy HH:mm")}",
                            FontAttributes = FontAttributes.Bold,
                            FontSize = 16
                        };

                        var typeLabel = new Label
                        {
                            Text = dutyShift.Type == DutyType.Independent ? "Samodzielny" : "Towarzyszący",
                            FontSize = 14,
                            TextColor = dutyShift.Type == DutyType.Independent ? Colors.DarkBlue : Colors.DarkGreen
                        };

                        var durationLabel = new Label
                        {
                            Text = $"Czas trwania: {dutyShift.DurationHours:F1} godzin",
                            FontSize = 14
                        };

                        var locationLabel = new Label
                        {
                            Text = $"Miejsce: {dutyShift.Location}",
                            FontSize = 14
                        };

                        var supervisorLabel = new Label
                        {
                            Text = $"Opiekun: {dutyShift.SupervisorName ?? "Brak"}",
                            FontSize = 14,
                            IsVisible = dutyShift.Type == DutyType.Accompanied && !string.IsNullOrEmpty(dutyShift.SupervisorName)
                        };

                        var editButton = new Button
                        {
                            Text = "Edytuj",
                            HeightRequest = 35,
                            FontSize = 14,
                            Margin = new Thickness(0, 5, 0, 0),
                            CommandParameter = dutyShift.Id
                        };
                        editButton.Clicked += OnEditDutyShiftClicked;

                        var contentLayout = new VerticalStackLayout
                        {
                            Children = { dateLabel, typeLabel, durationLabel, locationLabel }
                        };

                        if (supervisorLabel.IsVisible)
                            contentLayout.Children.Add(supervisorLabel);

                        contentLayout.Children.Add(editButton);

                        frame.Content = contentLayout;
                        DutyShiftsLayout.Children.Add(frame);
                    }
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error displaying duty shifts: {ex}");
                var errorLabel = new Label
                {
                    Text = "Wystąpił błąd podczas wyświetlania dyżurów.",
                    HorizontalOptions = LayoutOptions.Center,
                    VerticalOptions = LayoutOptions.Center,
                    Margin = new Thickness(0, 20, 0, 0),
                    TextColor = Colors.Red
                };
                DutyShiftsLayout.Children.Clear();
                DutyShiftsLayout.Children.Add(errorLabel);
            }
        }

        private string GetMonthName(int month)
        {
            return month switch
            {
                1 => "Styczeń",
                2 => "Luty",
                3 => "Marzec",
                4 => "Kwiecień",
                5 => "Maj",
                6 => "Czerwiec",
                7 => "Lipiec",
                8 => "Sierpień",
                9 => "Wrzesień",
                10 => "Październik",
                11 => "Listopad",
                12 => "Grudzień",
                _ => "Nieznany"
            };
        }

        private async void OnAddDutyShiftClicked(object sender, EventArgs e)
        {
            try
            {
                await Navigation.PushAsync(new DutyShiftDetailsPage(null, OnDutyShiftSaved));
            }
            catch (Exception ex)
            {
                await DisplayAlert("Błąd", $"Nie udało się otworzyć strony dodawania dyżuru: {ex.Message}", "OK");
                System.Diagnostics.Debug.WriteLine($"Error navigating to DutyShiftDetailsPage: {ex}");
            }
        }

        private async void OnEditDutyShiftClicked(object sender, EventArgs e)
        {
            try
            {
                if (sender is Button button && button.CommandParameter is int dutyShiftId)
                {
                    var dutyShift = await App.DutyShiftService.GetDutyShiftAsync(dutyShiftId);
                    if (dutyShift != null)
                    {
                        await Navigation.PushAsync(new DutyShiftDetailsPage(dutyShift, OnDutyShiftSaved));
                    }
                }
            }
            catch (Exception ex)
            {
                await DisplayAlert("Błąd", $"Nie udało się otworzyć strony edycji dyżuru: {ex.Message}", "OK");
                System.Diagnostics.Debug.WriteLine($"Error navigating to edit DutyShiftDetailsPage: {ex}");
            }
        }

        private async Task OnDutyShiftSaved(DutyShift dutyShift)
        {
            try
            {
                // Save to database
                await App.DutyShiftService.SaveDutyShiftAsync(dutyShift);

                // Refresh data
                await LoadDataAsyncTask();
            }
            catch (Exception ex)
            {
                await DisplayAlert("Błąd", $"Nie udało się zapisać dyżuru: {ex.Message}", "OK");
                System.Diagnostics.Debug.WriteLine($"Error saving duty shift: {ex}");
            }
        }

        // Asynchronous method that returns a Task for better handling
        private async Task LoadDataAsyncTask()
        {
            try
            {
                // Get all duty shifts from the database
                _dutyShifts = await App.DutyShiftService.GetAllDutyShiftsAsync();

                // Get specialization for required hours
                var specialization = await App.SpecializationService.GetSpecializationAsync();
                _totalRequiredHours = specialization.RequiredDutyHoursPerWeek * (specialization.BaseDurationWeeks / 52.0) * 52;

                // Get weekly average - calculate from data not from service
                double weeklyAverage = CalculateWeeklyAverage();

                // Update UI on main thread
                MainThread.BeginInvokeOnMainThread(() => {
                    // Update total hours
                    UpdateTotalHours();

                    // Display duty shifts
                    DisplayDutyShifts();

                    // Update weekly hours label
                    WeeklyHoursLabel.Text = $"{weeklyAverage:F1}h";
                });
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error loading duty shifts: {ex}");

                // Show error on main thread
                MainThread.BeginInvokeOnMainThread(async () => {
                    await DisplayAlert("Błąd", $"Nie udało się załadować dyżurów: {ex.Message}", "OK");
                });
            }
        }
    }
}
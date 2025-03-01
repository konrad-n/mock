using Microsoft.Extensions.Logging;
using SledzSpecke.Core.Models;
using System.Collections.Generic;
using System.Linq;

namespace SledzSpecke.App.Views
{
    public partial class AbsenceManagementPage : ContentPage
    {
        private Specialization _specialization;
        private List<Absence> _allAbsences = new List<Absence>();
        private List<Absence> _filteredAbsences = new List<Absence>();
        private AbsenceType? _selectedAbsenceType = null;
        private int? _selectedYear = null;

        public AbsenceManagementPage()
        {
            InitializeComponent();
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
                // Load specialization data
                _specialization = await App.SpecializationService.GetSpecializationAsync();

                // Calculate dates

                DateTime plannedEndDate = _specialization.StartDate.AddDays(_specialization.BaseDurationWeeks * 7);
                DateTime actualEndDate = await App.SpecializationDateCalculator.CalculateExpectedEndDateAsync(_specialization.Id);

                // Update UI with dates
                PlannedEndDateLabel.Text = plannedEndDate.ToString("dd.MM.yyyy");
                ActualEndDateLabel.Text = actualEndDate.ToString("dd.MM.yyyy");

                // Calculate self-education days
                int currentYear = DateTime.Now.Year;
                int remainingSelfEducationDays = await App.SpecializationDateCalculator.GetRemainingEducationDaysForYearAsync(_specialization.Id, currentYear);
                int usedSelfEducationDays = _specialization.SelfEducationDaysPerYear - remainingSelfEducationDays;

                SelfEducationDaysLabel.Text = $"{usedSelfEducationDays}/{_specialization.SelfEducationDaysPerYear}";

                // Load absences
                _allAbsences = await App.DatabaseService.QueryAsync<Absence>(
                    "SELECT * FROM Absences WHERE SpecializationId = ? ORDER BY StartDate DESC",
                    _specialization.Id);

                // Update total absence days
                int totalAbsenceDays = _allAbsences.Sum(a => a.DurationDays);
                TotalAbsenceDaysLabel.Text = totalAbsenceDays.ToString();

                // Setup filter year picker
                SetupYearFilterPicker();

                // Apply filters and display absences
                ApplyFiltersAndDisplayAbsences();
            }
            catch (Exception ex)
            {
                await DisplayAlert("Błąd", $"Nie udało się załadować danych: {ex.Message}", "OK");
            }
        }

        private void SetupYearFilterPicker()
        {
            // Clear existing items
            FilterYearPicker.Items.Clear();

            // Add "All years" option
            FilterYearPicker.Items.Add("Wszystkie lata");

            // Get unique years from absences
            var years = _allAbsences
                .Select(a => a.Year)
                .Distinct()
                .OrderBy(y => y)
                .ToList();

            // Add years to picker
            foreach (var year in years)
            {
                FilterYearPicker.Items.Add(year.ToString());
            }

            // Select "All years" by default
            FilterYearPicker.SelectedIndex = 0;
        }

        private void ApplyFiltersAndDisplayAbsences()
        {
            // Apply filters
            _filteredAbsences = _allAbsences;

            // Filter by type if selected
            if (_selectedAbsenceType.HasValue)
            {
                _filteredAbsences = _filteredAbsences
                    .Where(a => a.Type == _selectedAbsenceType.Value)
                    .ToList();
            }
            else if (FilterTypePicker.SelectedIndex > 0)
            {
                // Map picker index to absence type
                AbsenceType? type = FilterTypePicker.SelectedIndex switch
                {
                    1 => AbsenceType.SickLeave,
                    2 => AbsenceType.VacationLeave,
                    3 => AbsenceType.SelfEducationLeave,
                    4 => null, // Special case for maternity/parental leaves
                    _ => null
                };

                if (type.HasValue)
                {
                    _filteredAbsences = _filteredAbsences
                        .Where(a => a.Type == type.Value)
                        .ToList();
                }
                else if (FilterTypePicker.SelectedIndex == 4)
                {
                    // Handle maternity/parental leaves
                    _filteredAbsences = _filteredAbsences
                        .Where(a => a.Type == AbsenceType.MaternityLeave || a.Type == AbsenceType.ParentalLeave)
                        .ToList();
                }
                else if (FilterTypePicker.SelectedIndex == 5)
                {
                    // Handle other leaves
                    _filteredAbsences = _filteredAbsences
                        .Where(a => a.Type == AbsenceType.SpecialLeave ||
                                   a.Type == AbsenceType.UnpaidLeave ||
                                   a.Type == AbsenceType.Other)
                        .ToList();
                }
            }

            // Filter by year if selected
            if (_selectedYear.HasValue)
            {
                _filteredAbsences = _filteredAbsences
                    .Where(a => a.Year == _selectedYear.Value)
                    .ToList();
            }
            else if (FilterYearPicker.SelectedIndex > 0)
            {
                int selectedYear;
                if (int.TryParse(FilterYearPicker.Items[FilterYearPicker.SelectedIndex], out selectedYear))
                {
                    _filteredAbsences = _filteredAbsences
                        .Where(a => a.Year == selectedYear)
                        .ToList();
                }
            }

            // Display filtered absences
            DisplayAbsences();
        }

        private void DisplayAbsences()
        {
            // Clear previous list
            // Remove all children except the filter frame and no absences label
            while (AbsencesLayout.Children.Count > 2)
            {
                AbsencesLayout.Children.RemoveAt(2);
            }

            // Show/hide no absences label
            NoAbsencesLabel.IsVisible = _filteredAbsences.Count == 0;

            // Create absence cards
            foreach (var absence in _filteredAbsences)
            {
                var frame = CreateAbsenceCard(absence);
                AbsencesLayout.Children.Add(frame);
            }
        }

        private Frame CreateAbsenceCard(Absence absence)
        {
            // Determine card style based on absence type
            Color cardColor;
            string iconText;

            switch (absence.Type)
            {
                case AbsenceType.SickLeave:
                    cardColor = Color.FromArgb("#FFE0E0");
                    iconText = "🤒";
                    break;
                case AbsenceType.VacationLeave:
                    cardColor = Color.FromArgb("#E0F7FA");
                    iconText = "🏖️";
                    break;
                case AbsenceType.SelfEducationLeave:
                    cardColor = Color.FromArgb("#E8F5E9");
                    iconText = "📚";
                    break;
                case AbsenceType.MaternityLeave:
                case AbsenceType.ParentalLeave:
                    cardColor = Color.FromArgb("#FFF8E1");
                    iconText = "👶";
                    break;
                default:
                    cardColor = Color.FromArgb("#F5F5F5");
                    iconText = "📅";
                    break;
            }

            // Create card
            var frame = new Frame
            {
                BackgroundColor = cardColor,
                BorderColor = Color.FromArgb("#E0E0E0"),
                CornerRadius = 10,
                Margin = new Thickness(0, 0, 0, 10),
                Padding = new Thickness(15)
            };

            // Create card content
            var grid = new Grid
            {
                ColumnDefinitions = new ColumnDefinitionCollection
                {
                    new ColumnDefinition { Width = new GridLength(40) },
                    new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) },
                    new ColumnDefinition { Width = new GridLength(40) }
                }
            };

            // Icon
            var iconLabel = new Label
            {
                Text = iconText,
                FontSize = 24,
                HorizontalOptions = LayoutOptions.Center,
                VerticalOptions = LayoutOptions.Center
            };
            grid.Add(iconLabel, 0, 0);

            // Content
            var contentLayout = new VerticalStackLayout
            {
                Spacing = 5
            };

            // Title with date range
            var titleLabel = new Label
            {
                Text = GetAbsenceTypeText(absence.Type),
                FontSize = 16,
                FontAttributes = FontAttributes.Bold,
                TextColor = Color.FromArgb("#212121")
            };
            contentLayout.Add(titleLabel);

            // Date range
            var dateLabel = new Label
            {
                Text = $"{absence.StartDate:dd.MM.yyyy} - {absence.EndDate:dd.MM.yyyy} ({absence.DurationDays} dni)",
                FontSize = 14,
                TextColor = Color.FromArgb("#757575")
            };
            contentLayout.Add(dateLabel);

            // Description if available
            if (!string.IsNullOrEmpty(absence.Description))
            {
                var descriptionLabel = new Label
                {
                    Text = absence.Description,
                    FontSize = 14,
                    TextColor = Color.FromArgb("#757575")
                };
                contentLayout.Add(descriptionLabel);
            }

            // Add indicator if it affects specialization length
            if (absence.AffectsSpecializationLength)
            {
                var effectLabel = new Label
                {
                    Text = "Wydłuża specjalizację",
                    FontSize = 12,
                    TextColor = Color.FromArgb("#F44336"),
                    FontAttributes = FontAttributes.Italic
                };
                contentLayout.Add(effectLabel);
            }

            grid.Add(contentLayout, 1, 0);

            // Edit button
            var editButton = new Button
            {
                Text = "✏️",
                FontSize = 18,
                BackgroundColor = Colors.Transparent,
                BorderColor = Colors.Transparent,
                TextColor = Color.FromArgb("#0D759C"),
                Padding = new Thickness(0),
                HeightRequest = 40,
                WidthRequest = 40,
                HorizontalOptions = LayoutOptions.End,
                VerticalOptions = LayoutOptions.Start,
                CommandParameter = absence.Id
            };
            editButton.Clicked += OnEditAbsenceClicked;
            grid.Add(editButton, 2, 0);

            frame.Content = grid;

            // Add tap gesture
            var tapGesture = new TapGestureRecognizer
            {
                CommandParameter = absence.Id
            };
            tapGesture.Tapped += OnAbsenceTapped;
            frame.GestureRecognizers.Add(tapGesture);

            return frame;
        }

        private string GetAbsenceTypeText(AbsenceType type)
        {
            return type switch
            {
                AbsenceType.SickLeave => "Zwolnienie lekarskie (L4)",
                AbsenceType.VacationLeave => "Urlop wypoczynkowy",
                AbsenceType.SelfEducationLeave => "Urlop szkoleniowy (samokształcenie)",
                AbsenceType.MaternityLeave => "Urlop macierzyński",
                AbsenceType.ParentalLeave => "Urlop rodzicielski",
                AbsenceType.SpecialLeave => "Urlop okolicznościowy",
                AbsenceType.UnpaidLeave => "Urlop bezpłatny",
                AbsenceType.Other => "Inna nieobecność",
                _ => "Nieobecność"
            };
        }

        private void OnFilterTypeChanged(object sender, EventArgs e)
        {
            ApplyFiltersAndDisplayAbsences();
        }

        private void OnFilterYearChanged(object sender, EventArgs e)
        {
            ApplyFiltersAndDisplayAbsences();
        }

        private async void OnAddAbsenceClicked(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new AbsenceDetailsPage(null, OnAbsenceAdded));
        }

        private async void OnEditAbsenceClicked(object sender, EventArgs e)
        {
            if (sender is Button button && button.CommandParameter is int absenceId)
            {
                var absence = _allAbsences.FirstOrDefault(a => a.Id == absenceId);
                if (absence != null)
                {
                    await Navigation.PushAsync(new AbsenceDetailsPage(absence, OnAbsenceUpdated));
                }
            }
        }

        private async void OnAbsenceTapped(object sender, EventArgs e)
        {
            if (sender is TapGestureRecognizer tapGesture && tapGesture.CommandParameter is int absenceId)
            {
                var absence = _allAbsences.FirstOrDefault(a => a.Id == absenceId);
                if (absence != null)
                {
                    await Navigation.PushAsync(new AbsenceDetailsPage(absence, OnAbsenceUpdated));
                }
            }
        }

        private async void OnAbsenceAdded(Absence absence)
        {
            // Save absence
            absence.SpecializationId = _specialization.Id;
            await App.DatabaseService.SaveAsync(absence);

            // Reload data
            LoadDataAsync();
        }

        private async void OnAbsenceUpdated(Absence absence)
        {
            // Update absence
            absence.SpecializationId = _specialization.Id;
            await App.DatabaseService.SaveAsync(absence);

            // Reload data
            LoadDataAsync();
        }
    }
}
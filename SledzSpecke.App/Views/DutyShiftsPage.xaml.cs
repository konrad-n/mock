using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Maui.Controls;
using SledzSpecke.Core.Models;

namespace SledzSpecke.Views
{
    public partial class DutyShiftsPage : ContentPage
    {
        // Przykładowa lista dyżurów dla demonstracji
        private List<DutyShift> _dutyShifts = new List<DutyShift>
{
new DutyShift
{
Id = 1,
StartDate = DateTime.Now.AddDays(-30),
EndDate = DateTime.Now.AddDays(-29).AddHours(10).AddMinutes(5),
DurationHours = 10 + (5/60.0),
Type = DutyType.Accompanied,
Location = "Oddział Hematologii",
SupervisorName = "Dr Jan Kowalski"
},
new DutyShift
{
Id = 2,
StartDate = DateTime.Now.AddDays(-25),
EndDate = DateTime.Now.AddDays(-24).AddHours(10).AddMinutes(5),
DurationHours = 10 + (5/60.0),
Type = DutyType.Accompanied,
Location = "Oddział Hematologii",
SupervisorName = "Dr Jan Kowalski"
},
new DutyShift
{
Id = 3,
StartDate = DateTime.Now.AddDays(-20),
EndDate = DateTime.Now.AddDays(-19).AddHours(10).AddMinutes(5),
DurationHours = 10 + (5/60.0),
Type = DutyType.Independent,
Location = "Oddział Hematologii"
},
new DutyShift
{
Id = 4,
StartDate = DateTime.Now.AddDays(-15),
EndDate = DateTime.Now.AddDays(-14).AddHours(10).AddMinutes(5),
DurationHours = 10 + (5/60.0),
Type = DutyType.Independent,
Location = "Oddział Hematologii"
},
new DutyShift
{
Id = 5,
StartDate = DateTime.Now.AddDays(-10),
EndDate = DateTime.Now.AddDays(-9).AddHours(10).AddMinutes(5),
DurationHours = 10 + (5/60.0),
Type = DutyType.Independent,
Location = "Oddział Hematologii"
}
};



        public DutyShiftsPage()
        {
            InitializeComponent();
            UpdateTotalHours();
            DisplayDutyShifts();
        }

        private void UpdateTotalHours()
        {
            double totalHours = _dutyShifts.Sum(d => d.DurationHours);
            // Załóżmy, że wymagane jest 520 godzin dyżurów w trakcie specjalizacji
            TotalHoursLabel.Text = $"{totalHours:F1}/520 godzin";
        }

        private void DisplayDutyShifts()
        {
            DutyShiftsLayout.Children.Clear();

            // Grupowanie dyżurów po miesiącach
            var dutyShiftsByMonth = _dutyShifts
                .OrderByDescending(d => d.StartDate)
                .GroupBy(d => new { d.StartDate.Year, d.StartDate.Month })
                .ToList();

            foreach (var monthGroup in dutyShiftsByMonth)
            {
                // Nagłówek miesiąca
                var monthHeader = new Label
                {
                    Text = $"{GetMonthName(monthGroup.Key.Month)} {monthGroup.Key.Year}",
                    FontSize = 18,
                    FontAttributes = FontAttributes.Bold,
                    Margin = new Thickness(0, 10, 0, 5)
                };
                DutyShiftsLayout.Children.Add(monthHeader);

                // Łączna liczba godzin w miesiącu
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
            await Navigation.PushAsync(new DutyShiftDetailsPage(null, OnDutyShiftAdded));
        }

        private async void OnEditDutyShiftClicked(object sender, EventArgs e)
        {
            if (sender is Button button && button.CommandParameter is int dutyShiftId)
            {
                var dutyShift = _dutyShifts.FirstOrDefault(d => d.Id == dutyShiftId);
                if (dutyShift != null)
                {
                    await Navigation.PushAsync(new DutyShiftDetailsPage(dutyShift, OnDutyShiftUpdated));
                }
            }
        }

        private void OnDutyShiftAdded(DutyShift dutyShift)
        {
            // Generowanie nowego ID
            dutyShift.Id = _dutyShifts.Count > 0 ? _dutyShifts.Max(d => d.Id) + 1 : 1;
            _dutyShifts.Add(dutyShift);
            UpdateTotalHours();
            DisplayDutyShifts();
        }

        private void OnDutyShiftUpdated(DutyShift dutyShift)
        {
            var existingDutyShift = _dutyShifts.FirstOrDefault(d => d.Id == dutyShift.Id);
            if (existingDutyShift != null)
            {
                var index = _dutyShifts.IndexOf(existingDutyShift);
                _dutyShifts[index] = dutyShift;
            }
            UpdateTotalHours();
            DisplayDutyShifts();
        }
    }
}
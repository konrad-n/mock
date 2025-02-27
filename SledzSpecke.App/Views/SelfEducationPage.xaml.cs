using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Maui.Controls;
using SledzSpecke.Core.Models;

namespace SledzSpecke.Views
{
    public partial class SelfEducationPage : ContentPage
    {
        // Przykładowa lista wydarzeń samokształcenia dla demonstracji
        private List<SelfEducation> _selfEducationList = new List<SelfEducation>
{
new SelfEducation
{
Id = 1,
Title = "Konferencja Polskiego Towarzystwa Hematologów i Transfuzjologów",
Type = SelfEducationType.Conference,
StartDate = DateTime.Now.AddMonths(-5),
EndDate = DateTime.Now.AddMonths(-5).AddDays(2),
DurationDays = 3,
Location = "Warszawa",
Organizer = "PTHiT",
IsRequired = true,
Notes = "Udział w sesji dotyczącej nowotworów hematologicznych"
},
new SelfEducation
{
Id = 2,
Title = "Warsztaty diagnostyki cytologicznej szpiku",
Type = SelfEducationType.Workshop,
StartDate = DateTime.Now.AddMonths(-3),
EndDate = DateTime.Now.AddMonths(-3).AddDays(1),
DurationDays = 2,
Location = "Kraków",
Organizer = "Instytut Hematologii",
IsRequired = false,
Notes = "Praktyczne warsztaty z oceny rozmazów szpiku"
}
};



        public SelfEducationPage()
        {
            InitializeComponent();
            UpdateUsedDays();
            DisplaySelfEducation();
        }

        private void UpdateUsedDays()
        {
            int usedDays = _selfEducationList.Sum(s => s.DurationDays);
            // Załóżmy, że przysługuje 18 dni samokształcenia (6 dni rocznie przez 3 lata)
            UsedDaysLabel.Text = $"{usedDays}/18";
        }

        private void DisplaySelfEducation()
        {
            SelfEducationLayout.Children.Clear();

            // Grupowanie wydarzeń po roku
            var educationByYear = _selfEducationList
                .OrderByDescending(s => s.StartDate)
                .GroupBy(s => s.StartDate.Year)
                .ToList();

            foreach (var yearGroup in educationByYear)
            {
                // Nagłówek roku
                var yearHeader = new Label
                {
                    Text = $"Rok {yearGroup.Key}",
                    FontSize = 18,
                    FontAttributes = FontAttributes.Bold,
                    Margin = new Thickness(0, 10, 0, 5)
                };
                SelfEducationLayout.Children.Add(yearHeader);

                // Łączna liczba dni w roku
                var totalYearDays = yearGroup.Sum(s => s.DurationDays);
                var yearTotalLabel = new Label
                {
                    Text = $"Wykorzystano: {totalYearDays}/6 dni",
                    FontSize = 14,
                    Margin = new Thickness(0, 0, 0, 10)
                };
                SelfEducationLayout.Children.Add(yearTotalLabel);

                foreach (var education in yearGroup)
                {
                    var frame = new Frame
                    {
                        Padding = new Thickness(10),
                        Margin = new Thickness(0, 0, 0, 10),
                        CornerRadius = 5,
                        BorderColor = education.IsRequired ? Colors.DarkBlue : Colors.DarkGreen
                    };

                    var titleLabel = new Label
                    {
                        Text = education.Title,
                        FontAttributes = FontAttributes.Bold,
                        FontSize = 16
                    };

                    var typeLabel = new Label
                    {
                        Text = GetSelfEducationTypeName(education.Type),
                        FontSize = 14,
                        TextColor = education.IsRequired ? Colors.DarkBlue : Colors.DarkGreen
                    };

                    var dateLabel = new Label
                    {
                        Text = $"{education.StartDate.ToString("dd.MM.yyyy")} - {education.EndDate.ToString("dd.MM.yyyy")} ({education.DurationDays} dni)",
                        FontSize = 14
                    };

                    var locationLabel = new Label
                    {
                        Text = $"Miejsce: {education.Location}",
                        FontSize = 14
                    };

                    var organizerLabel = new Label
                    {
                        Text = $"Organizator: {education.Organizer}",
                        FontSize = 14
                    };

                    var requiredLabel = new Label
                    {
                        Text = education.IsRequired ? "Wymagane w programie specjalizacji" : "Dodatkowe",
                        FontSize = 12,
                        TextColor = education.IsRequired ? Colors.DarkBlue : Colors.Gray
                    };

                    var editButton = new Button
                    {
                        Text = "Edytuj",
                        HeightRequest = 35,
                        FontSize = 14,
                        Margin = new Thickness(0, 5, 0, 0),
                        CommandParameter = education.Id
                    };
                    editButton.Clicked += OnEditSelfEducationClicked;

                    var contentLayout = new VerticalStackLayout
                    {
                        Children = { titleLabel, typeLabel, dateLabel, locationLabel, organizerLabel, requiredLabel, editButton }
                    };

                    frame.Content = contentLayout;
                    SelfEducationLayout.Children.Add(frame);
                }
            }
        }

        private string GetSelfEducationTypeName(SelfEducationType type)
        {
            return type switch
            {
                SelfEducationType.Conference => "Konferencja",
                SelfEducationType.Workshop => "Warsztaty",
                SelfEducationType.Course => "Kurs",
                SelfEducationType.ScientificMeeting => "Spotkanie naukowe",
                SelfEducationType.Publication => "Publikacja",
                SelfEducationType.Other => "Inne",
                _ => "Nieznany"
            };
        }

        private async void OnAddSelfEducationClicked(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new SelfEducationDetailsPage(null, OnSelfEducationAdded));
        }

        private async void OnEditSelfEducationClicked(object sender, EventArgs e)
        {
            if (sender is Button button && button.CommandParameter is int selfEducationId)
            {
                var selfEducation = _selfEducationList.FirstOrDefault(s => s.Id == selfEducationId);
                if (selfEducation != null)
                {
                    await Navigation.PushAsync(new SelfEducationDetailsPage(selfEducation, OnSelfEducationUpdated));
                }
            }
        }

        private void OnSelfEducationAdded(SelfEducation selfEducation)
        {
            // Generowanie nowego ID
            selfEducation.Id = _selfEducationList.Count > 0 ? _selfEducationList.Max(s => s.Id) + 1 : 1;
            _selfEducationList.Add(selfEducation);
            UpdateUsedDays();
            DisplaySelfEducation();
        }

        private void OnSelfEducationUpdated(SelfEducation selfEducation)
        {
            var existingSelfEducation = _selfEducationList.FirstOrDefault(s => s.Id == selfEducation.Id);
            if (existingSelfEducation != null)
            {
                var index = _selfEducationList.IndexOf(existingSelfEducation);
                _selfEducationList[index] = selfEducation;
            }
            UpdateUsedDays();
            DisplaySelfEducation();
        }
    }
}
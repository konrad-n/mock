using System;
using Microsoft.Maui.Controls;
using SledzSpecke.Models;
using SledzSpecke.Services;
using System.Collections.Generic;
using System.Linq;
using SledzSpecke.Core.Models;

namespace SledzSpecke.Views
{
    public partial class ProceduresPage : ContentPage
    {
        private Specialization _specialization;
        private ModuleType _currentModule = ModuleType.Basic;
        private ProcedureType _currentProcedureType = ProcedureType.TypeA;



        public ProceduresPage()
        {
            InitializeComponent();
            _specialization = DataSeeder.SeedHematologySpecialization();
            DisplayProcedures(_currentModule, _currentProcedureType);
        }

        private void DisplayProcedures(ModuleType moduleType, ProcedureType procedureType)
        {
            _currentModule = moduleType;
            _currentProcedureType = procedureType;
            ProceduresLayout.Children.Clear();
            NoSelectionLabel.IsVisible = false;

            // Ustawienie aktywnych przycisków
            if (moduleType == ModuleType.Basic)
            {
                BasicModuleButton.BackgroundColor = Colors.DarkBlue;
                BasicModuleButton.TextColor = Colors.White;
                SpecialisticModuleButton.BackgroundColor = Colors.LightGray;
                SpecialisticModuleButton.TextColor = Colors.Black;
            }
            else
            {
                BasicModuleButton.BackgroundColor = Colors.LightGray;
                BasicModuleButton.TextColor = Colors.Black;
                SpecialisticModuleButton.BackgroundColor = Colors.DarkBlue;
                SpecialisticModuleButton.TextColor = Colors.White;
            }

            if (procedureType == ProcedureType.TypeA)
            {
                TypeAButton.BackgroundColor = Colors.DarkGreen;
                TypeAButton.TextColor = Colors.White;
                TypeBButton.BackgroundColor = Colors.LightGray;
                TypeBButton.TextColor = Colors.Black;
            }
            else
            {
                TypeAButton.BackgroundColor = Colors.LightGray;
                TypeAButton.TextColor = Colors.Black;
                TypeBButton.BackgroundColor = Colors.DarkGreen;
                TypeBButton.TextColor = Colors.White;
            }

            var procedures = _specialization.RequiredProcedures
                .Where(p => p.Module == moduleType && p.ProcedureType == procedureType)
                .OrderBy(p => p.InternshipId)
                .ThenBy(p => p.Name)
                .ToList();

            if (procedures.Count == 0)
            {
                ProceduresLayout.Children.Add(new Label
                {
                    Text = "Brak procedur do wyświetlenia",
                    HorizontalOptions = LayoutOptions.Center,
                    VerticalOptions = LayoutOptions.Center,
                    Margin = new Thickness(0, 20, 0, 0)
                });
                return;
            }

            // Grupowanie procedur według stażu
            var proceduresByInternship = procedures.GroupBy(p => p.InternshipId);

            foreach (var group in proceduresByInternship)
            {
                var internshipId = group.Key;
                var internshipName = _specialization.RequiredInternships
                    .FirstOrDefault(i => i.Id == internshipId)?.Name ?? "Nieokreślony staż";

                // Nagłówek stażu
                ProceduresLayout.Children.Add(new Label
                {
                    Text = internshipName,
                    FontSize = 18,
                    FontAttributes = FontAttributes.Bold,
                    Margin = new Thickness(0, 10, 0, 5)
                });

                foreach (var procedure in group)
                {
                    var frame = new Frame
                    {
                        Padding = new Thickness(10),
                        Margin = new Thickness(0, 0, 0, 10),
                        CornerRadius = 5,
                        BorderColor = procedure.CompletedCount >= procedure.RequiredCount ? Colors.Green : Colors.LightGray
                    };

                    var progressBar = new ProgressBar
                    {
                        Progress = procedure.CompletionPercentage / 100,
                        HeightRequest = 10,
                        Margin = new Thickness(0, 5, 0, 5)
                    };

                    var titleLabel = new Label
                    {
                        Text = procedure.Name,
                        FontAttributes = FontAttributes.Bold,
                        FontSize = 16
                    };

                    var countLabel = new Label
                    {
                        Text = $"Wykonane: {procedure.CompletedCount}/{procedure.RequiredCount} ({procedure.CompletionPercentage:F0}%)",
                        FontSize = 14
                    };

                    var addEntryButton = new Button
                    {
                        Text = "Dodaj wykonanie",
                        HeightRequest = 35,
                        FontSize = 14,
                        Margin = new Thickness(0, 5, 0, 0),
                        CommandParameter = procedure.Id
                    };
                    addEntryButton.Clicked += OnAddProcedureEntryClicked;

                    var detailsButton = new Button
                    {
                        Text = "Szczegóły",
                        HeightRequest = 35,
                        FontSize = 14,
                        Margin = new Thickness(0, 5, 0, 0),
                        CommandParameter = procedure.Id
                    };
                    detailsButton.Clicked += OnProcedureDetailsClicked;

                    var buttonsLayout = new Grid
                    {
                        ColumnDefinitions = new ColumnDefinitionCollection
                    {
                        new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) },
                        new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) }
                    }
                    };
                    buttonsLayout.Add(addEntryButton, 0, 0);
                    buttonsLayout.Add(detailsButton, 1, 0);

                    var contentLayout = new VerticalStackLayout
                    {
                        Children = { titleLabel, countLabel, progressBar, buttonsLayout }
                    };

                    frame.Content = contentLayout;
                    ProceduresLayout.Children.Add(frame);
                }
            }
        }

        private void OnBasicModuleButtonClicked(object sender, EventArgs e)
        {
            DisplayProcedures(ModuleType.Basic, _currentProcedureType);
        }

        private void OnSpecialisticModuleButtonClicked(object sender, EventArgs e)
        {
            DisplayProcedures(ModuleType.Specialistic, _currentProcedureType);
        }

        private void OnTypeAButtonClicked(object sender, EventArgs e)
        {
            DisplayProcedures(_currentModule, ProcedureType.TypeA);
        }

        private void OnTypeBButtonClicked(object sender, EventArgs e)
        {
            DisplayProcedures(_currentModule, ProcedureType.TypeB);
        }

        private async void OnAddProcedureClicked(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new ProcedureDetailsPage(null, _currentModule, _currentProcedureType, OnProcedureAdded));
        }

        private async void OnProcedureDetailsClicked(object sender, EventArgs e)
        {
            if (sender is Button button && button.CommandParameter is int procedureId)
            {
                var procedure = _specialization.RequiredProcedures.FirstOrDefault(p => p.Id == procedureId);
                if (procedure != null)
                {
                    await Navigation.PushAsync(new ProcedureDetailsPage(procedure, _currentModule, _currentProcedureType, OnProcedureUpdated));
                }
            }
        }

        private async void OnAddProcedureEntryClicked(object sender, EventArgs e)
        {
            if (sender is Button button && button.CommandParameter is int procedureId)
            {
                var procedure = _specialization.RequiredProcedures.FirstOrDefault(p => p.Id == procedureId);
                if (procedure != null)
                {
                    await Navigation.PushAsync(new ProcedureEntryPage(procedure, OnProcedureEntryAdded));
                }
            }
        }

        private void OnProcedureAdded(MedicalProcedure procedure)
        {
            _specialization.RequiredProcedures.Add(procedure);
            DisplayProcedures(_currentModule, _currentProcedureType);
        }

        private void OnProcedureUpdated(MedicalProcedure procedure)
        {
            var existingProcedure = _specialization.RequiredProcedures.FirstOrDefault(p => p.Id == procedure.Id);
            if (existingProcedure != null)
            {
                var index = _specialization.RequiredProcedures.IndexOf(existingProcedure);
                _specialization.RequiredProcedures[index] = procedure;
            }
            DisplayProcedures(_currentModule, _currentProcedureType);
        }

        private void OnProcedureEntryAdded(MedicalProcedure procedure, ProcedureEntry entry)
        {
            var existingProcedure = _specialization.RequiredProcedures.FirstOrDefault(p => p.Id == procedure.Id);
            if (existingProcedure != null)
            {
                existingProcedure.Entries.Add(entry);
                existingProcedure.CompletedCount = existingProcedure.Entries.Count;
            }
            DisplayProcedures(_currentModule, _currentProcedureType);
        }
    }
}
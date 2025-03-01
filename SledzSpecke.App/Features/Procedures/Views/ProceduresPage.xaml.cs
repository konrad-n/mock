using SledzSpecke.App.Services;
using SledzSpecke.Core.Models;
using SledzSpecke.Core.Models.Enums;

namespace SledzSpecke.App.Features.Procedures.Views
{
    public partial class ProceduresPage : ContentPage
    {
        private Specialization _specialization;
        private ModuleType _currentModule = ModuleType.Basic;
        private ProcedureType _currentProcedureType = ProcedureType.TypeA;
        private ISpecializationService _specializationService;

        public ProceduresPage()
        {
            _specializationService = App.SpecializationService;

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
                // Pobieramy dane z bazy, nie z seedera
                _specialization = await _specializationService.GetSpecializationAsync();
                DisplayProcedures(_currentModule, _currentProcedureType);
            }
            catch (Exception ex)
            {
                await DisplayAlert("Błąd", $"Nie udało się załadować danych: {ex.Message}", "OK");
            }
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
                BasicModuleButton.BackgroundColor = new Color(8,32,68);
                BasicModuleButton.TextColor = Colors.White;
                SpecialisticModuleButton.BackgroundColor = new Color(228,240,245);
                SpecialisticModuleButton.TextColor = Colors.Black;
            }
            else
            {
                BasicModuleButton.BackgroundColor = new Color(228,240,245);
                BasicModuleButton.TextColor = Colors.Black;
                SpecialisticModuleButton.BackgroundColor = new Color(8,32,68);
                SpecialisticModuleButton.TextColor = Colors.White;
            }

            if (procedureType == ProcedureType.TypeA)
            {
                TypeAButton.BackgroundColor = new Color(13,117,156);
                TypeAButton.TextColor = Colors.White;
                TypeBButton.BackgroundColor = new Color(228,240,245);
                TypeBButton.TextColor = Colors.Black;
            }
            else
            {
                TypeAButton.BackgroundColor = new Color(228,240,245);
                TypeAButton.TextColor = Colors.Black;
                TypeBButton.BackgroundColor = new Color(13, 117, 156);
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
                        BorderColor = procedure.CompletedCount >= procedure.RequiredCount ? Colors.Green : new Color(228,240,245)
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

        // Zmiana metod callback na async Task
        private async Task OnProcedureAdded(MedicalProcedure procedure)
        {
            await _specializationService.SaveProcedureAsync(procedure);
            LoadDataAsync();
        }

        private async Task OnProcedureUpdated(MedicalProcedure procedure)
        {
            await _specializationService.SaveProcedureAsync(procedure);
            LoadDataAsync();
        }

        private async Task OnProcedureEntryAdded(MedicalProcedure procedure, ProcedureEntry entry)
        {
            await _specializationService.AddProcedureEntryAsync(procedure, entry);
            LoadDataAsync();
        }
    }
}
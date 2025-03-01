using SledzSpecke.App.Common.Views;
using SledzSpecke.App.Features.Procedures.ViewModels;
using SledzSpecke.App.Services;
using SledzSpecke.Core.Models;
using SledzSpecke.Core.Models.Enums;
using SledzSpecke.Infrastructure.Database;

namespace SledzSpecke.App.Features.Procedures.Views
{
    public partial class ProceduresPage : BaseContentPage
    {
        private ProceduresViewModel _viewModel;
        private readonly ISpecializationService _specializationService;
        private readonly IDatabaseService _databaseService;

        public ProceduresPage(
            ISpecializationService specializationService,
            IDatabaseService databaseService)
        {
            _specializationService = specializationService;
            _databaseService = databaseService;
            InitializeComponent();
        }

        protected override async Task InitializePageAsync()
        {
            try
            {
                _viewModel = GetRequiredService<ProceduresViewModel>();
                BindingContext = _viewModel;
                await _viewModel.InitializeAsync();

                // Po inicjalizacji wyświetl procedury
                DisplayProcedures();
            }
            catch (Exception ex)
            {
                await DisplayAlert("Błąd", "Nie udało się załadować danych procedur.", "OK");
                System.Diagnostics.Debug.WriteLine($"Error in ProceduresPage: {ex}");
            }
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();

            // Odśwież dane procedur przy każdym pokazaniu strony
            if (_viewModel != null)
            {
                _viewModel.LoadSpecializationDataAsync().ContinueWith(_ =>
                {
                    MainThread.BeginInvokeOnMainThread(() =>
                    {
                        DisplayProcedures();
                    });
                });
            }
        }

        private void DisplayProcedures()
        {
            ProceduresLayout.Children.Clear();
            NoSelectionLabel.IsVisible = false;

            if (_viewModel.Specialization == null)
                return;

            var procedures = _viewModel.Specialization.RequiredProcedures
                .Where(p => p.Module == _viewModel.CurrentModule && p.ProcedureType == _viewModel.CurrentProcedureType)
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
                var internshipName = _viewModel.Specialization.RequiredInternships
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
                        BorderColor = procedure.CompletedCount >= procedure.RequiredCount ? Colors.Green : new Color(228, 240, 245)
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

        // Obsługa przycisków filtrów
        private void OnBasicModuleButtonClicked(object sender, EventArgs e)
        {
            if (_viewModel.CurrentModule != ModuleType.Basic)
            {
                _viewModel.CurrentModule = ModuleType.Basic;
                _viewModel.UpdateButtonStyles();
                DisplayProcedures();
            }
        }

        private void OnSpecialisticModuleButtonClicked(object sender, EventArgs e)
        {
            if (_viewModel.CurrentModule != ModuleType.Specialistic)
            {
                _viewModel.CurrentModule = ModuleType.Specialistic;
                _viewModel.UpdateButtonStyles();
                DisplayProcedures();
            }
        }

        private void OnTypeAButtonClicked(object sender, EventArgs e)
        {
            if (_viewModel.CurrentProcedureType != ProcedureType.TypeA)
            {
                _viewModel.CurrentProcedureType = ProcedureType.TypeA;
                _viewModel.UpdateButtonStyles();
                DisplayProcedures();
            }
        }

        private void OnTypeBButtonClicked(object sender, EventArgs e)
        {
            if (_viewModel.CurrentProcedureType != ProcedureType.TypeB)
            {
                _viewModel.CurrentProcedureType = ProcedureType.TypeB;
                _viewModel.UpdateButtonStyles();
                DisplayProcedures();
            }
        }

        private async void OnAddProcedureClicked(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new ProcedureDetailsPage(null, _viewModel.CurrentModule, _viewModel.CurrentProcedureType, OnProcedureAdded));
        }

        private async void OnProcedureDetailsClicked(object sender, EventArgs e)
        {
            if (sender is Button button && button.CommandParameter is int procedureId)
            {
                var procedure = _viewModel.Specialization.RequiredProcedures.FirstOrDefault(p => p.Id == procedureId);
                if (procedure != null)
                {
                    await Navigation.PushAsync(new ProcedureDetailsPage(procedure, _viewModel.CurrentModule, _viewModel.CurrentProcedureType, OnProcedureUpdated));
                }
            }
        }

        private async void OnAddProcedureEntryClicked(object sender, EventArgs e)
        {
            if (sender is Button button && button.CommandParameter is int procedureId)
            {
                var procedure = _viewModel.Specialization.RequiredProcedures.FirstOrDefault(p => p.Id == procedureId);
                if (procedure != null)
                {
                    await Navigation.PushAsync(new ProcedureEntryPage(_databaseService, procedure, OnProcedureEntryAdded));
                }
            }
        }

        // Callback methods
        private async Task OnProcedureAdded(MedicalProcedure procedure)
        {
            await _viewModel.SaveProcedureAsync(procedure);
            DisplayProcedures();
        }

        private async Task OnProcedureUpdated(MedicalProcedure procedure)
        {
            await _viewModel.SaveProcedureAsync(procedure);
            DisplayProcedures();
        }

        private async Task OnProcedureEntryAdded(MedicalProcedure procedure, ProcedureEntry entry)
        {
            await _viewModel.AddProcedureEntryAsync(procedure, entry);
            DisplayProcedures();
        }
    }
}
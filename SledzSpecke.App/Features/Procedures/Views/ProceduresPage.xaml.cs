using SledzSpecke.App.Common.Views;
using SledzSpecke.App.Features.Procedures.ViewModels;
using SledzSpecke.App.Services.Interfaces;
using SledzSpecke.Core.Models;
using SledzSpecke.Core.Models.Enums;
using SledzSpecke.Infrastructure.Database;

namespace SledzSpecke.App.Features.Procedures.Views
{
    public partial class ProceduresPage : BaseContentPage
    {
        private readonly IDatabaseService databaseService;
        private ProceduresViewModel viewModel;

        public ProceduresPage(
            ISpecializationService specializationService,
            IDatabaseService databaseService)
        {
            this.databaseService = databaseService;
            this.InitializeComponent();
        }

        protected override async Task InitializePageAsync()
        {
            try
            {
                this.viewModel = this.GetRequiredService<ProceduresViewModel>();
                this.BindingContext = this.viewModel;
                await this.viewModel.InitializeAsync();
                this.DisplayProcedures();
            }
            catch (Exception ex)
            {
                await this.DisplayAlert("Błąd", "Nie udało się zainicjalizować strony procedur.", "OK");
                System.Diagnostics.Debug.WriteLine($"Error in ProceduresPage: {ex}");
            }
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();

            if (this.viewModel != null)
            {
                this.viewModel.LoadSpecializationDataAsync().ContinueWith(_ =>
                {
                    MainThread.BeginInvokeOnMainThread(() =>
                    {
                        this.DisplayProcedures();
                    });
                });
            }
        }

        private void DisplayProcedures()
        {
            this.ProceduresLayout.Children.Clear();
            this.NoSelectionLabel.IsVisible = false;

            if (this.viewModel.Specialization == null)
            {
                return;
            }

            var procedures = this.viewModel.Specialization.RequiredProcedures
                .Where(p => p.Module == this.viewModel.CurrentModule && p.ProcedureType == this.viewModel.CurrentProcedureType)
                .OrderBy(p => p.Name)
                .ToList();

            this.viewModel.IsProceduresEmpty = procedures.Count == 0;

            if (procedures.Count == 0)
            {
                this.ProceduresLayout.Children.Add(new Label
                {
                    Text = "Brak procedur do wyświetlenia",
                    HorizontalOptions = LayoutOptions.Center,
                    VerticalOptions = LayoutOptions.Center,
                    Margin = new Thickness(0, 20, 0, 0),
                });
                return;
            }

            var proceduresByInternship = procedures.GroupBy(p => p.InternshipId);

            foreach (var group in proceduresByInternship)
            {
                var internshipId = group.Key;
                var internshipName = this.viewModel.Specialization.RequiredInternships
                    .FirstOrDefault(i => i.Id == internshipId)?.Name ?? "Nieokreślony staż";
                this.ProceduresLayout.Children.Add(new Label
                {
                    Text = internshipName,
                    FontSize = 18,
                    FontAttributes = FontAttributes.Bold,
                    Margin = new Thickness(0, 10, 0, 5),
                });

                foreach (var procedure in group)
                {
                    var frame = new Frame
                    {
                        Padding = new Thickness(10),
                        Margin = new Thickness(0, 0, 0, 10),
                        CornerRadius = 5,
                        BorderColor =
                            procedure.CompletedCount >= procedure.RequiredCount
                                ? Colors.Green
                                : new Color(228, 240, 245),
                    };

                    var progressBar = new ProgressBar
                    {
                        Progress = procedure.CompletionPercentage / 100,
                        HeightRequest = 10,
                        Margin = new Thickness(0, 5, 0, 5),
                    };

                    var titleLabel = new Label
                    {
                        Text = procedure.Name,
                        FontAttributes = FontAttributes.Bold,
                        FontSize = 16,
                    };

                    var countLabel = new Label
                    {
                        Text = $"Wykonane: {procedure.CompletedCount}/{procedure.RequiredCount} ({procedure.CompletionPercentage:F0}%)",
                        FontSize = 14,
                    };

                    var addEntryButton = new Button
                    {
                        Text = "Dodaj wykonanie",
                        HeightRequest = 35,
                        FontSize = 14,
                        Margin = new Thickness(0, 5, 0, 0),
                        CommandParameter = procedure.Id,
                    };
                    addEntryButton.Clicked += this.OnAddProcedureEntryClicked;

                    var detailsButton = new Button
                    {
                        Text = "Szczegóły",
                        HeightRequest = 35,
                        FontSize = 14,
                        Margin = new Thickness(0, 5, 0, 0),
                        CommandParameter = procedure.Id,
                    };
                    detailsButton.Clicked += this.OnProcedureDetailsClicked;

                    var buttonsLayout = new Grid
                    {
                        ColumnDefinitions = new ColumnDefinitionCollection
                        {
                            new ColumnDefinition
                            {
                                Width = new GridLength(1, GridUnitType.Star),
                            },
                            new ColumnDefinition
                            {
                                Width = new GridLength(1, GridUnitType.Star),
                            },
                        },
                    };

                    buttonsLayout.Add(addEntryButton, 0, 0);
                    buttonsLayout.Add(detailsButton, 1, 0);

                    var contentLayout = new VerticalStackLayout
                    {
                        Children = { titleLabel, countLabel, progressBar, buttonsLayout }
                    };

                    frame.Content = contentLayout;
                    this.ProceduresLayout.Children.Add(frame);
                }
            }
        }

        private void OnBasicModuleButtonClicked(object sender, EventArgs e)
        {
            if (this.viewModel.CurrentModule != ModuleType.Basic)
            {
                this.viewModel.CurrentModule = ModuleType.Basic;
                this.viewModel.UpdateButtonStyles();
                this.DisplayProcedures();
            }
        }

        private void OnSpecialisticModuleButtonClicked(object sender, EventArgs e)
        {
            if (this.viewModel.CurrentModule != ModuleType.Specialistic)
            {
                this.viewModel.CurrentModule = ModuleType.Specialistic;
                this.viewModel.UpdateButtonStyles();
                this.DisplayProcedures();
            }
        }

        private void OnTypeAButtonClicked(object sender, EventArgs e)
        {
            if (this.viewModel.CurrentProcedureType != ProcedureType.TypeA)
            {
                this.viewModel.CurrentProcedureType = ProcedureType.TypeA;
                this.viewModel.UpdateButtonStyles();
                this.DisplayProcedures();
            }
        }

        private void OnTypeBButtonClicked(object sender, EventArgs e)
        {
            if (this.viewModel.CurrentProcedureType != ProcedureType.TypeB)
            {
                this.viewModel.CurrentProcedureType = ProcedureType.TypeB;
                this.viewModel.UpdateButtonStyles();
                this.DisplayProcedures();
            }
        }

        private async void OnAddProcedureClicked(object sender, EventArgs e)
        {
            await this.Navigation.PushAsync(new ProcedureDetailsPage(null, this.viewModel.CurrentModule, this.viewModel.CurrentProcedureType, this.OnProcedureAdded));
        }

        private async void OnProcedureDetailsClicked(object sender, EventArgs e)
        {
            if (sender is Button button && button.CommandParameter is int procedureId)
            {
                var procedure = this.viewModel.Specialization.RequiredProcedures.FirstOrDefault(p => p.Id == procedureId);
                if (procedure != null)
                {
                    await this.Navigation.PushAsync(new ProcedureDetailsPage(procedure, this.viewModel.CurrentModule, this.viewModel.CurrentProcedureType, this.OnProcedureUpdated));
                }
            }
        }

        private async void OnAddProcedureEntryClicked(object sender, EventArgs e)
        {
            if (sender is Button button && button.CommandParameter is int procedureId)
            {
                var procedure = this.viewModel.Specialization.RequiredProcedures.FirstOrDefault(p => p.Id == procedureId);
                if (procedure != null)
                {
                    await this.Navigation.PushAsync(new ProcedureEntryPage(this.databaseService, procedure, this.OnProcedureEntryAdded));
                }
            }
        }

        private async Task OnProcedureAdded(MedicalProcedure procedure)
        {
            await this.viewModel.SaveProcedureAsync(procedure);
            this.DisplayProcedures();
        }

        private async Task OnProcedureUpdated(MedicalProcedure procedure)
        {
            await this.viewModel.SaveProcedureAsync(procedure);
            this.DisplayProcedures();
        }

        private async Task OnProcedureEntryAdded(MedicalProcedure procedure, ProcedureEntry entry)
        {
            await this.viewModel.AddProcedureEntryAsync(procedure, entry);
            this.DisplayProcedures();
        }
    }
}
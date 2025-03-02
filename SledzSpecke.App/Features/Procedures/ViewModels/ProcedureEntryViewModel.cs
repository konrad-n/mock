using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Extensions.Logging;
using SledzSpecke.App.Common.ViewModels;
using SledzSpecke.Core.Models;
using SledzSpecke.Infrastructure.Database;

namespace SledzSpecke.App.Features.Procedures.ViewModels
{
    public partial class ProcedureEntryViewModel : ViewModelBase
    {
        private readonly IDatabaseService databaseService;
        private Func<MedicalProcedure, ProcedureEntry, Task> onSaveCallback;
        private MedicalProcedure procedure;

        [ObservableProperty]
        private string procedureName;

        [ObservableProperty]
        private string procedureType;

        [ObservableProperty]
        private string completionStatus;

        [ObservableProperty]
        private string remainingText;

        [ObservableProperty]
        private double completionProgress;

        [ObservableProperty]
        private Color procedureTypeColor;

        [ObservableProperty]
        private Color procedureTypeBorderColor;

        [ObservableProperty]
        private Color progressColor;

        [ObservableProperty]
        private DateTime entryDate = DateTime.Now;

        [ObservableProperty]
        private string patientId;

        [ObservableProperty]
        private string patientGender = "Mężczyzna";

        [ObservableProperty]
        private string location;

        [ObservableProperty]
        private string supervisorName;

        [ObservableProperty]
        private string supervisorLabel;

        [ObservableProperty]
        private string supervisorPlaceholder;

        [ObservableProperty]
        private string firstAssistantLabel;

        [ObservableProperty]
        private string firstAssistantPlaceholder;

        [ObservableProperty]
        private string firstAssistantData;

        [ObservableProperty]
        private string secondAssistantData;

        [ObservableProperty]
        private string procedureGroup;

        [ObservableProperty]
        private string notes;

        public ProcedureEntryViewModel(
            IDatabaseService databaseService,
            ILogger<ProcedureEntryViewModel> logger)
            : base(logger)
        {
            this.databaseService = databaseService;
            this.Title = "Dodaj wykonanie procedury";
        }

        public void Initialize(MedicalProcedure procedure, Func<MedicalProcedure, ProcedureEntry, Task> onSaveCallback)
        {
            this.procedure = procedure;
            this.onSaveCallback = onSaveCallback;
            this.ProcedureName = procedure.Name;

            if (procedure.ProcedureType == Core.Models.Enums.ProcedureType.TypeA)
            {
                this.ProcedureType = "Kod A - wykonywanie samodzielne";
                this.ProcedureTypeColor = Color.FromArgb("#2196F3");
                this.ProcedureTypeBorderColor = Color.FromArgb("#1976D2");
                this.SupervisorLabel = "Nadzorujący (opcjonalnie)";
                this.SupervisorPlaceholder = "Wprowadź imię i nazwisko nadzorującego (jeśli dotyczy)";
                this.FirstAssistantLabel = "Dane osoby wykonującej I asystę";
                this.FirstAssistantPlaceholder = "Wprowadź dane osoby wykonującej I asystę";
            }
            else
            {
                this.ProcedureType = "Kod B - pierwsza asysta";
                this.ProcedureTypeColor = Color.FromArgb("#4CAF50");
                this.ProcedureTypeBorderColor = Color.FromArgb("#388E3C");
                this.SupervisorLabel = "Nadzorujący (wymagane)";
                this.SupervisorPlaceholder = "Wprowadź imię i nazwisko nadzorującego";
                this.FirstAssistantLabel = "Dane osoby wykonującej procedurę";
                this.FirstAssistantPlaceholder = "Wprowadź dane osoby wykonującej procedurę";
            }

            this.CompletionStatus = $"Wykonane: {procedure.CompletedCount}/{procedure.RequiredCount}";
            int remaining = procedure.RequiredCount - procedure.CompletedCount;
            this.RemainingText = $"Pozostało: {remaining}";
            this.CompletionProgress = (double)procedure.CompletedCount / procedure.RequiredCount;

            if (this.CompletionProgress >= 1.0)
            {
                this.ProgressColor = Color.FromArgb("#4CAF50");
            }
            else if (this.CompletionProgress >= 0.7)
            {
                this.ProgressColor = Color.FromArgb("#FFB74D");
            }
            else
            {
                this.ProgressColor = Color.FromArgb("#2196F3");
            }

            if (procedure.InternshipId.HasValue)
            {
                Task.Run(async () =>
                {
                    try
                    {
                        var internship = await this.databaseService.GetByIdAsync<Internship>(procedure.InternshipId.Value);
                        if (internship != null)
                        {
                            this.ProcedureGroup = $"{procedure.Name} - {internship.Name}";
                            this.OnPropertyChanged(nameof(this.ProcedureGroup));
                        }
                    }
                    catch (Exception ex)
                    {
                        this.logger.LogError(ex, "Error getting internship name");
                    }
                });
            }
        }

        [RelayCommand]
        private async Task ShowProcedureGroupInfoAsync()
        {
            await Application.Current.MainPage.DisplayAlert(
                "Informacja o polu 'Procedura z grupy'",
                "Pole to jest wymagane do eksportu SMK i jest wykorzystywane, gdy procedura jest częścią grupy procedur, np. \"Operacje brzucha: procedura A, procedura B\".\n\n" +
                "Jeśli w nazwie procedury wymienione są procedury po przecinku, należy uzupełnić to pole, wpisując konkretną procedurę, którą wykonano.",
                "Zamknij");
        }

        [RelayCommand]
        private async Task CancelAsync()
        {
            await Shell.Current.Navigation.PopAsync();
        }

        [RelayCommand]
        private async Task SaveEntryAsync()
        {
            try
            {
                if (string.IsNullOrWhiteSpace(this.PatientId))
                {
                    await Application.Current.MainPage.DisplayAlert("Błąd", "Identyfikator pacjenta jest wymagany.", "OK");
                    return;
                }

                if (string.IsNullOrWhiteSpace(this.PatientGender))
                {
                    await Application.Current.MainPage.DisplayAlert("Błąd", "Płeć pacjenta jest wymagana.", "OK");
                    return;
                }

                if (string.IsNullOrWhiteSpace(this.Location))
                {
                    await Application.Current.MainPage.DisplayAlert("Błąd", "Miejsce wykonania jest wymagane.", "OK");
                    return;
                }

                if (this.procedure.ProcedureType == Core.Models.Enums.ProcedureType.TypeB && string.IsNullOrWhiteSpace(this.SupervisorName))
                {
                    await Application.Current.MainPage.DisplayAlert("Błąd", "Imię i nazwisko nadzorującego jest wymagane dla procedury typu B.", "OK");
                    return;
                }

                var procedureEntry = new ProcedureEntry
                {
                    Date = this.EntryDate,
                    PatientId = this.PatientId,
                    PatientGender = this.PatientGender,
                    Location = this.Location,
                    SupervisorName = this.SupervisorName,
                    FirstAssistantData = this.FirstAssistantData,
                    SecondAssistantData = this.SecondAssistantData,
                    ProcedureGroup = this.ProcedureGroup,
                    InternshipName = this.procedure.InternshipId.HasValue ?
                        (await this.databaseService.GetByIdAsync<Internship>(this.procedure.InternshipId.Value))?.Name : string.Empty,
                    Notes = this.Notes
                };

                if (this.onSaveCallback != null)
                {
                    await this.onSaveCallback(this.procedure, procedureEntry);
                }

                await Shell.Current.Navigation.PopAsync();
            }
            catch (Exception ex)
            {
                this.logger.LogError(ex, "Error saving procedure entry");
                await Application.Current.MainPage.DisplayAlert("Błąd", $"Wystąpił błąd podczas zapisywania: {ex.Message}", "OK");
            }
        }
    }
}
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
        private readonly IDatabaseService _databaseService;
        private Func<MedicalProcedure, ProcedureEntry, Task> _onSaveCallback;
        private MedicalProcedure _procedure;

        [ObservableProperty]
        private string _procedureName;

        [ObservableProperty]
        private string _procedureType;

        [ObservableProperty]
        private string _completionStatus;

        [ObservableProperty]
        private string _remainingText;

        [ObservableProperty]
        private double _completionProgress;

        [ObservableProperty]
        private Color _procedureTypeColor;

        [ObservableProperty]
        private Color _procedureTypeBorderColor;

        [ObservableProperty]
        private Color _progressColor;

        [ObservableProperty]
        private DateTime _entryDate = DateTime.Now;

        [ObservableProperty]
        private string _patientId;

        [ObservableProperty]
        private string _patientGender = "Mężczyzna"; // Default value

        [ObservableProperty]
        private string _location;

        [ObservableProperty]
        private string _supervisorName;

        [ObservableProperty]
        private string _supervisorLabel;

        [ObservableProperty]
        private string _supervisorPlaceholder;

        [ObservableProperty]
        private string _firstAssistantLabel;

        [ObservableProperty]
        private string _firstAssistantPlaceholder;

        [ObservableProperty]
        private string _firstAssistantData;

        [ObservableProperty]
        private string _secondAssistantData;

        [ObservableProperty]
        private string _procedureGroup;

        [ObservableProperty]
        private string _notes;

        public ProcedureEntryViewModel(
            IDatabaseService databaseService,
            ILogger<ProcedureEntryViewModel> logger) : base(logger)
        {
            this._databaseService = databaseService;
            this.Title = "Dodaj wykonanie procedury";
        }

        public void Initialize(MedicalProcedure procedure, Func<MedicalProcedure, ProcedureEntry, Task> onSaveCallback)
        {
            this._procedure = procedure;
            this._onSaveCallback = onSaveCallback;

            this.ProcedureName = procedure.Name;

            // Set type-specific properties based on procedure type
            if (procedure.ProcedureType == Core.Models.Enums.ProcedureType.TypeA)
            {
                this.ProcedureType = "Kod A - wykonywanie samodzielne";
                this.ProcedureTypeColor = Color.FromArgb("#2196F3"); // Blue for Type A
                this.ProcedureTypeBorderColor = Color.FromArgb("#1976D2");
                this.SupervisorLabel = "Nadzorujący (opcjonalnie)";
                this.SupervisorPlaceholder = "Wprowadź imię i nazwisko nadzorującego (jeśli dotyczy)";
                this.FirstAssistantLabel = "Dane osoby wykonującej I asystę";
                this.FirstAssistantPlaceholder = "Wprowadź dane osoby wykonującej I asystę";
            }
            else
            {
                this.ProcedureType = "Kod B - pierwsza asysta";
                this.ProcedureTypeColor = Color.FromArgb("#4CAF50"); // Green for Type B
                this.ProcedureTypeBorderColor = Color.FromArgb("#388E3C");
                this.SupervisorLabel = "Nadzorujący (wymagane)";
                this.SupervisorPlaceholder = "Wprowadź imię i nazwisko nadzorującego";
                this.FirstAssistantLabel = "Dane osoby wykonującej procedurę";
                this.FirstAssistantPlaceholder = "Wprowadź dane osoby wykonującej procedurę";
            }

            // Calculate and set completion information
            this.CompletionStatus = $"Wykonane: {procedure.CompletedCount}/{procedure.RequiredCount}";
            int remaining = procedure.RequiredCount - procedure.CompletedCount;
            this.RemainingText = $"Pozostało: {remaining}";
            this.CompletionProgress = (double)procedure.CompletedCount / procedure.RequiredCount;

            // Set progress color based on completion
            if (this.CompletionProgress >= 1.0)
            {
                this.ProgressColor = Color.FromArgb("#4CAF50"); // Green when complete
            }
            else if (this.CompletionProgress >= 0.7)
            {
                this.ProgressColor = Color.FromArgb("#FFB74D"); // Orange when nearly complete
            }
            else
            {
                this.ProgressColor = Color.FromArgb("#2196F3"); // Blue for in progress
            }

            // Get internship name if available
            if (procedure.InternshipId.HasValue)
            {
                Task.Run(async () =>
                {
                    try
                    {
                        var internship = await this._databaseService.GetByIdAsync<Internship>(procedure.InternshipId.Value);
                        if (internship != null)
                        {
                            this.ProcedureGroup = $"{procedure.Name} - {internship.Name}";
                            this.OnPropertyChanged(nameof(this.ProcedureGroup));
                        }
                    }
                    catch (Exception ex)
                    {
                        this._logger.LogError(ex, "Error getting internship name");
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
                // Validation
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

                // For type B (assistance) supervisor is required
                if (this._procedure.ProcedureType == Core.Models.Enums.ProcedureType.TypeB && string.IsNullOrWhiteSpace(this.SupervisorName))
                {
                    await Application.Current.MainPage.DisplayAlert("Błąd", "Imię i nazwisko nadzorującego jest wymagane dla procedury typu B.", "OK");
                    return;
                }

                // Create new entry
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
                    InternshipName = this._procedure.InternshipId.HasValue ?
                        (await this._databaseService.GetByIdAsync<Internship>(this._procedure.InternshipId.Value))?.Name : "",
                    Notes = this.Notes
                };

                if (this._onSaveCallback != null)
                {
                    await this._onSaveCallback(this._procedure, procedureEntry);
                }

                await Shell.Current.Navigation.PopAsync();
            }
            catch (Exception ex)
            {
                this._logger.LogError(ex, "Error saving procedure entry");
                await Application.Current.MainPage.DisplayAlert("Błąd", $"Wystąpił błąd podczas zapisywania: {ex.Message}", "OK");
            }
        }
    }
}
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
            _databaseService = databaseService;
            Title = "Dodaj wykonanie procedury";
        }

        public void Initialize(MedicalProcedure procedure, Func<MedicalProcedure, ProcedureEntry, Task> onSaveCallback)
        {
            _procedure = procedure;
            _onSaveCallback = onSaveCallback;

            ProcedureName = procedure.Name;

            // Set type-specific properties based on procedure type
            if (procedure.ProcedureType == Core.Models.Enums.ProcedureType.TypeA)
            {
                ProcedureType = "Kod A - wykonywanie samodzielne";
                ProcedureTypeColor = Color.FromArgb("#2196F3"); // Blue for Type A
                ProcedureTypeBorderColor = Color.FromArgb("#1976D2");
                SupervisorLabel = "Nadzorujący (opcjonalnie)";
                SupervisorPlaceholder = "Wprowadź imię i nazwisko nadzorującego (jeśli dotyczy)";
                FirstAssistantLabel = "Dane osoby wykonującej I asystę";
                FirstAssistantPlaceholder = "Wprowadź dane osoby wykonującej I asystę";
            }
            else
            {
                ProcedureType = "Kod B - pierwsza asysta";
                ProcedureTypeColor = Color.FromArgb("#4CAF50"); // Green for Type B
                ProcedureTypeBorderColor = Color.FromArgb("#388E3C");
                SupervisorLabel = "Nadzorujący (wymagane)";
                SupervisorPlaceholder = "Wprowadź imię i nazwisko nadzorującego";
                FirstAssistantLabel = "Dane osoby wykonującej procedurę";
                FirstAssistantPlaceholder = "Wprowadź dane osoby wykonującej procedurę";
            }

            // Calculate and set completion information
            CompletionStatus = $"Wykonane: {procedure.CompletedCount}/{procedure.RequiredCount}";
            int remaining = procedure.RequiredCount - procedure.CompletedCount;
            RemainingText = $"Pozostało: {remaining}";
            CompletionProgress = (double)procedure.CompletedCount / procedure.RequiredCount;

            // Set progress color based on completion
            if (CompletionProgress >= 1.0)
            {
                ProgressColor = Color.FromArgb("#4CAF50"); // Green when complete
            }
            else if (CompletionProgress >= 0.7)
            {
                ProgressColor = Color.FromArgb("#FFB74D"); // Orange when nearly complete
            }
            else
            {
                ProgressColor = Color.FromArgb("#2196F3"); // Blue for in progress
            }

            // Get internship name if available
            if (procedure.InternshipId.HasValue)
            {
                Task.Run(async () =>
                {
                    try
                    {
                        var internship = await _databaseService.GetByIdAsync<Internship>(procedure.InternshipId.Value);
                        if (internship != null)
                        {
                            ProcedureGroup = $"{procedure.Name} - {internship.Name}";
                            OnPropertyChanged(nameof(ProcedureGroup));
                        }
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex, "Error getting internship name");
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
        private async Task SaveAsync()
        {
            try
            {
                // Validation
                if (string.IsNullOrWhiteSpace(PatientId))
                {
                    await Application.Current.MainPage.DisplayAlert("Błąd", "Identyfikator pacjenta jest wymagany.", "OK");
                    return;
                }

                if (string.IsNullOrWhiteSpace(PatientGender))
                {
                    await Application.Current.MainPage.DisplayAlert("Błąd", "Płeć pacjenta jest wymagana.", "OK");
                    return;
                }

                if (string.IsNullOrWhiteSpace(Location))
                {
                    await Application.Current.MainPage.DisplayAlert("Błąd", "Miejsce wykonania jest wymagane.", "OK");
                    return;
                }

                // For type B (assistance) supervisor is required
                if (_procedure.ProcedureType == Core.Models.Enums.ProcedureType.TypeB && string.IsNullOrWhiteSpace(SupervisorName))
                {
                    await Application.Current.MainPage.DisplayAlert("Błąd", "Imię i nazwisko nadzorującego jest wymagane dla procedury typu B.", "OK");
                    return;
                }

                // Create new entry
                var procedureEntry = new ProcedureEntry
                {
                    Id = new Random().Next(1000, 9999), // Temporary ID
                    Date = EntryDate,
                    PatientId = PatientId,
                    PatientGender = PatientGender,
                    Location = Location,
                    SupervisorName = SupervisorName,
                    FirstAssistantData = FirstAssistantData,
                    SecondAssistantData = SecondAssistantData,
                    ProcedureGroup = ProcedureGroup,
                    InternshipName = _procedure.InternshipId.HasValue ?
                        (await _databaseService.GetByIdAsync<Internship>(_procedure.InternshipId.Value))?.Name : "",
                    Notes = Notes
                };

                if (_onSaveCallback != null)
                {
                    await _onSaveCallback(_procedure, procedureEntry);
                }

                await Shell.Current.Navigation.PopAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error saving procedure entry");
                await Application.Current.MainPage.DisplayAlert("Błąd", $"Wystąpił błąd podczas zapisywania: {ex.Message}", "OK");
            }
        }
    }
}
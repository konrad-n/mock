using SledzSpecke.Core.Models;

namespace SledzSpecke.App.Views
{
    public partial class ProcedureEntryPage : ContentPage
    {
        private MedicalProcedure _procedure;
        private Func<MedicalProcedure, ProcedureEntry, Task> _onSaveCallback;

        public string ProcedureName { get; set; }
        public string ProcedureType { get; set; }
        public string CompletionStatus { get; set; }
        public string RemainingText { get; set; }
        public double CompletionProgress { get; set; }
        public Color ProcedureTypeColor { get; set; }
        public Color ProcedureTypeBorderColor { get; set; }
        public Color ProgressColor { get; set; }
        public DateTime EntryDate { get; set; } = DateTime.Now;
        public string PatientId { get; set; }
        public string PatientGender { get; set; } = "Mężczyzna"; // Default value
        public string Location { get; set; }
        public string SupervisorName { get; set; }
        public string SupervisorLabel { get; set; }
        public string SupervisorPlaceholder { get; set; }
        public string FirstAssistantLabel { get; set; }
        public string FirstAssistantPlaceholder { get; set; }
        public string FirstAssistantData { get; set; }
        public string SecondAssistantData { get; set; }
        public string ProcedureGroup { get; set; }
        public string Notes { get; set; }

        public ProcedureEntryPage(MedicalProcedure procedure, Func<MedicalProcedure, ProcedureEntry, Task> onSaveCallback)
        {
            InitializeComponent();
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
                        var internship = await App.DatabaseService.GetByIdAsync<Internship>(procedure.InternshipId.Value);
                        if (internship != null)
                        {
                            ProcedureGroup = $"{procedure.Name} - {internship.Name}";

                            // Update UI on main thread
                            MainThread.BeginInvokeOnMainThread(() =>
                            {
                                OnPropertyChanged(nameof(ProcedureGroup));
                            });
                        }
                    }
                    catch (Exception ex)
                    {
                        System.Diagnostics.Debug.WriteLine($"Error getting internship: {ex.Message}");
                    }
                });
            }

            BindingContext = this;
        }

        private async void OnProcedureGroupInfoClicked(object sender, EventArgs e)
        {
            await DisplayAlert(
                "Informacja o polu 'Procedura z grupy'",
                "Pole to jest wymagane do eksportu SMK i jest wykorzystywane, gdy procedura jest częścią grupy procedur, np. \"Operacje brzucha: procedura A, procedura B\".\n\n" +
                "Jeśli w nazwie procedury wymienione są procedury po przecinku, należy uzupełnić to pole, wpisując konkretną procedurę, którą wykonano.",
                "Zamknij");
        }

        private async void OnCancelClicked(object sender, EventArgs e)
        {
            await Navigation.PopAsync();
        }

        private async void OnSaveClicked(object sender, EventArgs e)
        {
            // Validation
            if (string.IsNullOrWhiteSpace(PatientId))
            {
                await DisplayAlert("Błąd", "Identyfikator pacjenta jest wymagany.", "OK");
                return;
            }

            if (string.IsNullOrWhiteSpace(PatientGender))
            {
                await DisplayAlert("Błąd", "Płeć pacjenta jest wymagana.", "OK");
                return;
            }

            if (string.IsNullOrWhiteSpace(Location))
            {
                await DisplayAlert("Błąd", "Miejsce wykonania jest wymagane.", "OK");
                return;
            }

            // For type B (assistance) supervisor is required
            if (_procedure.ProcedureType == Core.Models.Enums.ProcedureType.TypeB && string.IsNullOrWhiteSpace(SupervisorName))
            {
                await DisplayAlert("Błąd", "Imię i nazwisko nadzorującego jest wymagane dla procedury typu B.", "OK");
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
                    (await App.DatabaseService.GetByIdAsync<Internship>(_procedure.InternshipId.Value))?.Name : "",
                Notes = Notes
            };

            if (_onSaveCallback != null)
            {
                await _onSaveCallback(_procedure, procedureEntry);
            }

            await Navigation.PopAsync();
        }
    }
}
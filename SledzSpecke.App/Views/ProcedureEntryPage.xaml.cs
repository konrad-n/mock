using SledzSpecke.Core.Models;
using SledzSpecke.Core.Models.Enums;

namespace SledzSpecke.App.Views
{
    public partial class ProcedureEntryPage : ContentPage
    {
        private MedicalProcedure _procedure;
        private Func<MedicalProcedure, ProcedureEntry, Task> _onSaveCallback;

        public string ProcedureName { get; set; }
        public string ProcedureType { get; set; }
        public string CompletionStatus { get; set; }
        public DateTime EntryDate { get; set; } = DateTime.Now;
        public string PatientId { get; set; }
        public string PatientGender { get; set; } // New field for SMK
        public string Location { get; set; }
        public string SupervisorName { get; set; }
        public string FirstAssistantData { get; set; } // New field for SMK
        public string SecondAssistantData { get; set; } // New field for SMK
        public string ProcedureGroup { get; set; } // New field for SMK
        public string InternshipName { get; set; } // New field for SMK
        public string Notes { get; set; }

        public ProcedureEntryPage(MedicalProcedure procedure, Func<MedicalProcedure, ProcedureEntry, Task> onSaveCallback)
        {
            InitializeComponent();
            _procedure = procedure;
            _onSaveCallback = onSaveCallback;

            ProcedureName = procedure.Name;
            ProcedureType = procedure.ProcedureType == Core.Models.Enums.ProcedureType.TypeA
                ? "Kod A - wykonywanie samodzielne"
                : "Kod B - pierwsza asysta";
            CompletionStatus = $"Wykonane: {procedure.CompletedCount}/{procedure.RequiredCount}";

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
                            InternshipName = internship.Name;
                            // Update UI on main thread
                            MainThread.BeginInvokeOnMainThread(() =>
                            {
                                OnPropertyChanged(nameof(InternshipName));
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
                InternshipName = InternshipName,
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
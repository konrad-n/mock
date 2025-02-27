using System;
using Microsoft.Maui.Controls;
using SledzSpecke.Models;

namespace SledzSpecke.App.Views
{
    public partial class ProcedureEntryPage : ContentPage
    {
        private MedicalProcedure _procedure;
        private Action<MedicalProcedure, ProcedureEntry> _onSaveCallback;



        public string ProcedureName { get; set; }
        public string ProcedureType { get; set; }
        public string CompletionStatus { get; set; }
        public DateTime EntryDate { get; set; } = DateTime.Now;
        public string PatientId { get; set; }
        public string Location { get; set; }
        public string SupervisorName { get; set; }
        public string Notes { get; set; }

        public ProcedureEntryPage(MedicalProcedure procedure, Action<MedicalProcedure, ProcedureEntry> onSaveCallback)
        {
            InitializeComponent();
            _procedure = procedure;
            _onSaveCallback = onSaveCallback;

            ProcedureName = procedure.Name;
            ProcedureType = procedure.ProcedureType == ProcedureType.TypeA
                ? "Kod A - wykonywanie samodzielne"
                : "Kod B - pierwsza asysta";
            CompletionStatus = $"Wykonane: {procedure.CompletedCount}/{procedure.RequiredCount}";

            BindingContext = this;
        }

        private async void OnCancelClicked(object sender, EventArgs e)
        {
            await Navigation.PopAsync();
        }

        private async void OnSaveClicked(object sender, EventArgs e)
        {
            // Walidacja
            if (string.IsNullOrWhiteSpace(PatientId))
            {
                await DisplayAlert("Błąd", "Identyfikator pacjenta jest wymagany.", "OK");
                return;
            }

            if (string.IsNullOrWhiteSpace(Location))
            {
                await DisplayAlert("Błąd", "Miejsce wykonania jest wymagane.", "OK");
                return;
            }

            // Dla typu B (asysta) wymagany jest nadzorujący
            if (_procedure.ProcedureType == ProcedureType.TypeB && string.IsNullOrWhiteSpace(SupervisorName))
            {
                await DisplayAlert("Błąd", "Imię i nazwisko nadzorującego jest wymagane dla procedury typu B.", "OK");
                return;
            }

            // Tworzenie nowego wpisu
            var procedureEntry = new ProcedureEntry
            {
                Id = new Random().Next(1000, 9999), // Tymczasowe ID
                Date = EntryDate,
                PatientId = PatientId,
                Location = Location,
                SupervisorName = SupervisorName,
                Notes = Notes
            };

            _onSaveCallback?.Invoke(_procedure, procedureEntry);
            await Navigation.PopAsync();
        }
    }
}
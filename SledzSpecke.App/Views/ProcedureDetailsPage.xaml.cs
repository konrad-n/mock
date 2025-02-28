using SledzSpecke.Core.Models;
using SledzSpecke.Core.Models.Enums;
using SledzSpecke.Infrastructure.Database.Initialization;

namespace SledzSpecke.App.Views
{
    public partial class ProcedureDetailsPage : ContentPage
    {
        private MedicalProcedure _procedure;
        private ModuleType _currentModule;
        private ProcedureType _currentProcedureType;
        private Func<MedicalProcedure, Task> _onSaveCallback;
        private List<Internship> _internships;

        public string PageTitle { get; set; }
        public string RequiredCount { get; set; }
        public string CompletedCount { get; set; }
        public string Notes { get; set; }

        public MedicalProcedure Procedure => _procedure;

        public ProcedureDetailsPage(MedicalProcedure procedure, ModuleType currentModule, ProcedureType currentProcedureType, Func<MedicalProcedure, Task> onSaveCallback)
        {
            InitializeComponent();
            _currentModule = currentModule;
            _currentProcedureType = currentProcedureType;
            _onSaveCallback = onSaveCallback;
            _internships = DataSeeder.SeedHematologySpecialization().RequiredInternships.ToList();

            if (procedure == null)
            {
                // Nowa procedura
                _procedure = new MedicalProcedure
                {
                    Id = new Random().Next(1000, 9999), // Tymczasowe ID
                    Module = currentModule,
                    ProcedureType = currentProcedureType,
                    CompletedCount = 0
                };
                PageTitle = "Dodaj procedurę";
                RequiredCount = "1";
                CompletedCount = "0";
            }
            else
            {
                // Edycja istniejącej procedury
                _procedure = procedure;
                PageTitle = "Szczegóły procedury";
                RequiredCount = procedure.RequiredCount.ToString();
                CompletedCount = procedure.CompletedCount.ToString();
                Notes = procedure.Description;
            }

            // Ustawienie pickerów
            ProcedureTypePicker.SelectedIndex = _procedure.ProcedureType == ProcedureType.TypeA ? 0 : 1;
            ModulePicker.SelectedIndex = _procedure.Module == ModuleType.Basic ? 0 : 1;

            // Wypełnienie pickera stażów
            LoadInternships(_procedure.Module);

            // Jeśli edytujemy procedurę, ustawiamy wybrany staż
            if (_procedure.InternshipId.HasValue)
            {
                int index = _internships.FindIndex(i => i.Id == _procedure.InternshipId.Value);
                if (index >= 0)
                {
                    InternshipPicker.SelectedIndex = index;
                }
            }

            BindingContext = this;
        }

        private void LoadInternships(ModuleType moduleType)
        {
            InternshipPicker.Items.Clear();

            var filteredInternships = _internships.Where(i => i.Module == moduleType).ToList();
            foreach (var internship in filteredInternships)
            {
                InternshipPicker.Items.Add(internship.Name);
            }
        }

        private void OnProcedureTypePickerSelectedIndexChanged(object sender, EventArgs e)
        {
            _procedure.ProcedureType = ProcedureTypePicker.SelectedIndex == 0 ? ProcedureType.TypeA : ProcedureType.TypeB;
        }

        private void OnModulePickerSelectedIndexChanged(object sender, EventArgs e)
        {
            _procedure.Module = ModulePicker.SelectedIndex == 0 ? ModuleType.Basic : ModuleType.Specialistic;
            LoadInternships(_procedure.Module);
        }

        private void OnInternshipPickerSelectedIndexChanged(object sender, EventArgs e)
        {
            if (InternshipPicker.SelectedIndex >= 0)
            {
                var filteredInternships = _internships.Where(i => i.Module == _procedure.Module).ToList();
                if (InternshipPicker.SelectedIndex < filteredInternships.Count)
                {
                    _procedure.InternshipId = filteredInternships[InternshipPicker.SelectedIndex].Id;
                }
            }
        }

        private async void OnCancelClicked(object sender, EventArgs e)
        {
            await Navigation.PopAsync();
        }

        private async void OnSaveClicked(object sender, EventArgs e)
        {
            // Walidacja
            if (string.IsNullOrWhiteSpace(_procedure.Name))
            {
                await DisplayAlert("Błąd", "Nazwa procedury jest wymagana.", "OK");
                return;
            }

            if (!int.TryParse(RequiredCount, out int requiredCount) || requiredCount <= 0)
            {
                await DisplayAlert("Błąd", "Wprowadź poprawną wymaganą liczbę procedur.", "OK");
                return;
            }

            if (InternshipPicker.SelectedIndex < 0)
            {
                await DisplayAlert("Błąd", "Wybierz staż, w ramach którego wykonywana jest procedura.", "OK");
                return;
            }

            _procedure.RequiredCount = requiredCount;
            _procedure.Description = Notes;

            if (_onSaveCallback != null)
            {
                await _onSaveCallback(_procedure);
            }

            await Navigation.PopAsync();
        }
    }
}
using SledzSpecke.App.Common.Views;
using SledzSpecke.App.Features.Procedures.ViewModels;
using SledzSpecke.Core.Models;
using SledzSpecke.Core.Models.Enums;
using SledzSpecke.Infrastructure.Database.Initialization;

namespace SledzSpecke.App.Features.Procedures.Views
{
    public partial class ProcedureDetailsPage : BaseContentPage
    {
        private ProcedureDetailsViewModel _viewModel;
        private MedicalProcedure _procedure;
        private ModuleType _currentModule;
        private ProcedureType _currentProcedureType;
        private Func<MedicalProcedure, Task> _onSaveCallback;
        private List<Internship> _internships;

        public ProcedureDetailsPage(MedicalProcedure procedure, ModuleType currentModule, ProcedureType currentProcedureType, Func<MedicalProcedure, Task> onSaveCallback)
        {
            InitializeComponent();
            _procedure = procedure;
            _currentModule = currentModule;
            _currentProcedureType = currentProcedureType;
            _onSaveCallback = onSaveCallback;
            _internships = DataSeeder.SeedHematologySpecialization().RequiredInternships.ToList();
        }

        protected override async Task InitializePageAsync()
        {
            try
            {
                _viewModel = GetRequiredService<ProcedureDetailsViewModel>();
                // Najpierw inicjalizujemy ViewModel
                _viewModel.Initialize(_procedure, _currentModule, _currentProcedureType, _onSaveCallback, _internships);
                // Dopiero potem ustawiamy BindingContext
                BindingContext = _viewModel;
            }
            catch (Exception ex)
            {
                await DisplayAlert("Błąd", "Nie udało się zainicjalizować strony szczegółów procedury.", "OK");
                System.Diagnostics.Debug.WriteLine($"Error in ProcedureDetailsPage: {ex}");
            }
        }

        private void OnProcedureTypePickerSelectedIndexChanged(object sender, EventArgs e)
        {
            if (sender is Picker picker && _viewModel != null && _viewModel.Procedure != null)
            {
                _viewModel.UpdateProcedureTypeCommand.Execute(picker.SelectedIndex);
            }
        }

        private void OnModulePickerSelectedIndexChanged(object sender, EventArgs e)
        {
            if (sender is Picker picker && _viewModel != null && _viewModel.Procedure != null)
            {
                _viewModel.UpdateModuleCommand.Execute(picker.SelectedIndex);
            }
        }

        private void OnInternshipPickerSelectedIndexChanged(object sender, EventArgs e)
        {
            if (sender is Picker picker && _viewModel != null && _viewModel.Procedure != null)
            {
                _viewModel.UpdateInternshipCommand.Execute(picker.SelectedIndex);
            }
        }
    }
}
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
            this.InitializeComponent();
            this._procedure = procedure;
            this._currentModule = currentModule;
            this._currentProcedureType = currentProcedureType;
            this._onSaveCallback = onSaveCallback;
            this._internships = DataSeeder.SeedHematologySpecialization().RequiredInternships.ToList();
        }

        protected override async Task InitializePageAsync()
        {
            try
            {
                this._viewModel = this.GetRequiredService<ProcedureDetailsViewModel>();
                this.BindingContext = this._viewModel;

                // Initialize ViewModel with procedure and callback
                this._viewModel.Initialize(this._procedure, this._currentModule, this._currentProcedureType, this._onSaveCallback, this._internships);
            }
            catch (Exception ex)
            {
                await this.DisplayAlert("Błąd", "Nie udało się zainicjalizować formularza procedury.", "OK");
                System.Diagnostics.Debug.WriteLine($"Error in ProcedureDetailsPage: {ex}");
            }
        }

        private void OnProcedureTypePickerSelectedIndexChanged(object sender, EventArgs e)
        {
            if (sender is Picker picker && this._viewModel != null && this._viewModel.Procedure != null)
            {
                this._viewModel.UpdateProcedureTypeCommand.Execute(picker.SelectedIndex);
            }
        }

        private void OnModulePickerSelectedIndexChanged(object sender, EventArgs e)
        {
            if (sender is Picker picker && this._viewModel != null && this._viewModel.Procedure != null)
            {
                this._viewModel.UpdateModuleCommand.Execute(picker.SelectedIndex);
            }
        }

        private void OnInternshipPickerSelectedIndexChanged(object sender, EventArgs e)
        {
            if (sender is Picker picker && this._viewModel != null && this._viewModel.Procedure != null)
            {
                this._viewModel.UpdateInternshipCommand.Execute(picker.SelectedIndex);
            }
        }
    }
}
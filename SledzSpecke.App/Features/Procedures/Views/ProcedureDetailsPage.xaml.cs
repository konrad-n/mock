using SledzSpecke.App.Common.Views;
using SledzSpecke.App.Features.Procedures.ViewModels;
using SledzSpecke.Core.Models;
using SledzSpecke.Core.Models.Enums;
using SledzSpecke.Infrastructure.Database.Initialization;

namespace SledzSpecke.App.Features.Procedures.Views
{
    public partial class ProcedureDetailsPage : BaseContentPage
    {
        private readonly MedicalProcedure procedure;
        private readonly ModuleType currentModule;
        private readonly ProcedureType currentProcedureType;
        private readonly Func<MedicalProcedure, Task> onSaveCallback;
        private readonly List<Internship> internships;
        private ProcedureDetailsViewModel viewModel;

        public ProcedureDetailsPage(MedicalProcedure procedure, ModuleType currentModule, ProcedureType currentProcedureType, Func<MedicalProcedure, Task> onSaveCallback)
        {
            this.InitializeComponent();
            this.procedure = procedure;
            this.currentModule = currentModule;
            this.currentProcedureType = currentProcedureType;
            this.onSaveCallback = onSaveCallback;
            this.internships = DataSeeder.SeedHematologySpecialization().RequiredInternships.ToList();
        }

        protected override async Task InitializePageAsync()
        {
            try
            {
                this.viewModel = this.GetRequiredService<ProcedureDetailsViewModel>();
                this.BindingContext = this.viewModel;

                // Initialize ViewModel with procedure and callback
                this.viewModel.Initialize(this.procedure, this.currentModule, this.currentProcedureType, this.onSaveCallback, this.internships);
            }
            catch (Exception ex)
            {
                await this.DisplayAlert("Błąd", "Nie udało się zainicjalizować formularza procedury.", "OK");
                System.Diagnostics.Debug.WriteLine($"Error in ProcedureDetailsPage: {ex}");
            }
        }

        private void OnProcedureTypePickerSelectedIndexChanged(object sender, EventArgs e)
        {
            if (sender is Picker picker && this.viewModel != null && this.viewModel.Procedure != null)
            {
                this.viewModel.UpdateProcedureTypeCommand.Execute(picker.SelectedIndex);
            }
        }

        private void OnModulePickerSelectedIndexChanged(object sender, EventArgs e)
        {
            if (sender is Picker picker && this.viewModel != null && this.viewModel.Procedure != null)
            {
                this.viewModel.UpdateModuleCommand.Execute(picker.SelectedIndex);
            }
        }

        private void OnInternshipPickerSelectedIndexChanged(object sender, EventArgs e)
        {
            if (sender is Picker picker && this.viewModel != null && this.viewModel.Procedure != null)
            {
                this.viewModel.UpdateInternshipCommand.Execute(picker.SelectedIndex);
            }
        }
    }
}
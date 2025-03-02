using SledzSpecke.App.Common.Views;
using SledzSpecke.App.Features.Procedures.ViewModels;
using SledzSpecke.Core.Models;
using SledzSpecke.Infrastructure.Database;

namespace SledzSpecke.App.Features.Procedures.Views
{
    public partial class ProcedureEntryPage : BaseContentPage
    {
        private readonly MedicalProcedure procedure;
        private readonly Func<MedicalProcedure, ProcedureEntry, Task> onSaveCallback;
        private ProcedureEntryViewModel viewModel;

        public ProcedureEntryPage(
            IDatabaseService databaseService,
            MedicalProcedure procedure,
            Func<MedicalProcedure, ProcedureEntry, Task> onSaveCallback)
        {
            this.InitializeComponent();
            this.procedure = procedure;
            this.onSaveCallback = onSaveCallback;
        }

        protected override async Task InitializePageAsync()
        {
            try
            {
                this.viewModel = this.GetRequiredService<ProcedureEntryViewModel>();
                this.BindingContext = this.viewModel;

                // Initialize ViewModel with procedure and callback
                this.viewModel.Initialize(this.procedure, this.onSaveCallback);
            }
            catch (Exception ex)
            {
                await this.DisplayAlert("Błąd", "Nie udało się zainicjalizować formularza procedury.", "OK");
                System.Diagnostics.Debug.WriteLine($"Error in ProcedureEntryPage: {ex}");
            }
        }
    }
}
using SledzSpecke.App.Common.Views;
using SledzSpecke.App.Features.Procedures.ViewModels;
using SledzSpecke.Core.Models;
using SledzSpecke.Infrastructure.Database;

namespace SledzSpecke.App.Features.Procedures.Views
{
    public partial class ProcedureEntryPage : BaseContentPage
    {
        private ProcedureEntryViewModel _viewModel;
        private readonly MedicalProcedure _procedure;
        private readonly Func<MedicalProcedure, ProcedureEntry, Task> _onSaveCallback;

        public ProcedureEntryPage(
            IDatabaseService databaseService,
            MedicalProcedure procedure,
            Func<MedicalProcedure, ProcedureEntry, Task> onSaveCallback)
        {
            this.InitializeComponent();
            this._procedure = procedure;
            this._onSaveCallback = onSaveCallback;
        }

        protected override async Task InitializePageAsync()
        {
            try
            {
                this._viewModel = this.GetRequiredService<ProcedureEntryViewModel>();
                this.BindingContext = this._viewModel;

                // Initialize ViewModel with procedure and callback
                this._viewModel.Initialize(this._procedure, this._onSaveCallback);
            }
            catch (Exception ex)
            {
                await this.DisplayAlert("Błąd", "Nie udało się zainicjalizować formularza procedury.", "OK");
                System.Diagnostics.Debug.WriteLine($"Error in ProcedureEntryPage: {ex}");
            }
        }
    }
}
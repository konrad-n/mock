using SledzSpecke.App.Common.Views;
using SledzSpecke.App.Features.Procedures.ViewModels;
using SledzSpecke.Core.Models;
using SledzSpecke.Infrastructure.Database;

namespace SledzSpecke.App.Features.Procedures.Views
{
    public partial class ProcedureEntryPage : BaseContentPage
    {
        private ProcedureEntryViewModel _viewModel;
        private MedicalProcedure _procedure;
        private Func<MedicalProcedure, ProcedureEntry, Task> _onSaveCallback;
        private IDatabaseService _databaseService;

        public ProcedureEntryPage(
            IDatabaseService databaseService,
            MedicalProcedure procedure,
            Func<MedicalProcedure, ProcedureEntry, Task> onSaveCallback)
        {
            InitializeComponent();
            _databaseService = databaseService;
            _procedure = procedure;
            _onSaveCallback = onSaveCallback;
        }

        protected override async Task InitializePageAsync()
        {
            try
            {
                _viewModel = GetRequiredService<ProcedureEntryViewModel>();
                BindingContext = _viewModel;

                // Initialize ViewModel with procedure and callback
                _viewModel.Initialize(_procedure, _onSaveCallback);
            }
            catch (Exception ex)
            {
                await DisplayAlert("Błąd", "Nie udało się zainicjalizować formularza procedury.", "OK");
                System.Diagnostics.Debug.WriteLine($"Error in ProcedureEntryPage: {ex}");
            }
        }
    }
}
using SledzSpecke.App.Common.Views;
using SledzSpecke.App.Features.Absences.ViewModels;
using SledzSpecke.Core.Models;
using SledzSpecke.Infrastructure.Database;

namespace SledzSpecke.App.Features.Absences.Views
{
    public partial class AbsenceDetailsPage : BaseContentPage
    {
        private AbsenceDetailsViewModel _viewModel;
        private Absence _absence;
        private IDatabaseService _databaseService;
        private Action<Absence> _onSaveCallback;

        public AbsenceDetailsPage(
            IDatabaseService databaseService,
            Absence absence,
            Action<Absence> onSaveCallback)
        {
            InitializeComponent();
            _databaseService = databaseService;
            _absence = absence;
            _onSaveCallback = onSaveCallback;
        }

        protected override async Task InitializePageAsync()
        {
            try
            {
                _viewModel = GetRequiredService<AbsenceDetailsViewModel>();
                _viewModel.Initialize(_absence, _onSaveCallback);
                BindingContext = _viewModel;
            }
            catch (Exception ex)
            {
                await DisplayAlert("Błąd", "Nie udało się zainicjalizować strony szczegółów nieobecności.", "OK");
                System.Diagnostics.Debug.WriteLine($"Error in AbsenceDetailsPage: {ex}");
            }
        }

        private void OnAbsenceTypeChanged(object sender, EventArgs e)
        {
            if (sender is Picker picker && _viewModel != null)
            {
                _viewModel.UpdateAbsenceTypeCommand.Execute(picker.SelectedIndex);
            }
        }

        private void OnDateSelected(object sender, DateChangedEventArgs e)
        {
            if (_viewModel != null)
            {
                _viewModel.CalculateDuration();
            }
        }
    }
}
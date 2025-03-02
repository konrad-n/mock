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
            this.InitializeComponent();
            this._databaseService = databaseService;
            this._absence = absence;
            this._onSaveCallback = onSaveCallback;
        }

        protected override async Task InitializePageAsync()
        {
            try
            {
                this._viewModel = this.GetRequiredService<AbsenceDetailsViewModel>();
                this._viewModel.Initialize(this._absence, this._onSaveCallback);
                this.BindingContext = this._viewModel;
            }
            catch (Exception ex)
            {
                await this.DisplayAlert("Błąd", "Nie udało się zainicjalizować strony szczegółów nieobecności.", "OK");
                System.Diagnostics.Debug.WriteLine($"Error in AbsenceDetailsPage: {ex}");
            }
        }

        private void OnAbsenceTypeChanged(object sender, EventArgs e)
        {
            if (sender is Picker picker && this._viewModel != null)
            {
                this._viewModel.UpdateAbsenceTypeCommand.Execute(picker.SelectedIndex);
            }
        }

        private void OnDateSelected(object sender, DateChangedEventArgs e)
        {
            if (this._viewModel != null)
            {
                this._viewModel.CalculateDuration();
            }
        }
    }
}
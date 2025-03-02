using SledzSpecke.App.Common.Views;
using SledzSpecke.App.Features.Duties.ViewModels;
using SledzSpecke.Core.Models;

namespace SledzSpecke.App.Features.Duties.Views
{
    public partial class DutyShiftDetailsPage : BaseContentPage
    {
        private DutyShiftDetailsViewModel _viewModel;
        private readonly DutyShift _dutyShift;
        private readonly Func<DutyShift, Task> _onSaveCallback;

        public DutyShiftDetailsPage(DutyShift dutyShift, Func<DutyShift, Task> onSaveCallback)
        {
            this.InitializeComponent();
            this._dutyShift = dutyShift;
            this._onSaveCallback = onSaveCallback;
        }

        protected override async Task InitializePageAsync()
        {
            try
            {
                this._viewModel = this.GetRequiredService<DutyShiftDetailsViewModel>();

                // Najpierw inicjalizujemy ViewModel
                this._viewModel.Initialize(this._dutyShift, this._onSaveCallback);

                // Dopiero potem ustawiamy BindingContext
                this.BindingContext = this._viewModel;
            }
            catch (Exception ex)
            {
                await this.DisplayAlert("Błąd", "Nie udało się zainicjalizować strony szczegółów dyżuru.", "OK");
                System.Diagnostics.Debug.WriteLine($"Error in DutyShiftDetailsPage: {ex}");
            }
        }

        private void OnDutyTypePickerSelectedIndexChanged(object sender, EventArgs e)
        {
            if (sender is Picker picker && this._viewModel != null)
            {
                this._viewModel.UpdateDutyTypeCommand.Execute(picker.SelectedIndex);
            }
        }

        private void OnDateTimeChanged(object sender, EventArgs e)
        {
            if (this._viewModel != null)
            {
                this._viewModel.UpdateDurationText();
            }
        }
    }
}
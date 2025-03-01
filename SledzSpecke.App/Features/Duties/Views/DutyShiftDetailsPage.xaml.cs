using SledzSpecke.App.Common.Views;
using SledzSpecke.App.Features.Duties.ViewModels;
using SledzSpecke.Core.Models;

namespace SledzSpecke.App.Features.Duties.Views
{
    public partial class DutyShiftDetailsPage : BaseContentPage
    {
        private DutyShiftDetailsViewModel _viewModel;
        private DutyShift _dutyShift;
        private Func<DutyShift, Task> _onSaveCallback;

        public DutyShiftDetailsPage(DutyShift dutyShift, Func<DutyShift, Task> onSaveCallback)
        {
            InitializeComponent();
            _dutyShift = dutyShift;
            _onSaveCallback = onSaveCallback;
        }

        protected override async Task InitializePageAsync()
        {
            try
            {
                _viewModel = GetRequiredService<DutyShiftDetailsViewModel>();

                // Najpierw inicjalizujemy ViewModel
                _viewModel.Initialize(_dutyShift, _onSaveCallback);

                // Dopiero potem ustawiamy BindingContext
                BindingContext = _viewModel;
            }
            catch (Exception ex)
            {
                await DisplayAlert("Błąd", "Nie udało się zainicjalizować strony szczegółów dyżuru.", "OK");
                System.Diagnostics.Debug.WriteLine($"Error in DutyShiftDetailsPage: {ex}");
            }
        }

        private void OnDutyTypePickerSelectedIndexChanged(object sender, EventArgs e)
        {
            if (sender is Picker picker && _viewModel != null)
            {
                _viewModel.UpdateDutyTypeCommand.Execute(picker.SelectedIndex);
            }
        }

        private void OnDateTimeChanged(object sender, EventArgs e)
        {
            if (_viewModel != null)
            {
                _viewModel.UpdateDurationText();
            }
        }
    }
}
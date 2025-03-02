using SledzSpecke.App.Common.Views;
using SledzSpecke.App.Features.Duties.ViewModels;
using SledzSpecke.Core.Models;

namespace SledzSpecke.App.Features.Duties.Views
{
    public partial class DutyShiftDetailsPage : BaseContentPage
    {
        private DutyShiftDetailsViewModel viewModel;
        private readonly DutyShift dutyShift;
        private readonly Func<DutyShift, Task> onSaveCallback;

        public DutyShiftDetailsPage(DutyShift dutyShift, Func<DutyShift, Task> onSaveCallback)
        {
            this.InitializeComponent();
            this.dutyShift = dutyShift;
            this.onSaveCallback = onSaveCallback;
        }

        protected override async Task InitializePageAsync()
        {
            try
            {
                this.viewModel = this.GetRequiredService<DutyShiftDetailsViewModel>();
                this.viewModel.Initialize(this.dutyShift, this.onSaveCallback);
                this.BindingContext = this.viewModel;
            }
            catch (Exception ex)
            {
                await this.DisplayAlert("Błąd", "Nie udało się zainicjalizować strony szczegółów dyżuru.", "OK");
                System.Diagnostics.Debug.WriteLine($"Error in DutyShiftDetailsPage: {ex}");
            }
        }

        private void OnDutyTypePickerSelectedIndexChanged(object sender, EventArgs e)
        {
            if (sender is Picker picker && this.viewModel != null)
            {
                this.viewModel.UpdateDutyTypeCommand.Execute(picker.SelectedIndex);
            }
        }

        private void OnDateTimeChanged(object sender, EventArgs e)
        {
            if (this.viewModel != null)
            {
                this.viewModel.UpdateDurationText();
            }
        }
    }
}
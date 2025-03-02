using SledzSpecke.App.Common.Views;
using SledzSpecke.App.Features.Absences.ViewModels;
using SledzSpecke.Core.Models;

namespace SledzSpecke.App.Features.Absences.Views
{
    public partial class AbsenceDetailsPage : BaseContentPage
    {
        private readonly Absence? absence;
        private readonly Action<Absence> onSaveCallback;
        private AbsenceDetailsViewModel viewModel = null!;

        public AbsenceDetailsPage(
            Absence? absence,
            Action<Absence> onSaveCallback)
        {
            this.InitializeComponent();
            this.absence = absence;
            this.onSaveCallback = onSaveCallback;
        }

        protected override async Task InitializePageAsync()
        {
            try
            {
                this.viewModel = this.GetRequiredService<AbsenceDetailsViewModel>();
                this.viewModel.Initialize(this.absence, this.onSaveCallback);
                this.BindingContext = this.viewModel;
            }
            catch (Exception ex)
            {
                await this.DisplayAlert("Błąd", "Nie udało się zainicjalizować strony szczegółów nieobecności.", "OK");
                System.Diagnostics.Debug.WriteLine($"Error in AbsenceDetailsPage: {ex}");
            }
        }

        private void OnAbsenceTypeChanged(object sender, EventArgs e)
        {
            if (sender is Picker picker && this.viewModel is not null)
            {
                this.viewModel.UpdateAbsenceTypeCommand.Execute(picker.SelectedIndex);
            }
        }

        private void OnDateSelected(object sender, DateChangedEventArgs e)
        {
            this.viewModel?.CalculateDuration();
        }
    }
}
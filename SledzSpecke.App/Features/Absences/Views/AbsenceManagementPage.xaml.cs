using SledzSpecke.App.Common.Views;
using SledzSpecke.App.Features.Absences.ViewModels;

namespace SledzSpecke.App.Features.Absences.Views
{
    public partial class AbsenceManagementPage : BaseContentPage
    {
        private AbsenceManagementViewModel viewModel = null!;

        public AbsenceManagementPage()
        {
            this.InitializeComponent();
        }

        protected override async Task InitializePageAsync()
        {
            try
            {
                this.viewModel = this.GetRequiredService<AbsenceManagementViewModel>();
                this.BindingContext = this.viewModel;
                await this.viewModel.InitializeAsync();
            }
            catch (Exception ex)
            {
                await this.DisplayAlert("Blad", "Nie udalo sie zaladowac danych nieobecnosci.", "OK");
                System.Diagnostics.Debug.WriteLine($"Error in AbsenceManagementPage: {ex}");
            }
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();
            this.viewModel?.LoadDataAsync().ConfigureAwait(false);
        }
        private void OnFilterTypeChanged(object sender, EventArgs e)
        {
            if (sender is Picker picker)
            {
                this.viewModel.FilterByAbsenceTypeCommand.Execute(picker.SelectedIndex);
            }
        }

        private void OnFilterYearChanged(object sender, EventArgs e)
        {
            if (sender is Picker picker)
            {
                this.viewModel.FilterByYearCommand.Execute(picker.SelectedIndex);
            }
        }

        private void OnEditButtonClicked(object sender, EventArgs e)
        {
            if (sender is Button button && int.TryParse(button.ClassId, out int absenceId))
            {
                this.viewModel.EditAbsenceCommand.Execute(absenceId);
            }
        }
    }
}

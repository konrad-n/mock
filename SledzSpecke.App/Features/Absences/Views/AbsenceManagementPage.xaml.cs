using SledzSpecke.App.Common.Views;
using SledzSpecke.App.Features.Absences.ViewModels;

namespace SledzSpecke.App.Features.Absences.Views
{
    public partial class AbsenceManagementPage : BaseContentPage
    {
        private AbsenceManagementViewModel _viewModel;

        public AbsenceManagementPage()
        {
            this.InitializeComponent();
        }

        protected override async Task InitializePageAsync()
        {
            try
            {
                this._viewModel = this.GetRequiredService<AbsenceManagementViewModel>();
                this.BindingContext = this._viewModel;
                await this._viewModel.InitializeAsync();
            }
            catch (Exception ex)
            {
                await this.DisplayAlert("Błąd", "Nie udało się załadować danych nieobecności.", "OK");
                System.Diagnostics.Debug.WriteLine($"Error in AbsenceManagementPage: {ex}");
            }
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();

            // Refresh data when page appears
            if (this._viewModel != null)
            {
                this._viewModel.LoadDataAsync().ConfigureAwait(false);
            }
        }

        private void OnFilterTypeChanged(object sender, EventArgs e)
        {
            if (sender is Picker picker)
            {
                this._viewModel.FilterByAbsenceTypeCommand.Execute(picker.SelectedIndex);
            }
        }

        private void OnFilterYearChanged(object sender, EventArgs e)
        {
            if (sender is Picker picker)
            {
                this._viewModel.FilterByYearCommand.Execute(picker.SelectedIndex);
            }
        }
    }
}
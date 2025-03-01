using SledzSpecke.App.Common.Views;
using SledzSpecke.App.Features.Absences.ViewModels;

namespace SledzSpecke.App.Features.Absences.Views
{
    public partial class AbsenceManagementPage : BaseContentPage
    {
        private AbsenceManagementViewModel _viewModel;

        public AbsenceManagementPage()
        {
            InitializeComponent();
        }

        protected override async Task InitializePageAsync()
        {
            try
            {
                _viewModel = GetRequiredService<AbsenceManagementViewModel>();
                BindingContext = _viewModel;
                await _viewModel.InitializeAsync();
            }
            catch (Exception ex)
            {
                await DisplayAlert("Błąd", "Nie udało się załadować danych nieobecności.", "OK");
                System.Diagnostics.Debug.WriteLine($"Error in AbsenceManagementPage: {ex}");
            }
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();

            // Refresh data when page appears
            if (_viewModel != null)
            {
                _viewModel.LoadDataAsync().ConfigureAwait(false);
            }
        }

        private void OnFilterTypeChanged(object sender, EventArgs e)
        {
            if (sender is Picker picker)
            {
                _viewModel.FilterByAbsenceTypeCommand.Execute(picker.SelectedIndex);
            }
        }

        private void OnFilterYearChanged(object sender, EventArgs e)
        {
            if (sender is Picker picker)
            {
                _viewModel.FilterByYearCommand.Execute(picker.SelectedIndex);
            }
        }
    }
}
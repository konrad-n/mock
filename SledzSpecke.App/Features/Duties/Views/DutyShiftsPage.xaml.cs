using SledzSpecke.App.Common.Views;
using SledzSpecke.App.Features.Duties.ViewModels;

namespace SledzSpecke.App.Features.Duties.Views
{
    public partial class DutyShiftsPage : BaseContentPage
    {
        private DutyShiftsViewModel _viewModel;

        public DutyShiftsPage()
        {
            InitializeComponent();
        }

        protected override async Task InitializePageAsync()
        {
            try
            {
                _viewModel = GetRequiredService<DutyShiftsViewModel>();
                BindingContext = _viewModel;
                await _viewModel.InitializeAsync();
            }
            catch (Exception ex)
            {
                await DisplayAlert("Błąd", "Nie udało się załadować dyżurów.", "OK");
                System.Diagnostics.Debug.WriteLine($"Error in DutyShiftsPage: {ex}");
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
    }
}
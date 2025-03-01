using SledzSpecke.App.Common.Views;
using SledzSpecke.App.Features.Dashboard.ViewModels;

namespace SledzSpecke.App.Features.Dashboard.Views
{
    public partial class DashboardPage : BaseContentPage
    {
        private DashboardViewModel _viewModel;

        public DashboardPage()
        {
            InitializeComponent();
        }

        protected override async Task InitializePageAsync()
        {
            try
            {
                _viewModel = GetRequiredService<DashboardViewModel>();
                BindingContext = _viewModel;
                await _viewModel.InitializeAsync();
            }
            catch (Exception ex)
            {
                await DisplayAlert("Błąd", "Nie udało się załadować danych na pulpicie.", "OK");
                System.Diagnostics.Debug.WriteLine($"Error in DashboardPage: {ex}");
            }
        }
    }
}
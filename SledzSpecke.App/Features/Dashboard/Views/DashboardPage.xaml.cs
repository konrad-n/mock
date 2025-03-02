using SledzSpecke.App.Common.Views;
using SledzSpecke.App.Features.Dashboard.ViewModels;

namespace SledzSpecke.App.Features.Dashboard.Views
{
    public partial class DashboardPage : BaseContentPage
    {
        private DashboardViewModel _viewModel;

        public DashboardPage()
        {
            this.InitializeComponent();
        }

        protected override async Task InitializePageAsync()
        {
            try
            {
                this._viewModel = this.GetRequiredService<DashboardViewModel>();
                this.BindingContext = this._viewModel;
                await this._viewModel.InitializeAsync();
            }
            catch (Exception ex)
            {
                await this.DisplayAlert("Błąd", "Nie udało się załadować danych na pulpicie.", "OK");
                System.Diagnostics.Debug.WriteLine($"Error in DashboardPage: {ex}");
            }
        }
    }
}
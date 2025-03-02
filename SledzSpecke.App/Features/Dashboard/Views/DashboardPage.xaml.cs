using SledzSpecke.App.Common.Views;
using SledzSpecke.App.Features.Dashboard.ViewModels;

namespace SledzSpecke.App.Features.Dashboard.Views
{
    public partial class DashboardPage : BaseContentPage
    {
        private DashboardViewModel viewModel;

        public DashboardPage()
        {
            this.InitializeComponent();
        }

        protected override async Task InitializePageAsync()
        {
            try
            {
                this.viewModel = this.GetRequiredService<DashboardViewModel>();
                this.BindingContext = this.viewModel;
                await this.viewModel.InitializeAsync();
            }
            catch (Exception ex)
            {
                await this.DisplayAlert("Blad", "Nie udalo sie zaladowac danych na pulpicie.", "OK");
                System.Diagnostics.Debug.WriteLine($"Error in DashboardPage: {ex}");
            }
        }
    }
}

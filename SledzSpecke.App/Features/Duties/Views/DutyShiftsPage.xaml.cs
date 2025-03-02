using SledzSpecke.App.Common.Views;
using SledzSpecke.App.Features.Duties.ViewModels;

namespace SledzSpecke.App.Features.Duties.Views
{
    public partial class DutyShiftsPage : BaseContentPage
    {
        private DutyShiftsViewModel _viewModel;

        public DutyShiftsPage()
        {
            this.InitializeComponent();
        }

        protected override async Task InitializePageAsync()
        {
            try
            {
                this._viewModel = this.GetRequiredService<DutyShiftsViewModel>();
                this.BindingContext = this._viewModel;
                await this._viewModel.InitializeAsync();
            }
            catch (Exception ex)
            {
                await this.DisplayAlert("Błąd", "Nie udało się załadować dyżurów.", "OK");
                System.Diagnostics.Debug.WriteLine($"Error in DutyShiftsPage: {ex}");
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
    }
}
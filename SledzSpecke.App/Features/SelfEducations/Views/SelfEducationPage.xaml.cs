using SledzSpecke.App.Common.Views;
using SledzSpecke.App.Features.SelfEducations.ViewModels;

namespace SledzSpecke.App.Features.SelfEducations.Views
{
    public partial class SelfEducationPage : BaseContentPage
    {
        private SelfEducationViewModel _viewModel;

        public SelfEducationPage()
        {
            this.InitializeComponent();
        }

        protected override async Task InitializePageAsync()
        {
            try
            {
                this._viewModel = this.GetRequiredService<SelfEducationViewModel>();
                this.BindingContext = this._viewModel;
                await this._viewModel.InitializeAsync();
            }
            catch (Exception ex)
            {
                await this.DisplayAlert("Błąd", "Nie udało się załadować danych samokształcenia.", "OK");
                System.Diagnostics.Debug.WriteLine($"Error in SelfEducationPage: {ex}");
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
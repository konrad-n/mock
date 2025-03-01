using SledzSpecke.App.Common.Views;
using SledzSpecke.App.Features.SelfEducations.ViewModels;

namespace SledzSpecke.App.Features.SelfEducations.Views
{
    public partial class SelfEducationPage : BaseContentPage
    {
        private SelfEducationViewModel _viewModel;

        public SelfEducationPage()
        {
            InitializeComponent();
        }

        protected override async Task InitializePageAsync()
        {
            try
            {
                _viewModel = GetRequiredService<SelfEducationViewModel>();
                BindingContext = _viewModel;
                await _viewModel.InitializeAsync();
            }
            catch (Exception ex)
            {
                await DisplayAlert("Błąd", "Nie udało się załadować danych samokształcenia.", "OK");
                System.Diagnostics.Debug.WriteLine($"Error in SelfEducationPage: {ex}");
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
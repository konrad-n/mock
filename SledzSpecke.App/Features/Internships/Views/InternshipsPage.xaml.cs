using SledzSpecke.App.Common.Views;
using SledzSpecke.App.Features.Internships.ViewModels;

namespace SledzSpecke.App.Features.Internships.Views
{
    public partial class InternshipsPage : BaseContentPage
    {
        private InternshipsViewModel _viewModel;

        public InternshipsPage()
        {
            InitializeComponent();
        }

        protected override async Task InitializePageAsync()
        {
            try
            {
                _viewModel = GetRequiredService<InternshipsViewModel>();
                BindingContext = _viewModel;
                await _viewModel.InitializeAsync();
            }
            catch (Exception ex)
            {
                await DisplayAlert("Błąd", "Nie udało się załadować danych staży.", "OK");
                System.Diagnostics.Debug.WriteLine($"Error in InternshipsPage: {ex}");
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
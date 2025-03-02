using SledzSpecke.App.Common.Views;
using SledzSpecke.App.Features.SelfEducations.ViewModels;

namespace SledzSpecke.App.Features.SelfEducations.Views
{
    public partial class SelfEducationPage : BaseContentPage
    {
        private SelfEducationViewModel viewModel;

        public SelfEducationPage()
        {
            this.InitializeComponent();
        }

        protected override async Task InitializePageAsync()
        {
            try
            {
                this.viewModel = this.GetRequiredService<SelfEducationViewModel>();
                this.BindingContext = this.viewModel;
                await this.viewModel.InitializeAsync();
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

            if (this.viewModel != null)
            {
                this.viewModel.LoadDataAsync().ConfigureAwait(false);
            }
        }
    }
}
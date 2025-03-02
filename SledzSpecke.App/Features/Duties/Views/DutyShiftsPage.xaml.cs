using SledzSpecke.App.Common.Views;
using SledzSpecke.App.Features.Duties.ViewModels;

namespace SledzSpecke.App.Features.Duties.Views
{
    public partial class DutyShiftsPage : BaseContentPage
    {
        private DutyShiftsViewModel viewModel;

        public DutyShiftsPage()
        {
            this.InitializeComponent();
        }

        protected override async Task InitializePageAsync()
        {
            try
            {
                this.viewModel = this.GetRequiredService<DutyShiftsViewModel>();
                this.BindingContext = this.viewModel;
                await this.viewModel.InitializeAsync();
            }
            catch (Exception ex)
            {
                await this.DisplayAlert("Blad", "Nie udalo sie zaladowac dyzurów.", "OK");
                System.Diagnostics.Debug.WriteLine($"Error in DutyShiftsPage: {ex}");
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

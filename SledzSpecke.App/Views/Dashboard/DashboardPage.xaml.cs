using SledzSpecke.App.ViewModels.Dashboard;

namespace SledzSpecke.App.Views.Dashboard
{
    public partial class DashboardPage : ContentPage, IDisposable
    {
        private readonly DashboardViewModel viewModel;

        public DashboardPage()
        {
            this.InitializeComponent();
            this.viewModel = IPlatformApplication.Current.Services.GetService<SledzSpecke.App.ViewModels.Dashboard.DashboardViewModel>();
            this.BindingContext = this.viewModel;
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();

            if (this.BindingContext is SledzSpecke.App.ViewModels.Dashboard.DashboardViewModel viewModel)
            {
                viewModel.RefreshCommand?.Execute(null);
            }
        }

        protected override void OnDisappearing()
        {
            base.OnDisappearing();
        }

        public void Dispose()
        {
            (this.BindingContext as DashboardViewModel)?.Dispose();
        }
    }
}
using SledzSpecke.App.ViewModels.Dashboard;

namespace SledzSpecke.App.Views.Dashboard
{
    public partial class DashboardPage : ContentPage, IDisposable
    {
        private readonly DashboardViewModel viewModel;

        public DashboardPage()
        {
            this.InitializeComponent();

            // Pobranie ViewModel z kontenera DI
            this.viewModel = IPlatformApplication.Current.Services.GetService<SledzSpecke.App.ViewModels.Dashboard.DashboardViewModel>();
            this.BindingContext = this.viewModel;

            System.Diagnostics.Debug.WriteLine("DashboardPage: Konstruktor wywołany, BindingContext ustawiony");
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();
            System.Diagnostics.Debug.WriteLine("DashboardPage: OnAppearing");

            // Wykonaj odświeżenie danych przy każdym pokazaniu strony
            if (this.BindingContext is SledzSpecke.App.ViewModels.Dashboard.DashboardViewModel viewModel)
            {
                System.Diagnostics.Debug.WriteLine("DashboardPage: Odświeżanie danych...");
                viewModel.RefreshCommand?.Execute(null);
            }
        }

        protected override void OnDisappearing()
        {
            base.OnDisappearing();
            System.Diagnostics.Debug.WriteLine("DashboardPage: OnDisappearing");
        }

        public void Dispose()
        {
            // Zwolnij zasoby ViewModel przy zwalnianiu strony
            (this.BindingContext as DashboardViewModel)?.Dispose();
        }
    }
}
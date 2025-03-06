namespace SledzSpecke.App.Views.Dashboard
{
    public partial class DashboardPage : ContentPage
    {
        public DashboardPage()
        {
            this.InitializeComponent();

            // Pobranie ViewModel z kontenera DI
            this.BindingContext = IPlatformApplication.Current.Services.GetService<SledzSpecke.App.ViewModels.Dashboard.DashboardViewModel>();

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
    }
}
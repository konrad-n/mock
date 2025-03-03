namespace SledzSpecke.App
{
    public partial class SplashPage : ContentPage
    {
        public SplashPage()
        {
            this.InitializeComponent();
        }

        protected override async void OnAppearing()
        {
            base.OnAppearing();

            // Simulate initialization process
            await Task.Delay(2000);

            try
            {
                Application.Current.MainPage = new AppShell();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error navigating from splash screen: {ex.Message}");
                await this.DisplayAlert("Error", "Failed to initialize application. Please restart.", "OK");
            }
        }
    }
}
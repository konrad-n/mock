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
                // Instead of setting MainPage, set the Window's Page property
                var windows = Application.Current?.Windows;
                if (windows != null && windows.Count > 0)
                {
                    var window = windows[0];
                    window.Page = new AppShell();
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error navigating from splash screen: {ex.Message}");
                await this.DisplayAlert("Error", "Failed to initialize application. Please restart.", "OK");
            }
        }
    }
}
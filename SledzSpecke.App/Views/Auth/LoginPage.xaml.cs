namespace SledzSpecke.App.Views.Auth
{
    public partial class LoginPage : ContentPage
    {
        public LoginPage()
        {
            InitializeComponent();
#if DEBUG
            EmailEntry.Text = "olo@pozakontrololo.com";
            PasswordEntry.Text = "gucio";
#endif
        }

        private async void OnLoginClicked(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(EmailEntry.Text) || string.IsNullOrWhiteSpace(PasswordEntry.Text))
            {
                await DisplayAlert("Błąd", "Proszę wprowadzić adres email i hasło.", "OK");
                return;
            }

            LoginButton.IsEnabled = false;
            ActivityIndicator.IsRunning = true;

            try
            {
                bool result = await App.AuthenticationService.LoginAsync(EmailEntry.Text, PasswordEntry.Text);

                if (result)
                {
                    // Navigate to main page
                    Application.Current.MainPage = new AppShell();
                }
                else
                {
                    await DisplayAlert("Błąd logowania", "Nieprawidłowy adres email lub hasło.", "OK");
                }
            }
            catch (Exception ex)
            {
                await DisplayAlert("Błąd", $"Wystąpił problem podczas logowania: {ex.Message}", "OK");
            }
            finally
            {
                LoginButton.IsEnabled = true;
                ActivityIndicator.IsRunning = false;
            }
        }

        private async void OnRegisterClicked(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new RegistrationPage());
        }
    }
}

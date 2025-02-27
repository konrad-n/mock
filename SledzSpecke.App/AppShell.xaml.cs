using SledzSpecke.App.Views;
using SledzSpecke.App.Views.Auth;

namespace SledzSpecke.App
{
    public partial class AppShell : Shell
    {
        public AppShell()
        {
            InitializeComponent();

            // Register routes for navigation
            Routing.RegisterRoute(nameof(CourseDetailsPage), typeof(CourseDetailsPage));
            Routing.RegisterRoute(nameof(InternshipDetailsPage), typeof(InternshipDetailsPage));
            Routing.RegisterRoute(nameof(ProcedureDetailsPage), typeof(ProcedureDetailsPage));
            Routing.RegisterRoute(nameof(ProcedureEntryPage), typeof(ProcedureEntryPage));
            Routing.RegisterRoute(nameof(DutyShiftDetailsPage), typeof(DutyShiftDetailsPage));
            Routing.RegisterRoute(nameof(SelfEducationDetailsPage), typeof(SelfEducationDetailsPage));
            Routing.RegisterRoute(nameof(SelfEducationPage), typeof(SelfEducationPage));
            Routing.RegisterRoute(nameof(SMKExportPage), typeof(SMKExportPage));
            Routing.RegisterRoute(nameof(SettingsPage), typeof(SettingsPage));

            // Set user information in flyout header
            try
            {
                UpdateUserInfo();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error updating user info: {ex.Message}");
                // Ustawienie wartości domyślnych w przypadku błędu
                UserNameLabel.Text = "Użytkownik";
                SpecializationLabel.Text = "Specjalizacja";
            }
        }

        private void UpdateUserInfo()
        {
            // Get user info from Authentication service
            if (App.AuthenticationService.IsAuthenticated)
            {
                UserNameLabel.Text = App.AuthenticationService.CurrentUser.Username;

                try
                {
                    // Pobierz nazwę specjalizacji asynchronicznie
                    Task.Run(async () =>
                    {
                        try
                        {
                            var specialization = await App.SpecializationService.GetSpecializationAsync();

                            // Upewnij się, że aktualizacja UI odbywa się na głównym wątku
                            MainThread.BeginInvokeOnMainThread(() =>
                            {
                                SpecializationLabel.Text = specialization?.Name ?? "Brak specjalizacji";
                            });
                        }
                        catch (Exception ex)
                        {
                            System.Diagnostics.Debug.WriteLine($"Error getting specialization: {ex.Message}");

                            MainThread.BeginInvokeOnMainThread(() =>
                            {
                                SpecializationLabel.Text = "Brak specjalizacji";
                            });
                        }
                    });
                }
                catch (Exception ex)
                {
                    System.Diagnostics.Debug.WriteLine($"Error in task: {ex.Message}");
                    SpecializationLabel.Text = "Brak specjalizacji";
                }
            }
        }

        private async void OnLogoutClicked(object sender, EventArgs e)
        {
            bool confirm = await DisplayAlert("Wylogowanie", "Czy na pewno chcesz się wylogować?", "Tak", "Nie");
            if (confirm)
            {
                App.AuthenticationService.Logout();
                Application.Current.MainPage = new NavigationPage(new LoginPage());
            }
        }
    }
}
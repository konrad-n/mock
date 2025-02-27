using SledzSpecke.App.Views;
using SledzSpecke.App.Views.Auth;

namespace SledzSpecke;

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
        UpdateUserInfo();
    }

    private void UpdateUserInfo()
    {
        // Get user info from Authentication service
        if (App.AuthenticationService.IsAuthenticated)
        {
            UserNameLabel.Text = App.AuthenticationService.CurrentUser.Username;

            // Get specialization name
            var specializationTask = App.SpecializationService.GetSpecializationAsync();
            specializationTask.Wait(); // This is not ideal in production, but simple for now
            SpecializationLabel.Text = specializationTask.Result?.Name ?? "Brak specjalizacji";
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
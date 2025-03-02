using SledzSpecke.App.Features.Authentication.Views;
using SledzSpecke.App.Features.Courses.Views;
using SledzSpecke.App.Features.Duties.Views;
using SledzSpecke.App.Features.Internships.Views;
using SledzSpecke.App.Features.Procedures.Views;
using SledzSpecke.App.Features.SelfEducations.Views;
using SledzSpecke.App.Features.Settings.Views;
using SledzSpecke.App.Features.SMKExport.Views;
using SledzSpecke.App.Services.Interfaces;

namespace SledzSpecke.App
{
    public partial class AppShell : Shell
    {
        private readonly IAuthenticationService authenticationService;
        private readonly ISpecializationService specializationService;
        private readonly IServiceProvider serviceProvider;

        public AppShell(
            IAuthenticationService authenticationService,
            ISpecializationService specializationService,
            IServiceProvider serviceProvider)
        {
            this.InitializeComponent();

            this.authenticationService = authenticationService;
            this.specializationService = specializationService;
            this.serviceProvider = serviceProvider;

            Routing.RegisterRoute(nameof(CourseDetailsPage), typeof(CourseDetailsPage));
            Routing.RegisterRoute(nameof(InternshipDetailsPage), typeof(InternshipDetailsPage));
            Routing.RegisterRoute(nameof(ProcedureDetailsPage), typeof(ProcedureDetailsPage));
            Routing.RegisterRoute(nameof(ProcedureEntryPage), typeof(ProcedureEntryPage));
            Routing.RegisterRoute(nameof(DutyShiftDetailsPage), typeof(DutyShiftDetailsPage));
            Routing.RegisterRoute(nameof(SelfEducationDetailsPage), typeof(SelfEducationDetailsPage));
            Routing.RegisterRoute(nameof(SelfEducationPage), typeof(SelfEducationPage));
            Routing.RegisterRoute(nameof(SMKExportPage), typeof(SMKExportPage));
            Routing.RegisterRoute(nameof(SettingsPage), typeof(SettingsPage));
            Routing.RegisterRoute(nameof(RegistrationPage), typeof(RegistrationPage));

            try
            {
                this.UpdateUserInfo();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error updating user info: {ex.Message}");
                this.UserNameLabel.Text = "Użytkownik";
                this.SpecializationLabel.Text = "Specjalizacja";
            }
        }

        private void UpdateUserInfo()
        {
            if (this.authenticationService.IsAuthenticated)
            {
                this.UserNameLabel.Text = this.authenticationService.CurrentUser.Username;

                try
                {
                    Task.Run(async () =>
                    {
                        try
                        {
                            var specialization = await this.specializationService.GetSpecializationAsync();
                            MainThread.BeginInvokeOnMainThread(() =>
                            {
                                this.SpecializationLabel.Text = specialization?.Name ?? "Brak specjalizacji";
                            });
                        }
                        catch (Exception ex)
                        {
                            System.Diagnostics.Debug.WriteLine($"Error getting specialization: {ex.Message}");
                            MainThread.BeginInvokeOnMainThread(() =>
                            {
                                this.SpecializationLabel.Text = "Brak specjalizacji";
                            });
                        }
                    });
                }
                catch (Exception ex)
                {
                    System.Diagnostics.Debug.WriteLine($"Error in task: {ex.Message}");
                    this.SpecializationLabel.Text = "Brak specjalizacji";
                }
            }
        }

        private async void OnLogoutClicked(object sender, EventArgs e)
        {
            bool confirm = await this.DisplayAlert("Wylogowanie", "Czy na pewno chcesz się wylogować?", "Tak", "Nie");
            if (confirm)
            {
                this.authenticationService.Logout();

                if (Application.Current?.Windows.Count > 0)
                {
                    Application.Current.Windows[0].Page = new NavigationPage(this.serviceProvider.GetRequiredService<LoginPage>());
                }
            }
        }
    }
}
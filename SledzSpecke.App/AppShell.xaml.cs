using SledzSpecke.App.Services.Authentication;
using SledzSpecke.App.Views.Export;
using SledzSpecke.App.Views.Internships;
using SledzSpecke.App.Views.MedicalShifts;
using SledzSpecke.App.Views.Procedures;

namespace SledzSpecke.App
{
    public partial class AppShell : Shell
    {
        private readonly IAuthService authService;

        public AppShell(IAuthService authService)
        {
            this.authService = authService;
            this.InitializeComponent();
            this.RegisterRoutes();
            this.InitializeUserInfoAsync();
        }

        private void RegisterRoutes()
        {
            Routing.RegisterRoute("export", typeof(ExportPage));
            Routing.RegisterRoute("exportpreview", typeof(ExportPreviewPage));
            Routing.RegisterRoute("MedicalShiftsSelector", typeof(MedicalShiftsSelectorPage));
            Routing.RegisterRoute("OldSMKMedicalShifts", typeof(OldSMKMedicalShiftsPage));
            Routing.RegisterRoute("NewSMKMedicalShifts", typeof(NewSMKMedicalShiftsPage));
            Routing.RegisterRoute("AddEditOldSMKMedicalShift", typeof(AddEditOldSMKMedicalShiftPage));
            Routing.RegisterRoute("medicalshifts/AddEditOldSMKMedicalShift", typeof(AddEditOldSMKMedicalShiftPage));
            Routing.RegisterRoute("ProcedureSelector", typeof(ProcedureSelectorPage));
            Routing.RegisterRoute("OldSMKProcedures", typeof(OldSMKProceduresListPage));
            Routing.RegisterRoute("NewSMKProcedures", typeof(NewSMKProceduresListPage));
            Routing.RegisterRoute("AddEditOldSMKProcedure", typeof(AddEditOldSMKProcedurePage));
            Routing.RegisterRoute("AddEditNewSMKProcedure", typeof(AddEditNewSMKProcedurePage));
            Routing.RegisterRoute("internships", typeof(InternshipsSelectorPage));
            Routing.RegisterRoute("NewSMKInternships", typeof(NewSMKInternshipsListPage));
            Routing.RegisterRoute("AddEditInternship", typeof(AddEditInternshipPage));
        }

        private async void InitializeUserInfoAsync()
        {
            var user = await this.authService.GetCurrentUserAsync();
            if (user != null)
            {
                if (this.UserNameLabel != null)
                {
                    this.UserNameLabel.Text = user.Username;
                }

                var specializationService = IPlatformApplication.Current.Services.GetService<SledzSpecke.App.Services.Specialization.ISpecializationService>();
                if (specializationService != null)
                {
                    var specialization = await specializationService.GetCurrentSpecializationAsync();
                    if (specialization != null && this.SpecializationLabel != null)
                    {
                        this.SpecializationLabel.Text = specialization.Name;
                    }
                }
            }
        }

        public async Task LogoutAsync()
        {
            try
            {
                bool confirm = await this.DisplayAlert(
                    "Wylogowanie",
                    "Czy na pewno chcesz się wylogować?",
                    "Tak",
                    "Nie");

                if (confirm)
                {
                    await this.authService.LogoutAsync();

                    var loginViewModel = IPlatformApplication.Current.Services.GetService<SledzSpecke.App.ViewModels.Authentication.LoginViewModel>();
                    var loginPage = new SledzSpecke.App.Views.Authentication.LoginPage(loginViewModel);

                    Application.Current.MainPage = new NavigationPage(loginPage);
                }
            }
            catch (Exception ex)
            {
                await this.DisplayAlert("Błąd", "Wystąpił błąd podczas wylogowywania.", "OK");
            }
        }

        private async void OnLogoutClicked(object sender, EventArgs e)
        {
            await this.LogoutAsync();
        }
    }
}
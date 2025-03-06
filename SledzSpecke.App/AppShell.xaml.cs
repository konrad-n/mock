using SledzSpecke.App.Services.Authentication;

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
            // Rejestrujemy tylko ścieżki dla stron szczegółowych i edycji
            Routing.RegisterRoute("MedicalShiftDetails", typeof(Views.MedicalShifts.MedicalShiftDetailsPage));
            Routing.RegisterRoute("AddEditMedicalShift", typeof(Views.MedicalShifts.AddEditMedicalShiftPage));
            Routing.RegisterRoute("ProcedureDetails", typeof(Views.Procedures.ProcedureDetailsPage));
            Routing.RegisterRoute("AddEditProcedure", typeof(Views.Procedures.AddEditProcedurePage));
            Routing.RegisterRoute("InternshipDetails", typeof(Views.Internships.InternshipDetailsPage));
            Routing.RegisterRoute("AddEditInternship", typeof(Views.Internships.AddEditInternshipPage));
            Routing.RegisterRoute("CourseDetails", typeof(Views.Courses.CourseDetailsPage));
            Routing.RegisterRoute("AddEditCourse", typeof(Views.Courses.AddEditCoursePage));
            Routing.RegisterRoute("SelfEducationDetails", typeof(Views.SelfEducation.SelfEducationDetailsPage));
            Routing.RegisterRoute("AddEditSelfEducation", typeof(Views.SelfEducation.AddEditSelfEducationPage));
            Routing.RegisterRoute("PublicationDetails", typeof(Views.Publications.PublicationDetailsPage));
            Routing.RegisterRoute("AddEditPublication", typeof(Views.Publications.AddEditPublicationPage));
            Routing.RegisterRoute("AbsenceDetails", typeof(Views.Absences.AbsenceDetailsPage));
            Routing.RegisterRoute("AddEditAbsence", typeof(Views.Absences.AddEditAbsencePage));
            Routing.RegisterRoute("RecognitionDetails", typeof(Views.Recognitions.RecognitionDetailsPage));
            Routing.RegisterRoute("AddEditRecognition", typeof(Views.Recognitions.AddEditRecognitionPage));
            Routing.RegisterRoute("ExportPreview", typeof(Views.Export.ExportPreviewPage));
        }

        private async void InitializeUserInfoAsync()
        {
            try
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
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Błąd podczas inicjalizacji informacji o użytkowniku: {ex.Message}");
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
                System.Diagnostics.Debug.WriteLine($"Błąd podczas wylogowywania: {ex.Message}");
                await this.DisplayAlert("Błąd", "Wystąpił błąd podczas wylogowywania.", "OK");
            }
        }

        private async void OnLogoutClicked(object sender, EventArgs e)
        {
            await this.LogoutAsync();
        }
    }
}
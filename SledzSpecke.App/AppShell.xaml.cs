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
            // Rejestracja tras do stron dyżurów
            Routing.RegisterRoute("MedicalShiftDetails", typeof(Views.MedicalShifts.MedicalShiftDetailsPage));
            Routing.RegisterRoute("AddEditMedicalShift", typeof(Views.MedicalShifts.AddEditMedicalShiftPage));

            // Rejestracja tras do stron procedur
            Routing.RegisterRoute("ProcedureDetails", typeof(Views.Procedures.ProcedureDetailsPage));
            Routing.RegisterRoute("AddEditProcedure", typeof(Views.Procedures.AddEditProcedurePage));

            // Rejestracja tras do stron staży
            Routing.RegisterRoute("InternshipDetails", typeof(Views.Internships.InternshipDetailsPage));
            Routing.RegisterRoute("AddEditInternship", typeof(Views.Internships.AddEditInternshipPage));

            // Rejestracja tras do stron kursów
            Routing.RegisterRoute("CourseDetails", typeof(Views.Courses.CourseDetailsPage));
            Routing.RegisterRoute("AddEditCourse", typeof(Views.Courses.AddEditCoursePage));

            // Rejestracja tras do stron samokształcenia
            Routing.RegisterRoute("SelfEducationDetails", typeof(Views.SelfEducation.SelfEducationDetailsPage));
            Routing.RegisterRoute("AddEditSelfEducation", typeof(Views.SelfEducation.AddEditSelfEducationPage));

            // Rejestracja tras do stron publikacji
            Routing.RegisterRoute("PublicationDetails", typeof(Views.Publications.PublicationDetailsPage));
            Routing.RegisterRoute("AddEditPublication", typeof(Views.Publications.AddEditPublicationPage));

            // Rejestracja tras do stron absencji
            Routing.RegisterRoute("AbsenceDetails", typeof(Views.Absences.AbsenceDetailsPage));
            Routing.RegisterRoute("AddEditAbsence", typeof(Views.Absences.AddEditAbsencePage));

            // Rejestracja tras do stron uznań
            Routing.RegisterRoute("RecognitionDetails", typeof(Views.Recognitions.RecognitionDetailsPage));
            Routing.RegisterRoute("AddEditRecognition", typeof(Views.Recognitions.AddEditRecognitionPage));
        }

        private async void InitializeUserInfoAsync()
        {
            try
            {
                var user = await this.authService.GetCurrentUserAsync();
                if (user != null)
                {
                    // Aktualizacja informacji o użytkowniku w ShellHeader
                    if (this.UserNameLabel != null)
                    {
                        this.UserNameLabel.Text = user.Username;
                    }

                    // Pobierz informacje o specjalizacji dla użytkownika
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

                    // Przejście do ekranu logowania
                    var loginViewModel = IPlatformApplication.Current.Services.GetService<SledzSpecke.App.ViewModels.Authentication.LoginViewModel>();
                    var loginPage = new SledzSpecke.App.Views.Authentication.LoginPage(loginViewModel);

                    // Zamieniamy główną stronę aplikacji
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
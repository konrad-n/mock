namespace SledzSpecke.App
{
    public partial class AppShell : Shell
    {
        public AppShell()
        {
            this.InitializeComponent();
            this.RegisterRoutes();
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
    }
}
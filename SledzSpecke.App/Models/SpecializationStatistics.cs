namespace SledzSpecke.App.Models
{
    public class SpecializationStatistics
    {
        // Staże
        public int CompletedInternships { get; set; }

        public int RequiredInternships { get; set; }

        public int CompletedInternshipDays { get; set; }

        public int RequiredInternshipDays { get; set; }

        // Kursy
        public int CompletedCourses { get; set; }

        public int RequiredCourses { get; set; }

        // Dyżury
        public int CompletedShiftHours { get; set; }

        public int RequiredShiftHours { get; set; }

        // Procedury
        public int CompletedProceduresA { get; set; }

        public int RequiredProceduresA { get; set; }

        public int CompletedProceduresB { get; set; }

        public int RequiredProceduresB { get; set; }

        // Samokształcenie
        public int SelfEducationDaysUsed { get; set; }

        public int SelfEducationDaysTotal { get; set; }

        // Aktywność edukacyjna
        public int EducationalActivitiesCompleted { get; set; }

        // Publikacje
        public int PublicationsCompleted { get; set; }

        // Absencje
        public int AbsenceDays { get; set; }

        public int AbsenceDaysExtendingSpecialization { get; set; }

        // Obliczanie procentowego ukończenia specjalizacji
        public double GetOverallProgress()
        {
            // Wagi dla różnych kategorii
            const double internshipWeight = 0.35;
            const double courseWeight = 0.25;
            const double procedureWeight = 0.3;
            const double otherWeight = 0.1;

            // Obliczanie procentu ukończenia dla każdej kategorii
            double internshipProgress = this.RequiredInternships > 0
                ? (double)this.CompletedInternships / this.RequiredInternships
                : 0;

            double courseProgress = this.RequiredCourses > 0
                ? (double)this.CompletedCourses / this.RequiredCourses
                : 0;

            double procedureProgress = (this.RequiredProceduresA + this.RequiredProceduresB) > 0
                ? (double)(this.CompletedProceduresA + this.CompletedProceduresB) / (this.RequiredProceduresA + this.RequiredProceduresB)
                : 0;

            // Średni procent ukończenia ważony
            double overallProgress = (internshipProgress * internshipWeight) +
                                     (courseProgress * courseWeight) +
                                     (procedureProgress * procedureWeight) +
                                     otherWeight; // Pozostałe aktywności

            return Math.Min(1.0, overallProgress);
        }
    }
}

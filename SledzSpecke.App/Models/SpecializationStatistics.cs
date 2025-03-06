using System;

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

        // Procedury (teraz wyraźnie podzielone na typy A i B zgodnie z JSONem)
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

        /// <summary>
        /// Obliczanie procentowego ukończenia specjalizacji na podstawie ważonych kategorii.
        /// </summary>
        /// <returns>Wartość postępu (0.0 - 1.0).</returns>
        public double GetOverallProgress()
        {
            // Wagi dla różnych kategorii zgodnie z JSONem
            const double internshipWeight = 0.35;
            const double courseWeight = 0.25;
            const double procedureWeight = 0.30;
            const double otherWeight = 0.10;

            // Obliczanie procentu ukończenia dla każdej kategorii
            double internshipProgress = this.RequiredInternships > 0
                ? (double)this.CompletedInternships / this.RequiredInternships
                : 0;

            double courseProgress = this.RequiredCourses > 0
                ? (double)this.CompletedCourses / this.RequiredCourses
                : 0;

            // Dla procedur, uwzględniamy zarówno typ A jak i B
            double procedureProgress;

            if (this.RequiredProceduresA + this.RequiredProceduresB > 0)
            {
                double procedureAProgress = this.RequiredProceduresA > 0
                    ? (double)this.CompletedProceduresA / this.RequiredProceduresA
                    : 0;

                double procedureBProgress = this.RequiredProceduresB > 0
                    ? (double)this.CompletedProceduresB / this.RequiredProceduresB
                    : 0;

                // Połączony postęp procedur ważony liczbą wymaganych procedur
                procedureProgress =
                    (procedureAProgress * this.RequiredProceduresA +
                     procedureBProgress * this.RequiredProceduresB) /
                    (this.RequiredProceduresA + this.RequiredProceduresB);
            }
            else
            {
                procedureProgress = 0;
            }

            // Ważony ogólny postęp
            double overallProgress =
                (internshipProgress * internshipWeight) +
                (courseProgress * courseWeight) +
                (procedureProgress * procedureWeight) +
                otherWeight; // Wkład innych aktywności

            // Upewniamy się, że nie przekraczamy 100%
            return Math.Min(1.0, overallProgress);
        }
    }
}
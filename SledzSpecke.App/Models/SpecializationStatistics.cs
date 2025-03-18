using System;

namespace SledzSpecke.App.Models
{
    // ZAKAZ MODYFIKACJI!!!! JEST TO MODEL 1 DO 1 Z JSON!!!
    public class SpecializationStatistics
    {
        public int CompletedInternships { get; set; }

        public int RequiredInternships { get; set; }

        public int CompletedInternshipDays { get; set; }

        public int RequiredInternshipWorkingDays { get; set; }
        public int CompletedCourses { get; set; }

        public int RequiredCourses { get; set; }

        public int CompletedShiftHours { get; set; }

        public int RequiredShiftHours { get; set; }

        public int CompletedProceduresA { get; set; }

        public int RequiredProceduresA { get; set; }

        public int CompletedProceduresB { get; set; }

        public int RequiredProceduresB { get; set; }

        public int SelfEducationDaysUsed { get; set; }

        public int SelfEducationDaysTotal { get; set; }

        public int EducationalActivitiesCompleted { get; set; }

        public int PublicationsCompleted { get; set; }

        public int AbsenceDays { get; set; }

        public int AbsenceDaysExtendingSpecialization { get; set; }

        public double GetOverallProgress()
        {
            const double internshipWeight = 0.35;
            const double courseWeight = 0.25;
            const double procedureWeight = 0.30;
            const double otherWeight = 0.10;

            double internshipProgress = this.RequiredInternships > 0
                ? (double)this.CompletedInternships / this.RequiredInternships
                : 0;

            double courseProgress = this.RequiredCourses > 0
                ? (double)this.CompletedCourses / this.RequiredCourses
                : 0;

            double procedureProgress;

            if (this.RequiredProceduresA + this.RequiredProceduresB > 0)
            {
                double procedureAProgress = this.RequiredProceduresA > 0
                    ? (double)this.CompletedProceduresA / this.RequiredProceduresA
                    : 0;

                double procedureBProgress = this.RequiredProceduresB > 0
                    ? (double)this.CompletedProceduresB / this.RequiredProceduresB
                    : 0;

                procedureProgress =
                    (procedureAProgress * this.RequiredProceduresA +
                     procedureBProgress * this.RequiredProceduresB) /
                    (this.RequiredProceduresA + this.RequiredProceduresB);
            }
            else
            {
                procedureProgress = 0;
            }

            double otherActivitiesProgress = 0;
            int totalOtherItems = this.PublicationsCompleted + this.EducationalActivitiesCompleted;

            if (this.SelfEducationDaysTotal > 0)
            {
                double selfEducationProgress = Math.Min(1.0, (double)this.SelfEducationDaysUsed / this.SelfEducationDaysTotal);

                if (totalOtherItems == 0)
                {
                    otherActivitiesProgress = selfEducationProgress;
                }
                else
                {
                    double otherProgress = Math.Min(1.0, totalOtherItems / 10.0);
                    otherActivitiesProgress = (selfEducationProgress + otherProgress) / 2.0;
                }
            }
            else if (totalOtherItems > 0)
            {
                otherActivitiesProgress = Math.Min(1.0, totalOtherItems / 10.0);
            }

            double overallProgress =
                (internshipProgress * internshipWeight) +
                (courseProgress * courseWeight) +
                (procedureProgress * procedureWeight) +
                (otherActivitiesProgress * otherWeight);

            return Math.Min(1.0, overallProgress);
        }
    }
}
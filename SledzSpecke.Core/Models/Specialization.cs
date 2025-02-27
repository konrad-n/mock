using SledzSpecke.Core.Models.Enums;
using SQLite;
using System;
using System.Collections.Generic;
using System.Linq;

namespace SledzSpecke.Core.Models
{
    [Table("Specializations")]
    public class Specialization
    {
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }

        [MaxLength(100), Indexed]
        public string Name { get; set; }

        public DateTime StartDate { get; set; }

        public DateTime ExpectedEndDate { get; set; }

        public int BaseDurationWeeks { get; set; }

        public int BasicModuleDurationWeeks { get; set; }

        public int SpecialisticModuleDurationWeeks { get; set; }

        public int VacationDaysPerYear { get; set; }

        public int SelfEducationDaysPerYear { get; set; }

        public int StatutoryHolidaysPerYear { get; set; }

        public double RequiredDutyHoursPerWeek { get; set; }

        public bool RequiresPublication { get; set; }

        public int RequiredConferences { get; set; }

        public int SpecializationTypeId { get; set; }

        [Ignore]
        public List<Course> RequiredCourses { get; set; } = new List<Course>();

        [Ignore]
        public List<Internship> RequiredInternships { get; set; } = new List<Internship>();

        [Ignore]
        public List<MedicalProcedure> RequiredProcedures { get; set; } = new List<MedicalProcedure>();

        public double GetCompletionPercentage()
        {
            double coursesPercentage = RequiredCourses.Count > 0
                ? RequiredCourses.Count(c => c.IsCompleted) * 100.0 / RequiredCourses.Count
                : 0;

            double internshipsPercentage = RequiredInternships.Count > 0
                ? RequiredInternships.Count(i => i.IsCompleted) * 100.0 / RequiredInternships.Count
                : 0;

            double proceduresTypeAPercentage = RequiredProcedures.Where(p => p.ProcedureType == ProcedureType.TypeA).Sum(p => p.RequiredCount) > 0
                ? RequiredProcedures.Where(p => p.ProcedureType == ProcedureType.TypeA).Sum(p => p.CompletedCount) * 100.0 /
                  RequiredProcedures.Where(p => p.ProcedureType == ProcedureType.TypeA).Sum(p => p.RequiredCount)
                : 0;

            double proceduresTypeBPercentage = RequiredProcedures.Where(p => p.ProcedureType == ProcedureType.TypeB).Sum(p => p.RequiredCount) > 0
                ? RequiredProcedures.Where(p => p.ProcedureType == ProcedureType.TypeB).Sum(p => p.CompletedCount) * 100.0 /
                  RequiredProcedures.Where(p => p.ProcedureType == ProcedureType.TypeB).Sum(p => p.RequiredCount)
                : 0;

            return (coursesPercentage + internshipsPercentage + proceduresTypeAPercentage + proceduresTypeBPercentage) / 4;
        }

        public List<DateTime> GetUpcomingDates()
        {
            var dates = new List<DateTime>();

            // Add course dates
            foreach (var course in RequiredCourses.Where(c => !c.IsCompleted && c.ScheduledDate.HasValue))
                dates.Add(course.ScheduledDate.Value);

            // Add internship dates
            foreach (var internship in RequiredInternships.Where(i => !i.IsCompleted && i.StartDate.HasValue))
                dates.Add(internship.StartDate.Value);

            // Add end of basic module
            var endOfBasicModule = StartDate.AddDays(BasicModuleDurationWeeks * 7);
            if (endOfBasicModule > DateTime.Now)
                dates.Add(endOfBasicModule);

            return dates.OrderBy(d => d).ToList();
        }
    }
}
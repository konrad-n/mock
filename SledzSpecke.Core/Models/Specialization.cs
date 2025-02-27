using SledzSpecke.Core.Models.Domain;
using SledzSpecke.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace SledzSpecke.Core.Models
{
    public class Specialization
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime ExpectedEndDate { get; set; }
        public int BaseDurationWeeks { get; set; }
        public int BasicModuleDurationWeeks { get; set; }
        public int SpecialisticModuleDurationWeeks { get; set; }
        public int VacationDaysPerYear { get; set; }
        public int SelfEducationDaysPerYear { get; set; }
        public int StatutoryHolidaysPerYear { get; set; }
        public List<Course> RequiredCourses { get; set; } = new List<Course>();
        public List<Internship> RequiredInternships { get; set; } = new List<Internship>();
        public List<MedicalProcedure> RequiredProcedures { get; set; } = new List<MedicalProcedure>();
        public int RequiredDutyHoursPerWeek { get; set; }
        public bool RequiresPublication { get; set; }
        public int RequiredConferences { get; set; }


        public double GetCompletionPercentage()
        {
            double coursesPercentage = RequiredCourses.Count > 0
                ? RequiredCourses.Count(c => c.IsCompleted) * 100.0 / RequiredCourses.Count
                : 0;

            double internshipsPercentage = RequiredInternships.Count > 0
                ? RequiredInternships.Count(i => i.IsCompleted) * 100.0 / RequiredInternships.Count
                : 0;

            double proceduresTypeAPercentage = RequiredProcedures.Where(p => p.ProcedureType == ProcedureType.TypeA).Sum(p => p.CompletedCount) * 100.0 /
                                               RequiredProcedures.Where(p => p.ProcedureType == ProcedureType.TypeA).Sum(p => p.RequiredCount);

            double proceduresTypeBPercentage = RequiredProcedures.Where(p => p.ProcedureType == ProcedureType.TypeB).Sum(p => p.CompletedCount) * 100.0 /
                                               RequiredProcedures.Where(p => p.ProcedureType == ProcedureType.TypeB).Sum(p => p.RequiredCount);

            return (coursesPercentage + internshipsPercentage + proceduresTypeAPercentage + proceduresTypeBPercentage) / 4;
        }
    }
}

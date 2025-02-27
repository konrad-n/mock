using System;
using System.Collections.Generic;

namespace SledzSpecke.Core.Models
{
    public class Internship
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public int DurationWeeks { get; set; }
        public int WorkingDays { get; set; }
        public bool IsRequired { get; set; }
        public ModuleType Module { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public bool IsCompleted { get; set; }
        public bool HasPassedExam { get; set; }
        public string Location { get; set; }
        public string SupervisorName { get; set; }
        public List<MedicalProcedure> RequiredProcedures { get; set; } = new List<MedicalProcedure>();
        public string Notes { get; set; }
    }
}
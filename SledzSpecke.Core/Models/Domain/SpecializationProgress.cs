using System;

namespace SledzSpecke.Core.Models.Domain
{
    public class SpecializationProgress : BaseEntity
    {
        public int UserId { get; set; }
        public int SpecializationId { get; set; }
        public double ProceduresProgress { get; set; }
        public double CoursesProgress { get; set; }
        public double InternshipsProgress { get; set; }
        public double DutiesProgress { get; set; }
        public double OverallProgress { get; set; }
        public double TotalProgress { get; set; } // Added missing property
        public string RemainingRequirements { get; set; }
        public DateTime LastCalculated { get; set; }

        // Navigation properties
        public User User { get; set; }
        public Specialization Specialization { get; set; }
    }
}

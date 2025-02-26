using SQLite;
using System;

namespace SledzSpecke.Core.Models.Domain
{
    [Table("SpecializationProgress")]
    public class SpecializationProgress : BaseEntity
    {
        [PrimaryKey, AutoIncrement]
        public override int Id { get; set; }

        [Column("UserId")]
        public int UserId { get; set; }

        [Column("SpecializationId")]
        public int SpecializationId { get; set; }

        [Column("ProceduresProgress")]
        public double ProceduresProgress { get; set; }

        [Column("CoursesProgress")]
        public double CoursesProgress { get; set; }

        [Column("InternshipsProgress")]
        public double InternshipsProgress { get; set; }

        [Column("DutiesProgress")]
        public double DutiesProgress { get; set; }

        [Column("OverallProgress")]
        public double OverallProgress { get; set; }

        [Column("TotalProgress")]
        public double TotalProgress { get; set; } // Added missing property

        [Column("RemainingRequirements")]
        public string RemainingRequirements { get; set; }

        [Column("LastCalculated")]
        public DateTime LastCalculated { get; set; }

        // Navigation properties
        [Ignore]
        public User User { get; set; }

        [Ignore]
        public Specialization Specialization { get; set; }
    }
}

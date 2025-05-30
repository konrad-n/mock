﻿using SledzSpecke.App.Models.Enums;
using SQLite;

namespace SledzSpecke.App.Models
{
    public class Specialization
    {
        [PrimaryKey]
        [AutoIncrement]
        public int SpecializationId { get; set; }

        [MaxLength(100)]
        public string Name { get; set; }

        [MaxLength(20)]
        public string ProgramCode { get; set; }

        public SmkVersion SmkVersion { get; set; }

        public DateTime StartDate { get; set; }

        public DateTime PlannedEndDate { get; set; }

        public DateTime CalculatedEndDate { get; set; }

        public string ProgramStructure { get; set; }

        public int? CurrentModuleId { get; set; }

        [Ignore]
        public List<Module> Modules { get; set; } = new List<Module>();

        public int CompletedInternships { get; set; }

        public int TotalInternships { get; set; }

        public int CompletedCourses { get; set; }

        public int TotalCourses { get; set; }

        public int DurationYears { get; set; }
    }
}

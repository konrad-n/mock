using SQLite;

namespace SledzSpecke.Core.Models
{
    [Table("SpecializationTypes")]
    public class SpecializationType
    {
        [PrimaryKey]
        [AutoIncrement]
        public int Id { get; set; }

        [MaxLength(100)]
        [Indexed]
        public string Name { get; set; }

        public string Description { get; set; }

        public int BaseDurationWeeks { get; set; }

        public int BasicModuleDurationWeeks { get; set; }

        public int SpecialisticModuleDurationWeeks { get; set; }

        public int VacationDaysPerYear { get; set; }

        public int SelfEducationDaysPerYear { get; set; }

        public int StatutoryHolidaysPerYear { get; set; }

        public double RequiredDutyHoursPerWeek { get; set; }

        public bool RequiresPublication { get; set; }

        public int RequiredConferences { get; set; }
    }
}


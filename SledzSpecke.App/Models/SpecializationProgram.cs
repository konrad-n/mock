using SledzSpecke.App.Models.Enums;
using SQLite;

namespace SledzSpecke.App.Models
{
    // ZAKAZ MODYFIKACJI!!!! JEST TO MODEL 1 DO 1 Z JSON!!!

    public class SpecializationProgram
    {
        [PrimaryKey]
        [AutoIncrement]
        public int ProgramId { get; set; }

        [MaxLength(100)]
        public string Name { get; set; }

        [MaxLength(20)]
        public string Code { get; set; }

        public string Version { get; set; }

        public string Structure { get; set; }

        public SmkVersion SmkVersion { get; set; }

        public bool HasBasicModule { get; set; }

        public string BasicModuleCode { get; set; }

        public string SpecialisticModuleCode { get; set; }

        public int DurationYears { get; set; }

        public int DurationMonths { get; set; }

        public int DurationDays { get; set; }

        public int TotalWorkingDays { get; set; }

        [Ignore]
        public TotalDuration TotalDuration
        {
            get
            {
                return new TotalDuration
                {
                    Years = this.DurationYears,
                    Months = this.DurationMonths,
                    Days = this.DurationDays
                };
            }
            set
            {
                if (value != null)
                {
                    this.DurationYears = value.Years;
                    this.DurationMonths = value.Months;
                    this.DurationDays = value.Days;
                }
            }
        }
    }
}
namespace SledzSpecke.App.Models
{
    public class ExportOptions
    {
        public DateTime StartDate { get; set; }

        public DateTime EndDate { get; set; }

        public bool IncludeShifts { get; set; }

        public bool IncludeProcedures { get; set; }

        public bool IncludeInternships { get; set; }

        public bool IncludeCourses { get; set; }

        public bool IncludeSelfEducation { get; set; }

        public bool IncludePublications { get; set; }

        public bool IncludeAbsences { get; set; }

        public bool IncludeEducationalActivities { get; set; }

        public bool IncludeRecognitions { get; set; }

        // Format eksportu
        public bool FormatForOldSMK { get; set; }

        // Filtrowanie według modułu (dla specjalizacji modułowych)
        public int? ModuleId { get; set; }
    }
}
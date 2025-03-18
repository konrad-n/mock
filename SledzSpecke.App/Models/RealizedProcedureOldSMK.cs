using SledzSpecke.App.Models.Enums;
using SQLite;

namespace SledzSpecke.App.Models
{
    public class RealizedProcedureOldSMK : RealizedProcedureBase
    {
        public int Year { get; set; }

        [MaxLength(20)]
        public string Code { get; set; }

        [MaxLength(100)]
        public string PerformingPerson { get; set; }

        [MaxLength(100)]
        public string Location { get; set; }

        [Indexed]
        public int InternshipId { get; set; }

        [MaxLength(100)]
        public string InternshipName { get; set; }

        [MaxLength(10)]
        public string PatientInitials { get; set; }

        [MaxLength(1)]
        public string PatientGender { get; set; }

        public string AssistantData { get; set; }

        public string ProcedureGroup { get; set; }

        [Indexed]
        public int? ProcedureRequirementId { get; set; }
    }
}
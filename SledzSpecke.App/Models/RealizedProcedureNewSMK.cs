using SledzSpecke.App.Models.Enums;
using SQLite;

namespace SledzSpecke.App.Models
{
    public class RealizedProcedureNewSMK : RealizedProcedureBase
    {
        [Indexed]
        public int ProcedureRequirementId { get; set; }

        [Indexed]
        public int? ModuleId { get; set; }

        public int CountA { get; set; }
        public int CountB { get; set; }

        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }

        [Ignore]
        public string ProcedureName { get; set; }

        [Ignore]
        public string InternshipName { get; set; }

        [Ignore]
        public string DateRange => $"{this.StartDate:dd.MM.yyyy} - {this.EndDate:dd.MM.yyyy}";
    }
}
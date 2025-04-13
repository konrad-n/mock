using SledzSpecke.App.Models.Enums;
using SQLite;

namespace SledzSpecke.App.Models
{
    public class RealizedInternshipNewSMK : RealizedInternshipBase
    {
        [Indexed]
        public int InternshipRequirementId { get; set; }

        [Indexed]
        public int? ModuleId { get; set; }

        [Ignore]
        public string InternshipName { get; set; }
    }
}
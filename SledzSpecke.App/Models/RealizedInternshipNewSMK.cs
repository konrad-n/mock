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

        public bool IsRecognition { get; set; }

        public string RecognitionReason { get; set; }

        public int RecognitionDaysReduction { get; set; }

        public bool IsPartialRealization { get; set; }
    }
}
using System;
using SQLite;

namespace SledzSpecke.App.Models
{
    public class RealizedMedicalShiftNewSMK : RealizedMedicalShiftBase
    {
        [Indexed]
        public int InternshipRequirementId { get; set; }

        [Indexed]
        public int? ModuleId { get; set; }

        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }

        [Ignore]
        public string InternshipName { get; set; }
    }
}
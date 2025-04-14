using SledzSpecke.App.Models.Enums;
using SQLite;

namespace SledzSpecke.App.Models
{
    public class RealizedInternshipOldSMK : RealizedInternshipBase
    {
        public int Year { get; set; }

        public bool RequiresApproval { get; set; }
    }
}
using SledzSpecke.App.Models.Enums;
using SQLite;

namespace SledzSpecke.App.Models
{
    public class RealizedInternshipOldSMK : RealizedInternshipBase
    {
        public int Year { get; set; }

        [MaxLength(100)]
        public string InternshipName { get; set; }
    }
}
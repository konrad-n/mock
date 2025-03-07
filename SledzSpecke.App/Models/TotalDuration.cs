using SQLite;

namespace SledzSpecke.App.Models
{
    public class TotalDuration
    {
        public int Years { get; set; }
        public int Months { get; set; }
        public int Days { get; set; }

        [Ignore]
        public int TotalMonths => (Years * 12) + Months;
    }
}

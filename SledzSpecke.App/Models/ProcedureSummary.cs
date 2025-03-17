namespace SledzSpecke.App.Models
{
    public class ProcedureSummary
    {
        public int RequiredCountA { get; set; }
        public int RequiredCountB { get; set; }
        public int CompletedCountA { get; set; }
        public int CompletedCountB { get; set; }
        public int ApprovedCountA { get; set; }
        public int ApprovedCountB { get; set; }

        public int RemainingCountA => this.RequiredCountA - this.CompletedCountA;
        public int RemainingCountB => this.RequiredCountB - this.CompletedCountB;
    }
}
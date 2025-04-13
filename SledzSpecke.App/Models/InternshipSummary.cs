namespace SledzSpecke.App.Models
{
    public class InternshipSummary
    {
        public int RequiredDays { get; set; }
        public int CompletedDays { get; set; }
        public int RecognizedDays { get; set; }
        public int SelfEducationDays { get; set; }

        public int RemainingDays => this.RequiredDays - this.CompletedDays - this.RecognizedDays - this.SelfEducationDays;

        public bool IsCompleted => this.RemainingDays <= 0;

        public double CompletionPercentage
        {
            get
            {
                if (this.RequiredDays <= 0)
                {
                    return 0;
                }

                double completed = (double)(this.CompletedDays + this.RecognizedDays + this.SelfEducationDays) / this.RequiredDays;
                return Math.Min(1.0, completed);
            }
        }
    }
}
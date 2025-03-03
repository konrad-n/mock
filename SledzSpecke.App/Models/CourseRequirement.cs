namespace SledzSpecke.App.Models
{
    public class CourseRequirement
    {
        public string CourseName { get; set; }

        public double DurationWeeks { get; set; }

        public int DurationDays { get; set; }

        public bool IsMandatory { get; set; }
    }
}

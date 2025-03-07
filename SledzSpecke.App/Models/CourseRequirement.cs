namespace SledzSpecke.App.Models
{
    public class CourseRequirement
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public double Weeks { get; set; }
        public int WorkingDays { get; set; }
        public bool Required { get; set; }
    }
}

using SledzSpecke.App.Models.Enums;

namespace SledzSpecke.App.Models
{
    public class ModuleStructure
    {
        public string ModuleName { get; set; }

        public ModuleType ModuleType { get; set; }

        public int DurationMonths { get; set; }

        public List<InternshipRequirement> Internships { get; set; }

        public List<CourseRequirement> Courses { get; set; }

        public List<ProcedureRequirement> Procedures { get; set; }

        public int RequiredShiftHours { get; set; }

        public int SelfEducationDays { get; set; }
    }
}

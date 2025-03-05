using SledzSpecke.App.Models.Enums;

namespace SledzSpecke.App.Models
{
    public class SpecializationStructure
    {
        public SpecializationStructure()
        {
            Courses = new List<CourseRequirement>();
            Internships = new List<InternshipRequirement>();
            Procedures = new List<ProcedureRequirement>();
            Name = string.Empty;
            Code = string.Empty;
            TotalDuration = new TotalDuration();
            BasicInfo = new BasicInfo();
            SelfEducation = new SelfEducationInfo();
            Holidays = new HolidaysInfo();
            MedicalShifts = new MedicalShiftsInfo();
        }

        public string Name { get; set; }

        public string Code { get; set; }

        public SmkVersion SmkVersion { get; set; }

        public bool HasModules { get; set; }

        public TotalDuration TotalDuration { get; set; }

        public int TotalWorkingDays { get; set; }

        public BasicInfo BasicInfo { get; set; }

        public List<CourseRequirement> Courses { get; set; }

        public List<InternshipRequirement> Internships { get; set; }

        public List<ProcedureRequirement> Procedures { get; set; }

        public SelfEducationInfo SelfEducation { get; set; }

        public HolidaysInfo Holidays { get; set; }

        public MedicalShiftsInfo MedicalShifts { get; set; }
    }
}

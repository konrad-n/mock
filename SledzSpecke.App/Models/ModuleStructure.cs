using SledzSpecke.App.Models.Enums;

namespace SledzSpecke.App.Models
{
    // ZAKAZ MODYFIKACJI!!!! JEST TO MODEL 1 DO 1 Z JSON!!!

    public class ModuleStructure
    {
        public int ModuleId { get; set; }
        public string Name { get; set; }
        public string Code { get; set; }
        public ModuleType ModuleType { get; set; }
        public string Version { get; set; }
        public TotalDuration Duration { get; set; }
        public int WorkingDays { get; set; }
        public List<CourseRequirement> Courses { get; set; }
        public List<InternshipRequirement> Internships { get; set; }
        public List<ProcedureRequirement> Procedures { get; set; }
        public SelfEducationInfo SelfEducation { get; set; }
        public HolidaysInfo Holidays { get; set; }
        public MedicalShiftsInfo MedicalShifts { get; set; }
        public ProcedureCodeDescription ProcedureCodeDescription { get; set; }
        public ExaminationInfo ExaminationInfo { get; set; }

        public int RequiredShiftHours { get; set; }
        public int SelfEducationDays { get; set; }
    }
}
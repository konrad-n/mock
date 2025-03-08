using SledzSpecke.App.Models.Enums;

namespace SledzSpecke.App.Models
{
    // ZAKAZ MODYFIKACJI!!!! JEST TO MODEL 1 DO 1 Z JSON!!!

    public class SpecializationStructure
    {
        public string Name { get; set; }
        public string Code { get; set; }
        public string Version { get; set; }
        public TotalDuration TotalDuration { get; set; }
        public int TotalWorkingDays { get; set; }
        public BasicInfo BasicInfo { get; set; }
        public List<ModuleStructure> Modules { get; set; }
        public SelfEducationInfo SelfEducation { get; set; }
        public HolidaysInfo Holidays { get; set; }
        public MedicalShiftsInfo MedicalShifts { get; set; }
        public ProcedureCodeDescription ProcedureCodeDescription { get; set; }
        public ExaminationInfo ExaminationInfo { get; set; }
    }
}

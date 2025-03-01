using SledzSpecke.Core.Models;
using SledzSpecke.Core.Models.Enums;

namespace SledzSpecke.App.Services
{
    public interface ISpecializationService
    {
        Task AddProcedureEntryAsync(MedicalProcedure procedure, ProcedureEntry entry);
        Task<List<Course>> GetCoursesAsync(ModuleType moduleType);
        Task<List<Internship>> GetInternshipsAsync(ModuleType moduleType);
        Task<List<MedicalProcedure>> GetProceduresAsync(ModuleType moduleType, ProcedureType procedureType);
        Task<Specialization> GetSpecializationAsync();
        Task SaveCourseAsync(Course course);
        Task SaveInternshipAsync(Internship internship);
        Task SaveProcedureAsync(MedicalProcedure procedure);
        Task SaveSpecializationAsync(Specialization specialization);
    }
}
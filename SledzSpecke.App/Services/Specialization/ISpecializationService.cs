using SledzSpecke.App.Models;
using SledzSpecke.App.Models.Enums;

namespace SledzSpecke.App.Services.Specialization
{
    public interface ISpecializationService
    {
        Task<Models.Specialization> GetCurrentSpecializationAsync(bool includeModules = true);

        Task<Module> GetCurrentModuleAsync();

        Task SetCurrentModuleAsync(int moduleId);

        Task<SpecializationProgram> LoadSpecializationProgramAsync(string code, SmkVersion smkVersion);

        Task<List<SpecializationProgram>> GetAvailableSpecializationProgramsAsync();

        Task<List<Module>> GetModulesAsync(int specializationId);

        Task<bool> InitializeSpecializationModulesAsync(int specializationId);

        Task<List<Internship>> GetInternshipsAsync(int? moduleId = null);

        Task<Internship> GetInternshipAsync(int internshipId);

        Task<List<Internship>> GetUserInternshipsAsync(int? moduleId = null);

        Task<Internship> GetCurrentInternshipAsync();

        Task<bool> AddInternshipAsync(Internship internship);

        Task<bool> UpdateInternshipAsync(Internship internship);

        Task<bool> DeleteInternshipAsync(int internshipId);

        Task<List<MedicalShift>> GetMedicalShiftsAsync(int? internshipId = null);

        Task<MedicalShift> GetMedicalShiftAsync(int shiftId);

        Task<bool> AddMedicalShiftAsync(MedicalShift shift);

        Task<bool> UpdateMedicalShiftAsync(MedicalShift shift);

        Task<bool> DeleteMedicalShiftAsync(int shiftId);

        event EventHandler<int> CurrentModuleChanged;

        Task<SpecializationStatistics> GetSpecializationStatisticsAsync(int? moduleId = null);

        Task UpdateSpecializationProgressAsync(int specializationId);

        Task UpdateModuleProgressAsync(int moduleId);

        Task<DateTime> CalculateSpecializationEndDateAsync(int specializationId);

        Task<int> GetShiftCountAsync(int? moduleId = null);

        Task<int> GetProcedureCountAsync(int? moduleId = null);

        Task<int> GetInternshipCountAsync(int? moduleId = null);

        Task<int> GetCourseCountAsync(int? moduleId = null);

        Task<int> GetSelfEducationCountAsync(int? moduleId = null);

        Task<int> GetPublicationCountAsync(int? moduleId = null);

        Task<int> GetEducationalActivityCountAsync(int? moduleId = null);

        Task<int> GetAbsenceCountAsync();

        Task<int> GetRecognitionCountAsync();
    }
}
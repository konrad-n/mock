using SledzSpecke.Core.Models.Domain;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SledzSpecke.Core.Interfaces.Services
{
    public interface ISpecializationService
    {
        Task<Specialization> GetCurrentSpecializationAsync();
        Task<Specialization> GetSpecializationAsync(int id);
        Task<List<Specialization>> GetAllSpecializationsAsync();
        Task<SpecializationProgress> GetProgressStatisticsAsync(int specializationId);
        Task<List<ProcedureRequirement>> GetRequiredProceduresAsync(int specializationId);
        Task<List<CourseDefinition>> GetRequiredCoursesAsync(int specializationId);
        Task<List<InternshipDefinition>> GetRequiredInternshipsAsync(int specializationId);
        Task<List<DutyRequirement>> GetRequiredDutiesAsync(int specializationId);
        Task<Dictionary<string, double>> GetRequirementsProgressAsync(int specializationId);
    }
}

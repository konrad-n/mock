using SledzSpecke.App.Models;
using SledzSpecke.App.Models.Enums;

namespace SledzSpecke.App.Services.Internships
{
    public interface IInternshipService
    {
        Task<List<RealizedInternshipNewSMK>> GetRealizedInternshipsNewSMKAsync(int? moduleId = null, int? internshipRequirementId = null);
        Task<RealizedInternshipNewSMK> GetRealizedInternshipNewSMKAsync(int id);
        Task<bool> SaveRealizedInternshipNewSMKAsync(RealizedInternshipNewSMK realizedInternship);
        Task<bool> DeleteRealizedInternshipNewSMKAsync(int id);

        Task<List<RealizedInternshipOldSMK>> GetRealizedInternshipsOldSMKAsync(int year = 0, string internshipName = null);
        Task<RealizedInternshipOldSMK> GetRealizedInternshipOldSMKAsync(int id);
        Task<bool> SaveRealizedInternshipOldSMKAsync(RealizedInternshipOldSMK realizedInternship);
        Task<bool> DeleteRealizedInternshipOldSMKAsync(int id);

        Task<InternshipSummary> GetInternshipSummaryAsync(int internshipRequirementId, int? moduleId = null);
    }
}
using SledzSpecke.Core.Models.Domain;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SledzSpecke.Core.Interfaces.Services
{
    public interface IInternshipService
    {
        // Istniejące metody
        Task<List<Internship>> GetUserInternshipsAsync();
        Task<Internship> GetInternshipAsync(int id);
        Task<Internship> StartInternshipAsync(Internship internship);
        Task<bool> CompleteInternshipAsync(int internshipId, InternshipDocument completion);
        
        // Nowe metody
        Task<List<InternshipDefinition>> GetRequiredInternshipsAsync();
        Task<List<InternshipModule>> GetModulesForInternshipAsync(int internshipDefinitionId);
        Task<Dictionary<string, List<string>>> GetRequiredSkillsByInternshipAsync(int internshipDefinitionId);
        Task<Dictionary<string, Dictionary<string, int>>> GetRequiredProceduresByInternshipAsync(int internshipDefinitionId);
        Task<double> GetInternshipProgressAsync();
        Task<Dictionary<string, (int Required, int Completed)>> GetInternshipProgressByYearAsync();
        Task<List<InternshipDefinition>> GetRecommendedInternshipsForCurrentYearAsync();
    }
}

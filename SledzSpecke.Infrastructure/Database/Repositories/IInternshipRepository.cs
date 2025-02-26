using SledzSpecke.Core.Models.Domain;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SledzSpecke.Infrastructure.Database.Repositories
{
    public interface IInternshipRepository : IBaseRepository<Internship>
    {
        Task<List<Internship>> GetUserInternshipsAsync(int userId);
        Task<List<InternshipDefinition>> GetRequiredInternshipsAsync(int specializationId);
        Task<List<InternshipModule>> GetModulesForInternshipAsync(int internshipDefinitionId);
        Task<double> GetInternshipProgressAsync(int userId, int specializationId);
        Task<Dictionary<string, (int Required, int Completed)>> GetInternshipProgressByYearAsync(int userId, int specializationId);
    }
}
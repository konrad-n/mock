using SledzSpecke.Core.Models.Domain;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SledzSpecke.Core.Interfaces.Services
{
    public interface IInternshipService
    {
        Task<List<Internship>> GetUserInternshipsAsync();
        Task<List<InternshipDefinition>> GetRequiredInternshipsAsync();
        Task<List<InternshipModule>> GetModulesForInternshipAsync(int internshipDefinitionId);
        Task<double> GetInternshipProgressAsync();
        Task<Dictionary<string, (int Required, int Completed)>> GetInternshipProgressByYearAsync();
    }
}

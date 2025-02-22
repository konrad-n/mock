using SledzSpecke.Core.Models.Domain;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SledzSpecke.Core.Interfaces.Services
{
    public interface IInternshipService
    {
        Task<List<Internship>> GetUserInternshipsAsync(int userId);
        Task<List<InternshipDefinition>> GetRequiredInternshipsAsync(int specializationId);
        Task<Internship> StartInternshipAsync(Internship internship);
        Task<bool> CompleteInternshipAsync(int internshipId, InternshipDocument completion);
        Task<InternshipDocument> AddDocumentAsync(int internshipId, InternshipDocument document);
        Task<double> GetInternshipProgressAsync(int userId);
    }
}

using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using SledzSpecke.Core.Models.Domain;

namespace SledzSpecke.Core.Interfaces.Services
{
    public interface ISMKIntegrationService
    {
        Task<bool> AuthenticateWithSMKAsync(string username, string password);
        Task<List<ProcedureExecution>> SynchronizeProceduresAsync(int userId);
        Task<bool> ExportToSMKAsync(int userId, DateTime startDate, DateTime endDate);
        Task<List<SMKSpecialization>> GetAvailableSpecializationsFromSMKAsync();
    }
}

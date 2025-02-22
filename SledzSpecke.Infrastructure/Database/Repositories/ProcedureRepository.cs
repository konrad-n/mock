using SledzSpecke.Core.Models.Domain;
using SledzSpecke.Infrastructure.Database.Context;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SledzSpecke.Infrastructure.Database.Repositories
{
    // Infrastructure/Database/Repositories/ProcedureRepository.cs
    public class ProcedureRepository : BaseRepository<ProcedureExecution>, IProcedureRepository
    {
        public ProcedureRepository(IApplicationDbContext context) : base(context)
        {
        }

        public async Task<List<ProcedureExecution>> GetUserProceduresAsync(int userId)
        {
            await _context.InitializeAsync();
            return await _connection.Table<ProcedureExecution>()
                .Where(p => p.UserId == userId)
                .OrderByDescending(p => p.ExecutionDate)
                .ToListAsync();
        }

        public async Task<Dictionary<string, int>> GetProcedureStatsAsync(int userId)
        {
            await _context.InitializeAsync();
            var procedures = await GetUserProceduresAsync(userId);

            return procedures
                .GroupBy(p => p.Type)
                .ToDictionary(
                    g => g.Key.ToString(),
                    g => g.Count()
                );
        }

        public async Task<List<ProcedureExecution>> GetProceduresByCategoryAsync(int userId, string category)
        {
            await _context.InitializeAsync();
            return await _connection.Table<ProcedureExecution>()
                .Where(p => p.UserId == userId && p.Category == category)
                .OrderByDescending(p => p.ExecutionDate)
                .ToListAsync();
        }

        public async Task<ProcedureExecution> GetProcedureWithDetailsAsync(int id)
        {
            await _context.InitializeAsync();
            var procedure = await _connection.GetAsync<ProcedureExecution>(id);

            if (procedure != null)
            {
                // Możesz tu dodać ładowanie dodatkowych danych jeśli są potrzebne
                procedure.Definition = await _connection.GetAsync<ProcedureDefinition>(procedure.ProcedureDefinitionId);
                if (procedure.SupervisorId.HasValue)
                {
                    procedure.Supervisor = await _connection.GetAsync<User>(procedure.SupervisorId.Value);
                }
            }

            return procedure;
        }

        public async Task<List<ProcedureDefinition>> SearchAsync(string query)
        {
            await _context.InitializeAsync();
            return await _connection.Table<ProcedureDefinition>()
                .Where(p => p.Name.ToLower().Contains(query.ToLower()))
                .ToListAsync();
        }
    }
}

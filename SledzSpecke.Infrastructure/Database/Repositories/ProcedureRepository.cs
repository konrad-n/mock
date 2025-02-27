using SledzSpecke.Core.Models.Domain;
using SledzSpecke.Infrastructure.Database.Context;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SledzSpecke.Infrastructure.Database.Repositories
{
    public class ProcedureRepository : BaseRepository<ProcedureExecution>, IProcedureRepository
    {
        public ProcedureRepository(IApplicationDbContext context) : base(context)
        {
        }

        /// <summary>
        /// Zwraca listę procedur użytkownika (bez dodatkowych wyliczeń).
        /// </summary>
        public async Task<List<ProcedureExecution>> GetUserProceduresAsync(int userId)
        {
            await _context.InitializeAsync();
            return await _connection.Table<ProcedureExecution>()
                .Where(p => p.UserId == userId)
                .OrderByDescending(p => p.ExecutionDate)
                .ToListAsync();
        }

        /// <summary>
        /// Pobiera pojedynczy rekord procedury z potencjalnie dołączonymi szczegółami.
        /// </summary>
        public async Task<ProcedureExecution> GetProcedureWithDetailsAsync(int id)
        {
            await _context.InitializeAsync();
            var procedure = await _connection.GetAsync<ProcedureExecution>(id);

            if (procedure != null)
            {
                if (procedure.ProcedureRequirementId.HasValue)
                {
                    procedure.ProcedureRequirement =
                        await _connection.GetAsync<ProcedureRequirement>(procedure.ProcedureRequirementId.Value);
                }

                if (procedure.SupervisorId.HasValue)
                {
                    procedure.Supervisor =
                        await _connection.GetAsync<User>(procedure.SupervisorId.Value);
                }
            }

            return procedure;
        }

        /// <summary>
        /// Zwraca wymagania dotyczące procedur dla danej specjalizacji.
        /// </summary>
        public async Task<List<ProcedureRequirement>> GetRequirementsForSpecializationAsync(int specializationId)
        {
            await _context.InitializeAsync();
            return await _connection.Table<ProcedureRequirement>()
                .Where(p => p.SpecializationId == specializationId)
                .ToListAsync();
        }
    }
}

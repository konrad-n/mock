using SledzSpecke.Core.Models.Domain;
using SledzSpecke.Infrastructure.Database.Context;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SledzSpecke.Infrastructure.Database.Repositories
{
    public class ProcedureRepository : BaseRepository<ProcedureExecution>, IProcedureRepository
    {
        public ProcedureRepository(IApplicationDbContext context) : base(context) { }
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
        
        public async Task<ProcedureExecution> GetProcedureWithDetailsAsync(int id)
        {
            await _context.InitializeAsync();
            var procedure = await _connection.GetAsync<ProcedureExecution>(id);
            
            if (procedure != null)
            {
                if (procedure.ProcedureRequirementId.HasValue)
                {
                    procedure.ProcedureRequirement = await _connection.GetAsync<ProcedureRequirement>(procedure.ProcedureRequirementId.Value);
                }
                
                if (procedure.SupervisorId.HasValue)
                {
                    procedure.Supervisor = await _connection.GetAsync<User>(procedure.SupervisorId.Value);
                }
            }

            return procedure;
        }

        public async Task<List<ProcedureRequirement>> GetRequirementsForSpecializationAsync(int specializationId)
        {
            await _context.InitializeAsync();
            return await _connection.Table<ProcedureRequirement>()
                .Where(p => p.SpecializationId == specializationId)
                .ToListAsync();
        }

        public async Task<Dictionary<string, (int Required, int Completed, int Assisted)>> GetProcedureProgressByCategoryAsync(int userId, int specializationId)
        {
            await _context.InitializeAsync();

            var requirements = await GetRequirementsForSpecializationAsync(specializationId);
            
            var userProcedures = await _connection.Table<ProcedureExecution>()
                .Where(p => p.UserId == userId)
                .ToListAsync();

            var categorizedRequirements = requirements
                .GroupBy(r => r.Category ?? "Uncategorized")
                .ToDictionary(
                    g => g.Key,
                    g => (
                        Required: g.Sum(r => r.RequiredCount),
                        Assisted: g.Sum(r => r.AssistanceCount),
                        Completed: 0
                    ));

            foreach (var procedure in userProcedures)
            {
                var category = procedure.Category ?? "Uncategorized";
                if (!categorizedRequirements.ContainsKey(category))
                {
                    categorizedRequirements[category] = (0, 0, 0);
                }
                
                var current = categorizedRequirements[category];
                if (procedure.Type == Core.Models.Enums.ProcedureType.Execution)
                {
                    categorizedRequirements[category] = (current.Required, current.Assisted, current.Completed + 1);
                }
                else
                {
                    categorizedRequirements[category] = (current.Required, current.Assisted - 1, current.Completed);
                }
            }
            
            return categorizedRequirements;
        }

        public async Task<Dictionary<string, (int Required, int Completed, int Assisted)>> GetProcedureProgressByStageAsync(int userId, int specializationId)
        {
            await _context.InitializeAsync();

            var requirements = await GetRequirementsForSpecializationAsync(specializationId);

            var userProcedures = await _connection.Table<ProcedureExecution>()
                .Where(p => p.UserId == userId)
                .ToListAsync();

            var stagedRequirements = requirements
                .GroupBy(r => r.Stage ?? "Uncategorized")
                .ToDictionary(
                    g => g.Key,
                    g => (
                        Required: g.Sum(r => r.RequiredCount),
                        Assisted: g.Sum(r => r.AssistanceCount),
                        Completed: 0
                    ));
            
            foreach (var procedure in userProcedures)
            {
                var stage = procedure.Stage ?? "Uncategorized";
                if (!stagedRequirements.ContainsKey(stage))
                {
                    stagedRequirements[stage] = (0, 0, 0);
                }
                
                var current = stagedRequirements[stage];
                if (procedure.Type == Core.Models.Enums.ProcedureType.Execution)
                {
                    stagedRequirements[stage] = (current.Required, current.Assisted, current.Completed + 1);
                }
                else
                {
                    stagedRequirements[stage] = (current.Required, current.Assisted - 1, current.Completed);
                }
            }
            
            return stagedRequirements;
        }
    }
}

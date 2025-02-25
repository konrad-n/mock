using SledzSpecke.Core.Models.Domain;
using SledzSpecke.Infrastructure.Database.Context;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;

namespace SledzSpecke.Infrastructure.Database.Repositories
{
    public class InternshipRepository : BaseRepository<Internship>, IInternshipRepository
    {
        public InternshipRepository(IApplicationDbContext context) : base(context) { }

        public async Task<List<Internship>> GetUserInternshipsAsync(int userId)
        {
            await _context.InitializeAsync();
            return await _connection.Table<Internship>()
                .Where(i => i.UserId == userId)
                .OrderBy(i => i.StartDate)
                .ToListAsync();
        }

        public async Task<List<InternshipDefinition>> GetRequiredInternshipsAsync(int specializationId)
        {
            await _context.InitializeAsync();
            return await _connection.Table<InternshipDefinition>()
                .Where(i => i.SpecializationId == specializationId && i.IsRequired)
                .OrderBy(i => i.RecommendedYear)
                .ToListAsync();
        }

        public async Task<List<InternshipModule>> GetModulesForInternshipAsync(int internshipDefinitionId)
        {
            await _context.InitializeAsync();
            return await _connection.Table<InternshipModule>()
                .Where(m => m.InternshipDefinitionId == internshipDefinitionId)
                .ToListAsync();
        }

        public async Task<Dictionary<string, List<string>>> GetRequiredSkillsByInternshipAsync(int internshipDefinitionId)
        {
            await _context.InitializeAsync();
            var modules = await GetModulesForInternshipAsync(internshipDefinitionId);
            var skillsDictionary = new Dictionary<string, List<string>>();
            
            foreach (var module in modules)
            {
                skillsDictionary[module.Name] = module.RequiredSkills;
            }
            
            return skillsDictionary;
        }

        public async Task<Dictionary<string, Dictionary<string, int>>> GetRequiredProceduresByInternshipAsync(int internshipDefinitionId)
        {
            await _context.InitializeAsync();
            var modules = await GetModulesForInternshipAsync(internshipDefinitionId);
            var proceduresDictionary = new Dictionary<string, Dictionary<string, int>>();
            
            foreach (var module in modules)
            {
                proceduresDictionary[module.Name] = module.RequiredProcedures;
            }
            
            return proceduresDictionary;
        }

        public async Task<double> GetInternshipProgressAsync(int userId, int specializationId)
        {
            await _context.InitializeAsync();
            var requiredInternships = await GetRequiredInternshipsAsync(specializationId);
            var userInternships = await GetUserInternshipsAsync(userId);
            
            if (requiredInternships.Count == 0)
                return 1.0; // 100% jeśli nie ma wymaganych staży
            
            var completedCount = userInternships.Count(i => i.IsCompleted);
            return (double)completedCount / requiredInternships.Count;
        }

        public async Task<Dictionary<string, (int Required, int Completed)>> GetInternshipProgressByYearAsync(int userId, int specializationId)
        {
            await _context.InitializeAsync();
            var requiredInternships = await GetRequiredInternshipsAsync(specializationId);
            var userInternships = await GetUserInternshipsAsync(userId);
            
            var progressByYear = requiredInternships
                .GroupBy(i => i.RecommendedYear)
                .ToDictionary(
                    g => $"Rok {g.Key}",
                    g => (Required: g.Count(), Completed: 0)
                );
            
            foreach (var internship in userInternships.Where(i => i.IsCompleted))
            {
                // Znajdź odpowiadającą definicję stażu
                var definition = requiredInternships.FirstOrDefault(d => d.Id == internship.InternshipDefinitionId);
                if (definition != null)
                {
                    var year = $"Rok {definition.RecommendedYear}";
                    if (progressByYear.ContainsKey(year))
                    {
                        var current = progressByYear[year];
                        progressByYear[year] = (current.Required, current.Completed + 1);
                    }
                }
            }
            
            return progressByYear;
        }
    }
}

using SledzSpecke.Core.Models.Domain;
using SledzSpecke.Infrastructure.Database.Context;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SledzSpecke.Infrastructure.Database.Repositories
{
    public class SpecializationRepository : BaseRepository<Specialization>, ISpecializationRepository
    {
        public SpecializationRepository(IApplicationDbContext context) : base(context)
        {
        }

        public async Task<Specialization> GetWithRequirementsAsync(int id)
        {
            await _context.InitializeAsync();
            var specialization = await _connection.GetAsync<Specialization>(id);

            if (specialization != null)
            {
                var courses = await _connection.Table<CourseDefinition>()
                    .Where(c => c.SpecializationId == id)
                    .ToListAsync();

                specialization.RequiredCourses = courses;

                var internships = await _connection.Table<InternshipDefinition>()
                    .Where(i => i.SpecializationId == id)
                    .ToListAsync();

                specialization.RequiredInternships = internships;

                var procedureRequirements = await _connection.Table<ProcedureRequirement>()
                    .Where(p => p.SpecializationId == id)
                    .ToListAsync();

                specialization.ProcedureRequirements = procedureRequirements;

                var dutyRequirements = await _connection.Table<DutyRequirement>()
                    .Where(d => d.SpecializationId == id)
                    .ToListAsync();

                specialization.DutyRequirements = dutyRequirements;
            }

            return specialization;
        }
    }
}

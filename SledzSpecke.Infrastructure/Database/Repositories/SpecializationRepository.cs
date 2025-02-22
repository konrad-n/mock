using SledzSpecke.Core.Models.Domain;
using SledzSpecke.Infrastructure.Database.Context;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SledzSpecke.Infrastructure.Database.Repositories
{
    // Infrastructure/Database/Repositories/SpecializationRepository.cs
    public class SpecializationRepository : BaseRepository<Specialization>, ISpecializationRepository
    {
        public SpecializationRepository(IApplicationDbContext context) : base(context)
        {
        }

        public async Task<List<Specialization>> GetActiveSpecializationsAsync()
        {
            await _context.InitializeAsync();
            return await _connection.Table<Specialization>()
                .ToListAsync();
        }

        public async Task<Specialization> GetWithRequirementsAsync(int id)
        {
            await _context.InitializeAsync();
            var specialization = await _connection.GetAsync<Specialization>(id);

            if (specialization != null)
            {
                // Załaduj powiązane wymagania
                specialization.RequiredCourses = await _connection.Table<Course>()
                    .Where(c => c.SpecializationId == id)
                    .ToListAsync();

                specialization.RequiredInternships = await _connection.Table<Internship>()
                    .Where(i => i.SpecializationId == id)
                    .ToListAsync();
            }

            return specialization;
        }
    }
}

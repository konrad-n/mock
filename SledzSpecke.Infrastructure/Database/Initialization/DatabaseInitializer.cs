using SledzSpecke.Infrastructure.Database.Context;
using SledzSpecke.Infrastructure.Database.Repositories;
using System.Linq;
using System.Threading.Tasks;

namespace SledzSpecke.Infrastructure.Database.Initialization
{
    public class DatabaseInitializer
    {
        private readonly IApplicationDbContext _context;
        private readonly ISpecializationRepository _specializationRepo;

        public DatabaseInitializer(IApplicationDbContext context,
                                 ISpecializationRepository specializationRepo)
        {
            _context = context;
            _specializationRepo = specializationRepo;
        }

        public async Task InitializeAsync()
        {
            await _context.InitializeAsync();
            await SeedBasicDataAsync();
        }

        private async Task SeedBasicDataAsync()
        {
            // Sprawdź czy dane już istnieją
            var hasData = await _specializationRepo.GetAllAsync();
            if (hasData.Any()) return;

            // Zainicjuj podstawowe dane
            await SeedSpecializationsAsync();
            await SeedProcedureDefinitionsAsync();
            await SeedCourseDefinitionsAsync();
        }
    }
}

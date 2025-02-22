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
        private readonly IProcedureRepository _procedureRepo;
        private readonly ICourseRepository _courseRepo;


        public DatabaseInitializer(IApplicationDbContext context,
                                 ISpecializationRepository specializationRepo,
                                 IProcedureRepository procedureRepo,
                                 ICourseRepository courseRepo)
        {
            _context = context;
            _specializationRepo = specializationRepo;
            _procedureRepo = procedureRepo;
            _courseRepo = courseRepo;

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

        private async Task SeedSpecializationsAsync()
        {
            var specializations = DataSeeder.GetBasicSpecializations();
            foreach (var spec in specializations)
            {
                await _specializationRepo.AddAsync(spec);
            }
        }

        private async Task SeedProcedureDefinitionsAsync()
        {
            var procedures = DataSeeder.GetBasicProcedures();
            foreach (var proc in procedures)
            {
                await _procedureRepo.AddAsync(proc);
            }
        }

        private async Task SeedCourseDefinitionsAsync()
        {
            var courses = DataSeeder.GetBasicCourses();
            foreach (var course in courses)
            {
                await _courseRepo.AddAsync(course);
            }
        }
    }
}

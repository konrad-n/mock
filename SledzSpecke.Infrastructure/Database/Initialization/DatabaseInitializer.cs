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
        private readonly IInternshipRepository _internshipRepo;
        private readonly IDutyRepository _dutyRepo;

        public DatabaseInitializer(IApplicationDbContext context,
                                 ISpecializationRepository specializationRepo,
                                 IProcedureRepository procedureRepo,
                                 ICourseRepository courseRepo,
                                 IInternshipRepository internshipRepo,
                                 IDutyRepository dutyRepo)
        {
            _context = context;
            _specializationRepo = specializationRepo;
            _procedureRepo = procedureRepo;
            _courseRepo = courseRepo;
            _internshipRepo = internshipRepo;
            _dutyRepo = dutyRepo;
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
            await SeedProcedureRequirementsAsync();
            await SeedCourseDefinitionsAsync();
            await SeedInternshipDefinitionsAsync();
            await SeedInternshipModulesAsync();
            await SeedDutyRequirementsAsync();
        }

        private async Task SeedSpecializationsAsync()
        {
            var specializations = DataSeeder.GetBasicSpecializations();
            foreach (var spec in specializations)
            {
                await _specializationRepo.AddAsync(spec);
            }
        }

        private async Task SeedProcedureRequirementsAsync()
        {
            var procedureRequirements = DataSeeder.GetBasicProcedureRequirements();
            foreach (var procReq in procedureRequirements)
            {
                await _procedureRepo.AddAsync(procReq);
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

        private async Task SeedInternshipDefinitionsAsync()
        {
            var internships = DataSeeder.GetBasicInternships();
            foreach (var internship in internships)
            {
                await _internshipRepo.AddAsync(internship);
            }
        }

        private async Task SeedInternshipModulesAsync()
        {
            var modules = DataSeeder.GetInternshipModules();
            foreach (var module in modules)
            {
                await _context.GetConnection().InsertAsync(module);
            }
        }

        private async Task SeedDutyRequirementsAsync()
        {
            var dutyRequirements = DataSeeder.GetDutyRequirements();
            foreach (var dutyReq in dutyRequirements)
            {
                await _dutyRepo.AddAsync(dutyReq);
            }
        }
    }
}

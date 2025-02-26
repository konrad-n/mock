using SledzSpecke.Infrastructure.Database.Context;
using SledzSpecke.Infrastructure.Database.Repositories;
using System;
using System.Linq;
using System.Threading;
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
            try
            {
                // Set up a cancellation token source with timeout
                using var cts = new CancellationTokenSource();
                cts.CancelAfter(TimeSpan.FromSeconds(30)); // 30 second timeout

                // Create a task for initialization
                var initTask = _context.InitializeAsync();

                // Wait for completion or cancellation
                try
                {
                    await initTask;
                    Console.WriteLine("Database context initialized successfully");

                    // Now try to seed data
                    var seedTask = SeedBasicDataAsync();
                    await seedTask;
                    Console.WriteLine("Database seeded successfully");
                }
                catch (OperationCanceledException)
                {
                    Console.WriteLine("Database initialization timed out after 30 seconds");
                    // Handle timeout - perhaps create a fresh database
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Database initialization error: {ex.GetType().Name}: {ex.Message}");
                // Consider logging the full exception details including stack trace
            }
        }

        private async Task SeedBasicDataAsync()
        {
            // Check if data already exists
            var hasData = await _specializationRepo.GetAllAsync();
            if (hasData.Any()) return;

            // Initialize basic data
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
                // Use appropriate repository method for ProcedureRequirement
                await _context.GetConnection().InsertAsync(procReq);
            }
        }

        private async Task SeedCourseDefinitionsAsync()
        {
            var courses = DataSeeder.GetBasicCourses();
            foreach (var course in courses)
            {
                // Use appropriate repository method for CourseDefinition
                await _context.GetConnection().InsertAsync(course);
            }
        }

        private async Task SeedInternshipDefinitionsAsync()
        {
            var internships = DataSeeder.GetBasicInternships();
            foreach (var internship in internships)
            {
                // Use appropriate repository method for InternshipDefinition
                await _context.GetConnection().InsertAsync(internship);
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
                // Use appropriate repository method for DutyRequirement
                await _context.GetConnection().InsertAsync(dutyReq);
            }
        }
    }
}

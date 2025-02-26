using SledzSpecke.Core.Models.Domain;
using SledzSpecke.Infrastructure.Database.Context;
using SledzSpecke.Infrastructure.Database.Repositories;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace SledzSpecke.Infrastructure.Database.Initialization
{
    public class DatabaseInitializer
    {
        private readonly IApplicationDbContext _context;
        private readonly ISpecializationRepository _specializationRepo;
        private readonly IProcedureRepository _procedureRepo;

        public DatabaseInitializer(
            IApplicationDbContext context,
            ISpecializationRepository specializationRepo,
            IProcedureRepository procedureRepo)
        {
            _context = context;
            _specializationRepo = specializationRepo;
            _procedureRepo = procedureRepo;
        }

        public async Task InitializeAsync()
        {
            try
            {
                Console.WriteLine("Starting database initialization");
                await _context.InitializeAsync();
                await SeedBasicDataAsync();
                Console.WriteLine("Database initialization completed successfully");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Database initialization failed: {ex.Message}");
                System.Diagnostics.Debug.WriteLine($"Warning: Database initialization failed: {ex.Message}. App will continue with limited functionality.");
            }
        }

        private async Task SeedBasicDataAsync()
        {
            // Check if data already exists
            var hasData = await _specializationRepo.GetAllAsync();
            if (hasData.Any()) return;

            // Seed specializations
            var psychiatry = new Specialization
            {
                Name = "Psychiatria",
                DurationInWeeks = 312,
                ProgramVersion = "2023",
                ApprovalDate = new DateTime(2023, 1, 1),
                MinimumDutyHours = 1200,
                Requirements = "Program specjalizacji w dziedzinie psychiatrii",
                Description = "Celem szkolenia specjalizacyjnego jest uzyskanie szczególnych kwalifikacji w dziedzinie psychiatrii"
            };

            await _specializationRepo.AddAsync(psychiatry);

            // Seed procedure requirements
            var procedures = new[]
            {
                new ProcedureRequirement
                {
                    SpecializationId = 1, // Psychiatry ID
                    Name = "Badanie psychiatryczne",
                    Description = "Przeprowadzenie badania psychiatrycznego",
                    RequiredCount = 20,
                    AssistanceCount = 0,
                    Category = "Podstawowe procedury",
                    SupervisionRequired = true
                },
                new ProcedureRequirement
                {
                    SpecializationId = 1,
                    Name = "Ocena stanu psychicznego",
                    Description = "Ocena stanu psychicznego za pomocą skal klinicznych",
                    RequiredCount = 20,
                    AssistanceCount = 0,
                    Category = "Diagnostyka",
                    SupervisionRequired = true
                },
                new ProcedureRequirement
                {
                    SpecializationId = 1,
                    Name = "Zabiegi elektrowstrząsowe",
                    Description = "Kwalifikacja i przygotowanie pacjentów oraz przeprowadzenie zabiegów",
                    RequiredCount = 5,
                    AssistanceCount = 0,
                    Category = "Zabiegi",
                    SupervisionRequired = true
                }
            };

            foreach (var proc in procedures)
            {
                await _context.GetConnection().InsertAsync(proc);
            }

            // Create a test user
            var user = new User
            {
                Email = "test@example.com",
                Name = "Test User",
                PWZ = "123456",
                CurrentSpecializationId = 1,
                SpecializationStartDate = DateTime.Now.AddYears(-1),
                ExpectedEndDate = DateTime.Now.AddYears(5)
            };

            await _context.GetConnection().InsertAsync(user);
        }
    }
}

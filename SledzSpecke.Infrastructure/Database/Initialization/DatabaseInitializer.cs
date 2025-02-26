using SledzSpecke.Core.Models.Domain;
using SledzSpecke.Infrastructure.Database.Context;
using SledzSpecke.Infrastructure.Database.Repositories;
using System;
using System.Collections.Generic;
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
                System.Diagnostics.Debug.WriteLine("Starting database initialization");
                await _context.InitializeAsync();
                System.Diagnostics.Debug.WriteLine("Context initialized, seeding basic data");
                await SeedBasicDataAsync();
                System.Diagnostics.Debug.WriteLine("Database initialization completed successfully");
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Database initialization failed: {ex.Message}");
                System.Diagnostics.Debug.WriteLine(ex.StackTrace);
                throw; // Rethrow to be caught in App.xaml.cs
            }
        }

        private async Task SeedBasicDataAsync()
        {
            System.Diagnostics.Debug.WriteLine("Checking if data already exists...");
            // Check if data already exists
            var hasData = await _specializationRepo.GetAllAsync();
            if (hasData.Any())
            {
                System.Diagnostics.Debug.WriteLine("Data already exists, skipping seeding...");
                return;
            }

            System.Diagnostics.Debug.WriteLine("Seeding specializations...");
            // Seed specializations from the new data seeder
            var specializations = DataSeeder.GetBasicSpecializations();
            foreach (var spec in specializations)
            {
                await _specializationRepo.AddAsync(spec);
            }

            System.Diagnostics.Debug.WriteLine("Seeding courses...");
            // Seed courses
            var courses = DataSeeder.GetBasicCourses();
            foreach (var course in courses)
            {
                await _context.GetConnection().InsertAsync(course);
            }

            System.Diagnostics.Debug.WriteLine("Seeding internships...");
            // Seed internships
            var internships = DataSeeder.GetBasicInternships();
            foreach (var internship in internships)
            {
                await _context.GetConnection().InsertAsync(internship);
            }

            // Add this code after inserting internships
            foreach (var internship in internships)
            {
                if (internship.DetailedStructure != null)
                {
                    foreach (var module in internship.DetailedStructure)
                    {
                        module.InternshipDefinitionId = internship.Id;
                        await _context.GetConnection().InsertAsync(module);
                    }
                }
            }

            System.Diagnostics.Debug.WriteLine("Seeding procedures requirements...");
            // Extract procedure requirements from the required procedures dictionary
            var procedureRequirements = new List<ProcedureRequirement>();
            var requiredProcedures = DataSeeder.GetRequiredProcedures();

            foreach (var category in requiredProcedures)
            {
                foreach (var procedure in category.Value)
                {
                    procedureRequirements.Add(new ProcedureRequirement
                    {
                        SpecializationId = 5, // Hematology ID from the new data
                        Name = procedure.Name,
                        Description = $"Procedura z kategorii: {category.Key}",
                        RequiredCount = procedure.RequiredCount,
                        AssistanceCount = procedure.AssistanceCount,
                        Category = category.Key,
                        SupervisionRequired = procedure.RequiredCount > 0 || procedure.AssistanceCount > 0
                    });
                }
            }

            foreach (var proc in procedureRequirements)
            {
                await _context.GetConnection().InsertAsync(proc);
            }

            System.Diagnostics.Debug.WriteLine("Seeding test user...");

            // Create a test user
            var user = new User
            {
                Email = "test@example.com",
                Name = "Test User",
                PWZ = "123456",
                CurrentSpecializationId = 5, // Hematology ID from the new data
                SpecializationStartDate = DateTime.Now.AddYears(-1),
                ExpectedEndDate = DateTime.Now.AddYears(3) // Hematology is 3 years
            };

            await _context.GetConnection().InsertAsync(user);

            System.Diagnostics.Debug.WriteLine("Basic data sync completed.");
        }
    }
}

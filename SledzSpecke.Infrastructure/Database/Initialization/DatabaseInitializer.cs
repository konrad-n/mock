using SledzSpecke.Core.Models.Domain;
using SledzSpecke.Infrastructure.Database.Context;
using SledzSpecke.Infrastructure.Database.Repositories;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SledzSpecke.Infrastructure.Database.Initialization
{
    public class DatabaseInitializer
    {
        private readonly IApplicationDbContext _context;
        private readonly ISpecializationRepository _specializationRepo;

        public DatabaseInitializer(
            IApplicationDbContext context,
            ISpecializationRepository specializationRepo)
        {
            _context = context;
            _specializationRepo = specializationRepo;
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
                throw;
            }
        }

        private async Task SeedBasicDataAsync()
        {
            var specializations = DataSeeder.GetBasicSpecializations();
            foreach (var spec in specializations)
            {
                await _specializationRepo.AddAsync(spec);
            }

            var courses = DataSeeder.GetBasicCourses();
            foreach (var course in courses)
            {
                await _context.GetConnection().InsertAsync(course);
            }

            var internships = DataSeeder.GetBasicInternships();
            foreach (var internship in internships)
            {
                await _context.GetConnection().InsertAsync(internship);
            }

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

            var procedureRequirements = new List<ProcedureRequirement>();
            var requiredProcedures = DataSeeder.GetRequiredProcedures();

            foreach (var category in requiredProcedures)
            {
                foreach (var procedure in category.Value)
                {
                    procedureRequirements.Add(new ProcedureRequirement
                    {
                        SpecializationId = 5,
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

            var user = new User
            {
                Email = "test@example.com",
                Name = "Test User",
                PWZ = "123456",
                CurrentSpecializationId = 1,
                SpecializationStartDate = DateTime.Now.AddYears(-1),
                ExpectedEndDate = DateTime.Now.AddYears(3)
            };

            await _context.GetConnection().InsertAsync(user);
        }
    }
}

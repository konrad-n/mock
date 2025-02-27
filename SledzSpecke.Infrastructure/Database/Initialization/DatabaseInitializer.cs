using SledzSpecke.Core.Models.Domain;
using SledzSpecke.Core.Models.Enums;
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
            int targetSpecializationId = 1;

            foreach (var category in requiredProcedures)
            {
                foreach (var procedure in category.Value)
                {
                    // Tylko dodaj wymagania dla specjalizacji hematologia
                    if (procedure.SpecializationId == targetSpecializationId)
                    {
                        procedureRequirements.Add(new ProcedureRequirement
                        {
                            SpecializationId = procedure.SpecializationId,
                            Name = procedure.Name,
                            Description = $"Procedura z kategorii: {category.Key}",
                            RequiredCount = procedure.RequiredCount,
                            AssistanceCount = procedure.AssistanceCount,
                            Category = category.Key,
                            Stage = "Etap podstawowy",
                            SupervisionRequired = procedure.RequiredCount > 0 || procedure.AssistanceCount > 0,
                            AllowSimulation = procedure.AllowSimulation,
                            SimulationLimit = procedure.SimulationLimit.HasValue ? procedure.SimulationLimit.Value : 0
                        });
                    }
                }
            }

            foreach (var proc in procedureRequirements)
            {
                await _context.GetConnection().InsertAsync(proc);
            }

            var user = new User
            {
                Email = "test@example.com",
                Name = "Jan Kowalski",
                CurrentSpecializationId = targetSpecializationId,
                SpecializationStartDate = DateTime.Now.AddYears(-1),
                ExpectedEndDate = DateTime.Now.AddYears(3),
                CreatedAt = DateTime.Now
            };

            await _context.GetConnection().InsertAsync(user);
            await AddSampleProceduresAsync(user.Id, targetSpecializationId, procedureRequirements);
            await AddSampleDutiesAsync(user.Id);
            await AddSampleCoursesAsync(user.Id, targetSpecializationId);
            await AddSampleInternshipsAsync(user.Id, targetSpecializationId);
        }

        private async Task AddSampleProceduresAsync(int userId, int specializationId, List<ProcedureRequirement> requirements)
        {
            var procedures = new List<ProcedureExecution>();
            var random = new Random();

            var sampleRequirements = requirements.Count > 10
                ? requirements.GetRange(0, 10)
                : requirements;

            foreach (var req in sampleRequirements)
            {
                int executionCount = Math.Min(req.RequiredCount, 3);
                for (int i = 0; i < executionCount; i++)
                {
                    var proc = new ProcedureExecution
                    {
                        UserId = userId,
                        Name = req.Name,
                        ExecutionDate = DateTime.Now.AddDays(-random.Next(1, 60)),
                        Type = ProcedureType.Execution,
                        Location = "Oddział Hematologii",
                        Notes = $"Wykonana procedura {req.Name}. Opiekun: dr Adam Nowak",
                        Category = req.Category,
                        Stage = req.Stage,
                        ProcedureRequirementId = req.Id,
                        CreatedAt = DateTime.Now,
                        IsSimulation = random.Next(5) == 0
                    };
                    procedures.Add(proc);
                }

                // Add assistance procedures
                int assistanceCount = Math.Min(req.AssistanceCount, 2);
                for (int i = 0; i < assistanceCount; i++)
                {
                    var proc = new ProcedureExecution
                    {
                        UserId = userId,
                        Name = req.Name,
                        ExecutionDate = DateTime.Now.AddDays(-random.Next(1, 60)),
                        Type = ProcedureType.Assistance,
                        Location = "Oddział Hematologii",
                        Notes = $"Asystowanie przy procedurze {req.Name}. Wykonujący: dr Adam Nowak",
                        Category = req.Category,
                        Stage = req.Stage,
                        ProcedureRequirementId = req.Id,
                        CreatedAt = DateTime.Now
                    };
                    procedures.Add(proc);
                }
            }

            foreach (var proc in procedures)
            {
                await _context.GetConnection().InsertAsync(proc);
            }
        }

        private async Task AddSampleDutiesAsync(int userId)
        {
            var duties = new List<Duty>();
            var random = new Random();
            var dutyTypes = Enum.GetValues(typeof(DutyType));

            for (int month = 1; month <= 6; month++)
            {
                int dutiesCount = random.Next(3, 6);

                for (int i = 0; i < dutiesCount; i++)
                {
                    var startDate = DateTime.Now.AddMonths(-month).AddDays(random.Next(1, 28));
                    var dutyType = (DutyType)dutyTypes.GetValue(random.Next(dutyTypes.Length));
                    var durationHours = random.Next(8, 25);

                    var duty = new Duty
                    {
                        UserId = userId,
                        StartTime = startDate.Date.AddHours(8),
                        EndTime = startDate.Date.AddHours(8 + durationHours),
                        Location = dutyType == DutyType.Emergency ? "SOR" : "Oddział Hematologii",
                        Type = dutyType,
                        Notes = $"Dyżur {dutyType} w dniu {startDate:d}",
                        DurationInHours = durationHours,
                        IsConfirmed = true,
                        CreatedAt = DateTime.Now
                    };

                    duties.Add(duty);
                }
            }

            for (int i = 0; i < 3; i++)
            {
                var startDate = DateTime.Now.AddDays(random.Next(1, 30));
                var dutyType = (DutyType)dutyTypes.GetValue(random.Next(dutyTypes.Length));
                var durationHours = random.Next(8, 25);

                var duty = new Duty
                {
                    UserId = userId,
                    StartTime = startDate.Date.AddHours(8),
                    EndTime = startDate.Date.AddHours(8 + durationHours),
                    Location = dutyType == DutyType.Emergency ? "SOR" : "Oddział Hematologii",
                    Type = dutyType,
                    Notes = $"Planowany dyżur {dutyType} w dniu {startDate:d}",
                    DurationInHours = durationHours,
                    IsConfirmed = false,
                    CreatedAt = DateTime.Now
                };

                duties.Add(duty);
            }

            foreach (var duty in duties)
            {
                await _context.GetConnection().InsertAsync(duty);
            }
        }

        private async Task AddSampleCoursesAsync(int userId, int specializationId)
        {
            var courseDefinitions = DataSeeder.GetBasicCourses()
                .FindAll(c => c.SpecializationId == specializationId)
                .GetRange(0, Math.Min(5, DataSeeder.GetBasicCourses().Count));

            var random = new Random();
            var courses = new List<Course>();

            foreach (var definition in courseDefinitions)
            {
                bool isCompleted = random.Next(10) < 7;
                DateTime? startDate = DateTime.Now.AddMonths(-random.Next(1, 12));
                DateTime? endDate = isCompleted ? startDate?.AddDays(definition.DurationInDays) : null;

                var course = new Course
                {
                    UserId = userId,
                    CourseDefinitionId = definition.Id,
                    StartDate = startDate,
                    EndDate = endDate,
                    Location = "Centrum Kształcenia Podyplomowego",
                    Organizer = "Centrum Medyczne Kształcenia Podyplomowego",
                    Status = isCompleted ? CourseStatus.Completed : CourseStatus.Registered,
                    IsCompleted = isCompleted,
                    CompletionDate = isCompleted ? endDate : null,
                    CertificateNumber = isCompleted ? $"CERT/{random.Next(10000, 99999)}/{DateTime.Now.Year}" : null,
                    Notes = $"Kurs: {definition.Name}",
                    SpecializationId = specializationId,
                    CreatedAt = DateTime.Now
                };

                courses.Add(course);
            }

            foreach (var course in courses)
            {
                await _context.GetConnection().InsertAsync(course);
            }
        }

        private async Task AddSampleInternshipsAsync(int userId, int specializationId)
        {
            var internshipDefinitions = DataSeeder.GetBasicInternships()
                .FindAll(i => i.SpecializationId == specializationId)
                .GetRange(0, Math.Min(3, DataSeeder.GetBasicInternships().Count));

            var random = new Random();
            var internships = new List<Internship>();

            foreach (var definition in internshipDefinitions)
            {
                bool isCompleted = random.Next(2) == 0;
                DateTime? startDate = DateTime.Now.AddMonths(-random.Next(1, 24));
                DateTime? endDate = isCompleted ? startDate?.AddDays(definition.DurationInWeeks * 7) : null;

                var internship = new Internship
                {
                    UserId = userId,
                    InternshipDefinitionId = definition.Id,
                    StartDate = startDate,
                    EndDate = endDate,
                    Location = "Klinika Hematologii",
                    Status = isCompleted ? InternshipStatus.Completed : InternshipStatus.InProgress,
                    Notes = $"Staż: {definition.Name}",
                    IsCompleted = isCompleted,
                    CompletionDate = isCompleted ? endDate : null,
                    SpecializationId = specializationId,
                    CreatedAt = DateTime.Now
                };

                internships.Add(internship);
            }

            foreach (var internship in internships)
            {
                await _context.GetConnection().InsertAsync(internship);
            }
        }
    }
}

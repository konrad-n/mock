using SledzSpecke.Core.Models.Domain;
using SQLite;
using System.Threading.Tasks;

namespace SledzSpecke.Infrastructure.Database.Migrations
{
    public class M019_EnhanceSpecializationModels : BaseMigration
    {
        public M019_EnhanceSpecializationModels(SQLiteAsyncConnection connection) : base(connection)
        {
            Version = 19;
            Description = "Enhance specialization data models to match seeders";
        }

        public override async Task UpAsync()
        {
            // 1. Aktualizacja tabeli Specialization
            await _connection.ExecuteAsync(@"
            ALTER TABLE Specialization ADD COLUMN MinimumDutyHours REAL DEFAULT 0;
            ");

            // 2. Aktualizacja tabeli CourseDefinition
            await _connection.ExecuteAsync(@"
            ALTER TABLE CourseDefinition ADD COLUMN DurationInDays INTEGER DEFAULT 1;
            ALTER TABLE CourseDefinition ADD COLUMN IsRequired INTEGER DEFAULT 1;
            ALTER TABLE CourseDefinition ADD COLUMN CanBeRemote INTEGER DEFAULT 0;
            ALTER TABLE CourseDefinition ADD COLUMN RecommendedYear INTEGER DEFAULT 1;
            ALTER TABLE CourseDefinition ADD COLUMN Requirements TEXT;
            ALTER TABLE CourseDefinition ADD COLUMN CompletionRequirements TEXT;
            ALTER TABLE CourseDefinition ADD COLUMN CourseTopicsJson TEXT;
            ");

            // 3. Utworzenie tabeli InternshipModule
            await _connection.CreateTableAsync<InternshipModule>();

            // 4. Aktualizacja tabeli InternshipDefinition
            await _connection.ExecuteAsync(@"
            ALTER TABLE InternshipDefinition ADD COLUMN IsRequired INTEGER DEFAULT 1;
            ALTER TABLE InternshipDefinition ADD COLUMN RecommendedYear INTEGER DEFAULT 1;
            ALTER TABLE InternshipDefinition ADD COLUMN Requirements TEXT;
            ALTER TABLE InternshipDefinition ADD COLUMN CompletionRequirementsJson TEXT;
            ");

            // 5. Aktualizacja tabeli ProcedureRequirement
            await _connection.ExecuteAsync(@"
            ALTER TABLE ProcedureRequirement ADD COLUMN RequiredCount INTEGER DEFAULT 0;
            ALTER TABLE ProcedureRequirement ADD COLUMN AssistanceCount INTEGER DEFAULT 0;
            ALTER TABLE ProcedureRequirement ADD COLUMN SupervisionRequired INTEGER DEFAULT 0;
            ALTER TABLE ProcedureRequirement ADD COLUMN Category TEXT;
            ALTER TABLE ProcedureRequirement ADD COLUMN Stage TEXT;
            ALTER TABLE ProcedureRequirement ADD COLUMN AllowSimulation INTEGER DEFAULT 0;
            ALTER TABLE ProcedureRequirement ADD COLUMN SimulationLimit INTEGER DEFAULT 0;
            ");

            // 6. Aktualizacja tabeli ProcedureExecution
            await _connection.ExecuteAsync(@"
            ALTER TABLE ProcedureExecution ADD COLUMN IsSimulation INTEGER DEFAULT 0;
            ALTER TABLE ProcedureExecution ADD COLUMN Category TEXT;
            ALTER TABLE ProcedureExecution ADD COLUMN Stage TEXT;
            ALTER TABLE ProcedureExecution ADD COLUMN ProcedureRequirementId INTEGER;
            ");

            // 7. Aktualizacja tabeli DutyRequirement
            await _connection.ExecuteAsync(@"
            ALTER TABLE DutyRequirement ADD COLUMN RequiresSupervision INTEGER DEFAULT 0;
            ALTER TABLE DutyRequirement ADD COLUMN MinimumHoursPerMonth INTEGER DEFAULT 0;
            ALTER TABLE DutyRequirement ADD COLUMN MinimumDutiesPerMonth INTEGER DEFAULT 0;
            ALTER TABLE DutyRequirement ADD COLUMN RequiredCompetenciesJson TEXT;
            ");
        }

        public override async Task DownAsync()
        {
            // Implementacja powrotu do poprzedniej wersji (opcjonalnie)
        }
    }
}

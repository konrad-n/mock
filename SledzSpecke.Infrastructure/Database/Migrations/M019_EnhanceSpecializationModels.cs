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
            // 1. Check if MinimumDutyHours column already exists in Specialization table
            var hasMinimumDutyHours = await CheckColumnExistsAsync("Specialization", "MinimumDutyHours");

            // Only add the column if it doesn't exist
            if (!hasMinimumDutyHours)
            {
                await _connection.ExecuteAsync(@"
                ALTER TABLE Specialization ADD COLUMN MinimumDutyHours REAL DEFAULT 0;
                ");
            }

            // 2. Check CourseDefinition columns before adding
            await SafeAddColumnAsync("CourseDefinition", "DurationInDays", "INTEGER", "1");
            await SafeAddColumnAsync("CourseDefinition", "IsRequired", "INTEGER", "1");
            await SafeAddColumnAsync("CourseDefinition", "CanBeRemote", "INTEGER", "0");
            await SafeAddColumnAsync("CourseDefinition", "RecommendedYear", "INTEGER", "1");
            await SafeAddColumnAsync("CourseDefinition", "Requirements", "TEXT");
            await SafeAddColumnAsync("CourseDefinition", "CompletionRequirements", "TEXT");
            await SafeAddColumnAsync("CourseDefinition", "CourseTopicsJson", "TEXT");

            // 3. Utworzenie tabeli InternshipModule - CreateTable jest bezpieczne, jeśli już istnieje, to nie tworzy
            await _connection.CreateTableAsync<InternshipModule>();

            // 4. Check InternshipDefinition columns before adding
            await SafeAddColumnAsync("InternshipDefinition", "IsRequired", "INTEGER", "1");
            await SafeAddColumnAsync("InternshipDefinition", "RecommendedYear", "INTEGER", "1");
            await SafeAddColumnAsync("InternshipDefinition", "Requirements", "TEXT");
            await SafeAddColumnAsync("InternshipDefinition", "CompletionRequirementsJson", "TEXT");

            // 5. Check ProcedureRequirement columns before adding
            await SafeAddColumnAsync("ProcedureRequirement", "RequiredCount", "INTEGER", "0");
            await SafeAddColumnAsync("ProcedureRequirement", "AssistanceCount", "INTEGER", "0");
            await SafeAddColumnAsync("ProcedureRequirement", "SupervisionRequired", "INTEGER", "0");
            await SafeAddColumnAsync("ProcedureRequirement", "Category", "TEXT");
            await SafeAddColumnAsync("ProcedureRequirement", "Stage", "TEXT");
            await SafeAddColumnAsync("ProcedureRequirement", "AllowSimulation", "INTEGER", "0");
            await SafeAddColumnAsync("ProcedureRequirement", "SimulationLimit", "INTEGER", "0");

            // 6. Check ProcedureExecution columns before adding
            await SafeAddColumnAsync("ProcedureExecution", "IsSimulation", "INTEGER", "0");
            await SafeAddColumnAsync("ProcedureExecution", "Category", "TEXT");
            await SafeAddColumnAsync("ProcedureExecution", "Stage", "TEXT");
            await SafeAddColumnAsync("ProcedureExecution", "ProcedureRequirementId", "INTEGER");

            // 7. Check DutyRequirement columns before adding
            await SafeAddColumnAsync("DutyRequirement", "RequiresSupervision", "INTEGER", "0");
            await SafeAddColumnAsync("DutyRequirement", "MinimumHoursPerMonth", "INTEGER", "0");
            await SafeAddColumnAsync("DutyRequirement", "MinimumDutiesPerMonth", "INTEGER", "0");
            await SafeAddColumnAsync("DutyRequirement", "RequiredCompetenciesJson", "TEXT");

            // 8. Check InternshipModule columns before adding
            await SafeAddColumnAsync("InternshipModule", "AssistantProceduresJson", "TEXT");
        }

        public override async Task DownAsync()
        {
            // Implementacja powrotu do poprzedniej wersji (opcjonalnie)
        }

        private async Task<bool> CheckColumnExistsAsync(string tableName, string columnName)
        {
            var tableInfo = await _connection.QueryAsync<TableInfoResult>($"PRAGMA table_info({tableName})");
            foreach (var column in tableInfo)
            {
                if (column.Name.Equals(columnName, System.StringComparison.OrdinalIgnoreCase))
                {
                    return true;
                }
            }
            return false;
        }

        private async Task<bool> SafeAddColumnAsync(string tableName, string columnName, string columnType, string defaultValue = null)
        {
            if (await CheckColumnExistsAsync(tableName, columnName))
            {
                return false; // Column already exists
            }

            string sql = $"ALTER TABLE {tableName} ADD COLUMN {columnName} {columnType}";
            if (defaultValue != null)
            {
                sql += $" DEFAULT {defaultValue}";
            }

            await _connection.ExecuteAsync(sql);
            return true;
        }

        private class TableInfoResult
        {
            public int Cid { get; set; }
            public string Name { get; set; }
            public string Type { get; set; }
            public int NotNull { get; set; }
            public string DefaultValue { get; set; }
            public int Pk { get; set; }
        }
    }
}

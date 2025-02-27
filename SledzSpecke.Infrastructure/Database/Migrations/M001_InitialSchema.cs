using SledzSpecke.Core.Models.Domain;
using SledzSpecke.Infrastructure.Database.Extensions;
using SQLite;
using System.Threading.Tasks;

namespace SledzSpecke.Infrastructure.Database.Migrations
{
    public class M001_InitialSchema : BaseMigration
    {
        public M001_InitialSchema(SQLiteAsyncConnection connection) : base(connection)
        {
            Version = 1;
            Description = "Initial schema creation";
        }

        public override async Task UpAsync()
        {
            if (!await _connection.TableExistsAsync<User>())
                await _connection.CreateTableAsync<User>();

            if (!await _connection.TableExistsAsync<Specialization>())
                await _connection.CreateTableAsync<Specialization>();

            if (!await _connection.TableExistsAsync<ProcedureDefinition>())
                await _connection.CreateTableAsync<ProcedureDefinition>();

            if (!await _connection.TableExistsAsync<ProcedureExecution>())
                await _connection.CreateTableAsync<ProcedureExecution>();

            if (!await _connection.TableExistsAsync<ProcedureRequirement>())
                await _connection.CreateTableAsync<ProcedureRequirement>();

            if (!await _connection.TableExistsAsync<Duty>())
                await _connection.CreateTableAsync<Duty>();

            if (!await _connection.TableExistsAsync<DutyRequirement>())
                await _connection.CreateTableAsync<DutyRequirement>();

            if (!await _connection.TableExistsAsync<Course>())
                await _connection.CreateTableAsync<Course>();

            if (!await _connection.TableExistsAsync<CourseDefinition>())
                await _connection.CreateTableAsync<CourseDefinition>();

            if (!await _connection.TableExistsAsync<Internship>())
                await _connection.CreateTableAsync<Internship>();

            if (!await _connection.TableExistsAsync<InternshipDefinition>())
                await _connection.CreateTableAsync<InternshipDefinition>();

            if (!await _connection.TableExistsAsync<CourseDocument>())
                await _connection.CreateTableAsync<CourseDocument>();

            if (!await _connection.TableExistsAsync<InternshipDocument>())
                await _connection.CreateTableAsync<InternshipDocument>();

            if (!await _connection.TableExistsAsync<DutyStatistics>())
                await _connection.CreateTableAsync<DutyStatistics>();

            if (!await _connection.TableExistsAsync<SpecializationProgress>())
                await _connection.CreateTableAsync<SpecializationProgress>();
        }

        public override async Task DownAsync()
        {
            await _connection.DropTableAsync<SpecializationProgress>();
            await _connection.DropTableAsync<DutyStatistics>();
            await _connection.DropTableAsync<InternshipDocument>();
            await _connection.DropTableAsync<CourseDocument>();
            await _connection.DropTableAsync<InternshipDefinition>();
            await _connection.DropTableAsync<Internship>();
            await _connection.DropTableAsync<CourseDefinition>();
            await _connection.DropTableAsync<Course>();
            await _connection.DropTableAsync<DutyRequirement>();
            await _connection.DropTableAsync<Duty>();
            await _connection.DropTableAsync<ProcedureRequirement>();
            await _connection.DropTableAsync<ProcedureExecution>();
            await _connection.DropTableAsync<ProcedureDefinition>();
            await _connection.DropTableAsync<Specialization>();
            await _connection.DropTableAsync<User>();
        }
    }
}

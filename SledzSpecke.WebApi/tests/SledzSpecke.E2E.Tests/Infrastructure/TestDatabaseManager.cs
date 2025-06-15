using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Npgsql;
using Serilog;
using SledzSpecke.Core.Entities;
using SledzSpecke.Infrastructure.DAL;

namespace SledzSpecke.E2E.Tests.Infrastructure;

/// <summary>
/// Manages test database lifecycle with snapshot/restore capabilities
/// Implements proper isolation for E2E tests following Clean Architecture principles
/// </summary>
public sealed class TestDatabaseManager : IAsyncDisposable
{
    private readonly IConfiguration _configuration;
    private readonly ILogger _logger;
    private readonly string _testDatabaseName;
    private readonly string _masterConnectionString;
    private readonly string _testConnectionString;
    private bool _disposed;

    public TestDatabaseManager(IConfiguration configuration, ILogger logger)
    {
        _configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        
        _testDatabaseName = $"sledzspecke_e2e_test_{Guid.NewGuid():N}";
        
        var originalConnectionString = _configuration.GetConnectionString("DefaultConnection") 
            ?? throw new InvalidOperationException("DefaultConnection not found");
        
        var builder = new NpgsqlConnectionStringBuilder(originalConnectionString);
        var originalDatabase = builder.Database;
        
        // Master connection for database operations
        builder.Database = "postgres";
        _masterConnectionString = builder.ToString();
        
        // Test database connection
        builder.Database = _testDatabaseName;
        _testConnectionString = builder.ToString();
        
        _logger.Information("Test database manager initialized for database: {Database}", _testDatabaseName);
    }

    /// <summary>
    /// Creates a fresh test database with schema and seed data
    /// </summary>
    public async Task<string> InitializeAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.Information("Creating test database: {Database}", _testDatabaseName);
            
            // Create test database
            await using (var connection = new NpgsqlConnection(_masterConnectionString))
            {
                await connection.OpenAsync(cancellationToken);
                
                await using var command = connection.CreateCommand();
                command.CommandText = $"CREATE DATABASE \"{_testDatabaseName}\"";
                await command.ExecuteNonQueryAsync(cancellationToken);
            }
            
            // Apply migrations
            await ApplyMigrationsAsync(cancellationToken);
            
            // Seed test data
            await SeedTestDataAsync(cancellationToken);
            
            _logger.Information("Test database initialized successfully: {Database}", _testDatabaseName);
            
            return _testConnectionString;
        }
        catch (Exception ex)
        {
            _logger.Error(ex, "Failed to initialize test database");
            throw;
        }
    }

    /// <summary>
    /// Creates a snapshot of the current database state
    /// </summary>
    public async Task<TestDatabaseSnapshot> CreateSnapshotAsync(string name, CancellationToken cancellationToken = default)
    {
        var snapshotName = $"{_testDatabaseName}_snapshot_{name}_{DateTime.UtcNow:yyyyMMddHHmmss}";
        
        _logger.Information("Creating database snapshot: {Snapshot}", snapshotName);
        
        await using var connection = new NpgsqlConnection(_masterConnectionString);
        await connection.OpenAsync(cancellationToken);
        
        // Create snapshot by creating a template database
        await using var command = connection.CreateCommand();
        command.CommandText = $@"
            CREATE DATABASE ""{snapshotName}"" 
            WITH TEMPLATE ""{_testDatabaseName}""";
        await command.ExecuteNonQueryAsync(cancellationToken);
        
        return new TestDatabaseSnapshot(snapshotName, DateTime.UtcNow);
    }

    /// <summary>
    /// Restores database from a snapshot
    /// </summary>
    public async Task RestoreSnapshotAsync(TestDatabaseSnapshot snapshot, CancellationToken cancellationToken = default)
    {
        _logger.Information("Restoring database from snapshot: {Snapshot}", snapshot.Name);
        
        await using var connection = new NpgsqlConnection(_masterConnectionString);
        await connection.OpenAsync(cancellationToken);
        
        // Terminate active connections
        await TerminateConnectionsAsync(connection, _testDatabaseName, cancellationToken);
        
        // Drop current test database
        await using (var dropCommand = connection.CreateCommand())
        {
            dropCommand.CommandText = $"DROP DATABASE IF EXISTS \"{_testDatabaseName}\"";
            await dropCommand.ExecuteNonQueryAsync(cancellationToken);
        }
        
        // Restore from snapshot
        await using (var restoreCommand = connection.CreateCommand())
        {
            restoreCommand.CommandText = $@"
                CREATE DATABASE ""{_testDatabaseName}"" 
                WITH TEMPLATE ""{snapshot.Name}""";
            await restoreCommand.ExecuteNonQueryAsync(cancellationToken);
        }
        
        _logger.Information("Database restored from snapshot successfully");
    }

    /// <summary>
    /// Cleans all data from specified tables
    /// </summary>
    public async Task CleanTablesAsync(params string[] tableNames)
    {
        _logger.Information("Cleaning tables: {Tables}", string.Join(", ", tableNames));
        
        await using var connection = new NpgsqlConnection(_testConnectionString);
        await connection.OpenAsync();
        
        await using var transaction = await connection.BeginTransactionAsync();
        
        try
        {
            foreach (var table in tableNames)
            {
                await using var command = connection.CreateCommand();
                command.Transaction = transaction;
                command.CommandText = $"TRUNCATE TABLE \"{table}\" CASCADE";
                await command.ExecuteNonQueryAsync();
            }
            
            await transaction.CommitAsync();
            _logger.Information("Tables cleaned successfully");
        }
        catch (Exception ex)
        {
            await transaction.RollbackAsync();
            _logger.Error(ex, "Failed to clean tables");
            throw;
        }
    }

    /// <summary>
    /// Executes raw SQL for test setup
    /// </summary>
    public async Task ExecuteSqlAsync(string sql, CancellationToken cancellationToken = default)
    {
        await using var connection = new NpgsqlConnection(_testConnectionString);
        await connection.OpenAsync(cancellationToken);
        
        await using var command = connection.CreateCommand();
        command.CommandText = sql;
        await command.ExecuteNonQueryAsync(cancellationToken);
    }

    public async ValueTask DisposeAsync()
    {
        if (_disposed) return;
        
        try
        {
            _logger.Information("Disposing test database: {Database}", _testDatabaseName);
            
            await using var connection = new NpgsqlConnection(_masterConnectionString);
            await connection.OpenAsync();
            
            // Terminate active connections
            await TerminateConnectionsAsync(connection, _testDatabaseName);
            
            // Drop test database
            await using var command = connection.CreateCommand();
            command.CommandText = $"DROP DATABASE IF EXISTS \"{_testDatabaseName}\"";
            await command.ExecuteNonQueryAsync();
            
            _logger.Information("Test database disposed successfully");
        }
        catch (Exception ex)
        {
            _logger.Error(ex, "Failed to dispose test database");
        }
        finally
        {
            _disposed = true;
        }
    }

    private async Task ApplyMigrationsAsync(CancellationToken cancellationToken)
    {
        _logger.Information("Applying migrations to test database");
        
        var optionsBuilder = new DbContextOptionsBuilder<SledzSpeckeDbContext>();
        optionsBuilder.UseNpgsql(_testConnectionString);
        
        await using var context = new SledzSpeckeDbContext(optionsBuilder.Options);
        await context.Database.MigrateAsync(cancellationToken);
        
        _logger.Information("Migrations applied successfully");
    }

    private async Task SeedTestDataAsync(CancellationToken cancellationToken)
    {
        _logger.Information("Seeding test data");
        
        // Use raw SQL to seed test users since the User entity uses value objects
        await using var connection = new NpgsqlConnection(_testConnectionString);
        await connection.OpenAsync(cancellationToken);
        
        await using var command = connection.CreateCommand();
        command.CommandText = @"
            INSERT INTO ""Users"" 
            (""Email"", ""Username"", ""Password"", ""FullName"", ""SmkVersion"", ""SpecializationId"", ""RegistrationDate"", ""CreatedAt"", ""NotificationsEnabled"", ""EmailNotificationsEnabled"")
            VALUES 
            (@email1, @username1, @password, @fullName1, @smkVersion1, @specId, @regDate1, @regDate1, true, true),
            (@email2, @username2, @password, @fullName2, @smkVersion2, @specId, @regDate2, @regDate2, true, true)";
        
        command.Parameters.AddWithValue("@email1", "test.user@sledzspecke.pl");
        command.Parameters.AddWithValue("@username1", "test.user");
        command.Parameters.AddWithValue("@password", "$2a$10$8JqVb7SERQ2HCLKSMbfAkOSr1r2Ot.piVcAVZQQYjQxZX2x1B0XMO"); // Test123!
        command.Parameters.AddWithValue("@fullName1", "Jan Testowy");
        command.Parameters.AddWithValue("@smkVersion1", "new");
        command.Parameters.AddWithValue("@specId", 1); // Assuming specialization ID 1 exists
        command.Parameters.AddWithValue("@regDate1", DateTime.UtcNow.AddMonths(-6));
        
        command.Parameters.AddWithValue("@email2", "anna.kowalska@sledzspecke.pl");
        command.Parameters.AddWithValue("@username2", "anna.kowalska");
        command.Parameters.AddWithValue("@fullName2", "Anna Kowalska");
        command.Parameters.AddWithValue("@smkVersion2", "old");
        command.Parameters.AddWithValue("@regDate2", DateTime.UtcNow.AddYears(-2));
        
        await command.ExecuteNonQueryAsync(cancellationToken);
        
        _logger.Information("Test data seeded successfully with 2 test users");
    }

    private async Task TerminateConnectionsAsync(NpgsqlConnection connection, string databaseName, CancellationToken cancellationToken = default)
    {
        await using var command = connection.CreateCommand();
        command.CommandText = $@"
            SELECT pg_terminate_backend(pg_stat_activity.pid)
            FROM pg_stat_activity
            WHERE pg_stat_activity.datname = '{databaseName}'
              AND pid <> pg_backend_pid()";
        await command.ExecuteNonQueryAsync(cancellationToken);
    }
}

/// <summary>
/// Represents a database snapshot
/// </summary>
public record TestDatabaseSnapshot(string Name, DateTime CreatedAt);

/// <summary>
/// Extension methods for test database operations
/// </summary>
public static class TestDatabaseExtensions
{
    public static async Task<T> WithTestDatabaseAsync<T>(
        this IConfiguration configuration,
        ILogger logger,
        Func<string, Task<T>> testAction)
    {
        await using var dbManager = new TestDatabaseManager(configuration, logger);
        var connectionString = await dbManager.InitializeAsync();
        
        try
        {
            return await testAction(connectionString);
        }
        finally
        {
            // Cleanup happens in DisposeAsync
        }
    }
}
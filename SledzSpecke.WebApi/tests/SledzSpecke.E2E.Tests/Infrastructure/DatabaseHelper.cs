using Npgsql;
using BCrypt.Net;

namespace SledzSpecke.E2E.Tests.Infrastructure;

public static class DatabaseHelper
{
    public static async Task<string> CreateTestDatabaseAsync()
    {
        var dbName = $"sledzspecke_e2e_{Guid.NewGuid():N}";
        
        using var conn = new NpgsqlConnection("Host=localhost;Username=postgres;Password=postgres");
        await conn.OpenAsync();
        
        using var cmd = new NpgsqlCommand($"CREATE DATABASE {dbName}", conn);
        await cmd.ExecuteNonQueryAsync();
        
        return dbName;
    }
    
    public static async Task<string> GetTestConnectionStringAsync(string dbName)
    {
        return $"Host=localhost;Database={dbName};Username=postgres;Password=postgres";
    }
    
    public static async Task RunMigrationsAsync(string connectionString)
    {
        // This would run EF Core migrations
        // For now, we'll create basic schema
        using var conn = new NpgsqlConnection(connectionString);
        await conn.OpenAsync();
        
        var sql = @"
            CREATE TABLE IF NOT EXISTS ""Users"" (
                ""Id"" SERIAL PRIMARY KEY,
                ""Email"" VARCHAR(255) NOT NULL UNIQUE,
                ""PasswordHash"" VARCHAR(255) NOT NULL,
                ""FirstName"" VARCHAR(100) NOT NULL,
                ""LastName"" VARCHAR(100) NOT NULL,
                ""SmkVersion"" VARCHAR(10) NOT NULL,
                ""IsActive"" BOOLEAN NOT NULL DEFAULT true,
                ""CreatedAt"" TIMESTAMP NOT NULL DEFAULT CURRENT_TIMESTAMP
            );";
            
        using var cmd = new NpgsqlCommand(sql, conn);
        await cmd.ExecuteNonQueryAsync();
    }
    
    public static async Task SeedTestUserAsync(string connectionString)
    {
        var passwordHash = BCrypt.Net.BCrypt.HashPassword("Test123!");
        
        using var conn = new NpgsqlConnection(connectionString);
        await conn.OpenAsync();
        
        var sql = @"
            INSERT INTO ""Users"" 
            (""Email"", ""PasswordHash"", ""FirstName"", ""LastName"", ""SmkVersion"", ""IsActive"")
            VALUES 
            (@email, @hash, 'Test', 'User', 'new', true)
            ON CONFLICT (""Email"") DO NOTHING";
            
        using var cmd = new NpgsqlCommand(sql, conn);
        cmd.Parameters.AddWithValue("email", "test.user@sledzspecke.pl");
        cmd.Parameters.AddWithValue("hash", passwordHash);
        await cmd.ExecuteNonQueryAsync();
    }
    
    public static async Task CleanupDatabaseAsync(string dbName)
    {
        try
        {
            using var conn = new NpgsqlConnection("Host=localhost;Username=postgres;Password=postgres");
            await conn.OpenAsync();
            
            // Terminate connections
            var terminateSql = $@"
                SELECT pg_terminate_backend(pid)
                FROM pg_stat_activity
                WHERE datname = '{dbName}' AND pid <> pg_backend_pid()";
            using (var cmd = new NpgsqlCommand(terminateSql, conn))
            {
                await cmd.ExecuteNonQueryAsync();
            }
            
            // Drop database
            using var dropCmd = new NpgsqlCommand($"DROP DATABASE IF EXISTS {dbName}", conn);
            await dropCmd.ExecuteNonQueryAsync();
        }
        catch
        {
            // Ignore cleanup errors
        }
    }
}

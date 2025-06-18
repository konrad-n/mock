#!/bin/bash

# Quick E2E Test Fix Script for SledzSpecke
# This script helps automate common E2E test fixes

set -e

echo "🔧 SledzSpecke E2E Test Quick Fix Script"
echo "========================================"
echo ""

# Colors for output
GREEN='\033[0;32m'
YELLOW='\033[1;33m'
RED='\033[0;31m'
NC='\033[0m'

# Base directory
BASE_DIR="/home/ubuntu/projects/mock/SledzSpecke.WebApi"
E2E_DIR="$BASE_DIR/tests/SledzSpecke.E2E.Tests"

echo -e "${YELLOW}Step 1: Checking current directory...${NC}"
if [ ! -f "$BASE_DIR/SledzSpecke.WebApi.sln" ]; then
    echo -e "${RED}❌ Not in the correct directory. Please run from SledzSpecke.WebApi root.${NC}"
    exit 1
fi
echo -e "${GREEN}✅ Directory check passed${NC}"

echo ""
echo -e "${YELLOW}Step 2: Creating missing directories...${NC}"
mkdir -p "$E2E_DIR/Infrastructure"
mkdir -p "$E2E_DIR/Reports/Screenshots"
mkdir -p "$E2E_DIR/Reports/Videos"
mkdir -p "$E2E_DIR/Reports/Traces"
mkdir -p "$E2E_DIR/Reports/Logs"
echo -e "${GREEN}✅ Directories created${NC}"

echo ""
echo -e "${YELLOW}Step 3: Creating DatabaseHelper.cs...${NC}"
cat > "$E2E_DIR/Infrastructure/DatabaseHelper.cs" << 'EOF'
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
EOF
echo -e "${GREEN}✅ DatabaseHelper.cs created${NC}"

echo ""
echo -e "${YELLOW}Step 4: Creating TestDataModels.cs...${NC}"
cat > "$E2E_DIR/Infrastructure/TestDataModels.cs" << 'EOF'
namespace SledzSpecke.E2E.Tests.Infrastructure;

// DTOs for test data - not domain entities
public class MedicalShiftTestData
{
    public DateTime Date { get; set; }
    public int Hours { get; set; }
    public int Minutes { get; set; }
    public string Location { get; set; } = "";
    public int Year { get; set; }
    public string ShiftType { get; set; } = "";
}

public class ProcedureTestData
{
    public DateTime Date { get; set; }
    public string Name { get; set; } = "";
    public string Category { get; set; } = "";
    public string IcdCode { get; set; } = "";
    public string ExecutionType { get; set; } = "";
    public string Supervisor { get; set; } = "";
}

public class TestUser
{
    public string Email { get; set; } = "";
    public string Password { get; set; } = "";
    public string FirstName { get; set; } = "";
    public string LastName { get; set; } = "";
    public string SmkVersion { get; set; } = "";
    public string Specialization { get; set; } = "";
    public int Year { get; set; }
}

// Add ShiftData class that was missing
public class ShiftData
{
    public DateTime Date { get; set; }
    public int Hours { get; set; }
    public int Minutes { get; set; }
    public string Location { get; set; } = "";
    public int Year { get; set; }
    public string ShiftType { get; set; } = "";
}
EOF
echo -e "${GREEN}✅ TestDataModels.cs created${NC}"

echo ""
echo -e "${YELLOW}Step 5: Updating test configuration...${NC}"
cat > "$E2E_DIR/appsettings.json" << 'EOF'
{
  "E2ETests": {
    "BaseUrl": "https://sledzspecke.pl",
    "ApiUrl": "https://api.sledzspecke.pl",
    "LocalApiUrl": "http://localhost:5263",
    "Browser": "chromium",
    "Headless": true,
    "SlowMo": 0,
    "DefaultTimeout": 30000,
    "NavigationTimeout": 30000,
    "TraceEnabled": true,
    "VideoEnabled": true,
    "ScreenshotOnFailure": true,
    "TestUser": {
      "Email": "test.user@sledzspecke.pl",
      "Password": "Test123!"
    }
  },
  "Serilog": {
    "MinimumLevel": {
      "Default": "Information",
      "Override": {
        "Microsoft": "Warning",
        "System": "Warning"
      }
    },
    "WriteTo": [
      {
        "Name": "Console"
      },
      {
        "Name": "File",
        "Args": {
          "path": "Reports/Logs/e2e-tests-.log",
          "rollingInterval": "Day"
        }
      }
    ]
  }
}
EOF
echo -e "${GREEN}✅ Test configuration created${NC}"

echo ""
echo -e "${YELLOW}Step 6: Creating a simple working E2E test...${NC}"
cat > "$E2E_DIR/Scenarios/BasicSmokeTest.cs" << 'EOF'
using Microsoft.Playwright;
using Xunit;
using FluentAssertions;

namespace SledzSpecke.E2E.Tests.Scenarios;

public class BasicSmokeTest : IAsyncLifetime
{
    private IPlaywright _playwright = null!;
    private IBrowser _browser = null!;
    private IPage _page = null!;

    public async Task InitializeAsync()
    {
        _playwright = await Playwright.CreateAsync();
        _browser = await _playwright.Chromium.LaunchAsync(new BrowserTypeLaunchOptions
        {
            Headless = true
        });
        var context = await _browser.NewContextAsync();
        _page = await context.NewPageAsync();
    }

    public async Task DisposeAsync()
    {
        await _browser.DisposeAsync();
        _playwright.Dispose();
    }

    [Fact]
    public async Task HomePage_ShouldLoad_Successfully()
    {
        // Act
        var response = await _page.GotoAsync("https://sledzspecke.pl");
        
        // Assert
        response.Should().NotBeNull();
        response.Status.Should().Be(200);
    }
    
    [Fact]
    public async Task ApiHealth_ShouldReturn_OK()
    {
        // Act
        var response = await _page.APIRequest.GetAsync("https://api.sledzspecke.pl/api/health");
        
        // Assert
        response.Ok.Should().BeTrue();
        var json = await response.JsonAsync();
        json.Should().NotBeNull();
    }
}
EOF
echo -e "${GREEN}✅ Basic smoke test created${NC}"

echo ""
echo -e "${YELLOW}Step 7: Building E2E test project...${NC}"
cd "$E2E_DIR"
dotnet build --configuration Release

if [ $? -eq 0 ]; then
    echo -e "${GREEN}✅ E2E test project builds successfully!${NC}"
else
    echo -e "${RED}❌ Build failed. Check the errors above.${NC}"
    exit 1
fi

echo ""
echo -e "${GREEN}🎉 E2E Test Quick Fix Complete!${NC}"
echo ""
echo "Next steps:"
echo "1. Run the basic smoke test: dotnet test --filter BasicSmokeTest"
echo "2. Update remaining test scenarios to use TestDataModels"
echo "3. Fix page object implementations"
echo "4. Run full E2E test suite"
echo ""
echo "Pro tip: Start with one test scenario and get it working before fixing all tests."
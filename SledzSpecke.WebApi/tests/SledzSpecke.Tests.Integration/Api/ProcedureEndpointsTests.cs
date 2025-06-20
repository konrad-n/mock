using System.Net;
using System.Net.Http.Json;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using SledzSpecke.Api.Controllers;
using SledzSpecke.Application.Commands;
using SledzSpecke.Application.DTO;
using SledzSpecke.Core.Entities;
using SledzSpecke.Core.ValueObjects;
using SledzSpecke.Infrastructure.DAL;
using SledzSpecke.Tests.Integration.Common;
using Xunit;

namespace SledzSpecke.Tests.Integration.Api;

public class ProcedureEndpointsTests : IntegrationTestBase
{
    public ProcedureEndpointsTests(SledzSpeckeApiFactory factory) : base(factory)
    {
    }

    [Fact]
    public async Task GetModuleProcedures_WithValidModuleId_ShouldReturnProcedures()
    {
        // Arrange
        await AuthenticateAsync();
        var moduleId = await CreateTestModuleWithProcedures();

        // Act
        var response = await Client.GetAsync($"/api/procedures/modules/{moduleId}");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var content = await response.Content.ReadFromJsonAsync<ModuleProceduresDto>();
        content.Should().NotBeNull();
        content!.ModuleId.Should().Be(moduleId);
        content.Procedures.Should().NotBeEmpty();
    }

    [Fact]
    public async Task GetUserProcedures_ShouldReturnUserProcedures()
    {
        // Arrange
        await AuthenticateAsync();
        await CreateTestProcedureRealization();

        // Act
        var response = await Client.GetAsync("/api/procedures/user");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var content = await response.Content.ReadFromJsonAsync<UserProceduresDto>();
        content.Should().NotBeNull();
        content!.TotalRealizations.Should().BeGreaterThan(0);
    }

    [Fact]
    public async Task AddProcedureRealization_WithValidData_ShouldCreateRealization()
    {
        // Arrange
        await AuthenticateAsync();
        var requirementId = await CreateTestProcedureRequirement();
        
        var request = new AddProcedureRealizationRequest
        {
            RequirementId = requirementId,
            Date = DateTime.UtcNow.Date,
            Location = "Test Hospital",
            Role = ProcedureRole.Operator,
            Year = null
        };

        // Act
        var response = await Client.PostAsJsonAsync("/api/procedures/realizations", request);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var content = await response.Content.ReadFromJsonAsync<dynamic>();
        content!.message.ToString().Should().Contain("dodana");
    }

    [Fact]
    public async Task UpdateProcedureRealization_WithValidData_ShouldUpdateRealization()
    {
        // Arrange
        await AuthenticateAsync();
        var realizationId = await CreateTestProcedureRealization();
        
        var request = new UpdateProcedureRealizationRequest
        {
            Date = DateTime.UtcNow.Date.AddDays(-1),
            Location = "Updated Hospital",
            Role = ProcedureRole.Assistant
        };

        // Act
        var response = await Client.PutAsJsonAsync($"/api/procedures/realizations/{realizationId}", request);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var content = await response.Content.ReadFromJsonAsync<dynamic>();
        content!.message.ToString().Should().Contain("zaktualizowana");
    }

    [Fact]
    public async Task DeleteProcedureRealization_WithValidId_ShouldDeleteRealization()
    {
        // Arrange
        await AuthenticateAsync();
        var realizationId = await CreateTestProcedureRealization();

        // Act
        var response = await Client.DeleteAsync($"/api/procedures/realizations/{realizationId}");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var content = await response.Content.ReadFromJsonAsync<dynamic>();
        content!.message.ToString().Should().Contain("usunięta");
    }

    [Fact]
    public async Task AddProcedureRealization_WithInvalidRequirementId_ShouldReturnBadRequest()
    {
        // Arrange
        await AuthenticateAsync();
        
        var request = new AddProcedureRealizationRequest
        {
            RequirementId = 99999, // Non-existent
            Date = DateTime.UtcNow.Date,
            Location = "Test Hospital",
            Role = ProcedureRole.Operator
        };

        // Act
        var response = await Client.PostAsJsonAsync("/api/procedures/realizations", request);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task AddProcedureRealization_WithFutureDate_ShouldReturnBadRequest()
    {
        // Arrange
        await AuthenticateAsync();
        var requirementId = await CreateTestProcedureRequirement();
        
        var request = new AddProcedureRealizationRequest
        {
            RequirementId = requirementId,
            Date = DateTime.UtcNow.Date.AddDays(1),
            Location = "Test Hospital",
            Role = ProcedureRole.Operator
        };

        // Act
        var response = await Client.PostAsJsonAsync("/api/procedures/realizations", request);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    private int _userId;
    
    private async Task AuthenticateAsync()
    {
        // Create a test user and set authentication
        _userId = await CreateTestUserAsync();
        Client.DefaultRequestHeaders.Add("X-User-Id", _userId.ToString());
    }

    private async Task<int> CreateTestModuleWithProcedures()
    {
        // Create specialization and module
        var specialization = new Specialization(
            new SpecializationId(1),
            new UserId(_userId),
            "Test Specialization",
            "test",
            SmkVersion.New,
            "standard",
            DateTime.UtcNow,
            DateTime.UtcNow.AddYears(5),
            2024,
            "modular",
            5
        );
        
        var module = new Module(
            new ModuleId(1),
            "Test Module",
            ModuleType.Basic,
            specialization.Id,
            1,
            0,
            0,
            0,
            0,
            5,
            10,
            DateTime.UtcNow,
            null,
            null,
            null,
            false,
            SmkVersion.New
        );
        
        DbContext.Specializations.Add(specialization);
        DbContext.Modules.Add(module);
        
        // Create procedure requirement
        var requirement = new ProcedureRequirement(
            module.Id,
            "TEST001",
            "Test Procedure",
            1,
            0,
            SmkVersion.New,
            null
        );
        
        DbContext.Set<ProcedureRequirement>().Add(requirement);
        await DbContext.SaveChangesAsync();
        
        return module.Id.Value;
    }

    private async Task<int> CreateTestProcedureRequirement()
    {
        var module = await DbContext.Modules.FirstAsync();
        var requirement = new ProcedureRequirement(
            module.Id,
            $"TEST{Guid.NewGuid().ToString().Substring(0, 6)}",
            "Test Procedure",
            1,
            0,
            SmkVersion.New,
            null
        );
        
        DbContext.Set<ProcedureRequirement>().Add(requirement);
        await DbContext.SaveChangesAsync();
        
        return requirement.Id.Value;
    }

    private async Task<int> CreateTestProcedureRealization()
    {
        var requirementId = await CreateTestProcedureRequirement();
        
        var realization = ProcedureRealization.Create(
            new ProcedureRequirementId(requirementId),
            new UserId(_userId),
            DateTime.UtcNow.Date,
            "Test Location",
            ProcedureRole.Operator,
            null
        );
        
        if (realization.IsSuccess)
        {
            DbContext.Set<ProcedureRealization>().Add(realization.Value);
            await DbContext.SaveChangesAsync();
            return realization.Value.Id.Value;
        }
        
        throw new InvalidOperationException("Failed to create test realization");
    }
}
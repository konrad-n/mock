using FluentAssertions;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection;
using SledzSpecke.Application.Commands;
using SledzSpecke.Core.Entities;
using SledzSpecke.Core.Repositories;
using SledzSpecke.Core.ValueObjects;
using SledzSpecke.Tests.Integration.Common;
using System;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using Xunit;

namespace SledzSpecke.Tests.Integration.Controllers;

public class ProceduresControllerTests : IntegrationTestBase
{
    private readonly HttpClient _authenticatedClient;
    private readonly ISpecializationRepository _specializationRepository;
    private readonly IInternshipRepository _internshipRepository;
    private readonly IModuleRepository _moduleRepository;
    private readonly IProcedureRepository _procedureRepository;

    public ProceduresControllerTests(SledzSpeckeApiFactory factory) : base(factory)
    {
        _authenticatedClient = Factory.WithWebHostBuilder(builder =>
        {
            builder.ConfigureTestServices(services =>
            {
                services.AddAuthentication(TestAuthHandler.AuthenticationScheme)
                    .AddScheme<AuthenticationSchemeOptions, TestAuthHandler>(
                        TestAuthHandler.AuthenticationScheme, options => { });
            });
        }).CreateClient();
        
        _authenticatedClient.DefaultRequestHeaders.Authorization = 
            new System.Net.Http.Headers.AuthenticationHeaderValue(TestAuthHandler.AuthenticationScheme);
            
        _specializationRepository = Scope.ServiceProvider.GetRequiredService<ISpecializationRepository>();
        _internshipRepository = Scope.ServiceProvider.GetRequiredService<IInternshipRepository>();
        _moduleRepository = Scope.ServiceProvider.GetRequiredService<IModuleRepository>();
        _procedureRepository = Scope.ServiceProvider.GetRequiredService<IProcedureRepository>();
    }

    [Fact]
    public async Task AddProcedure_WithOldSmk_ShouldCreateProcedure()
    {
        // Arrange
        var specialization = TestDataFactory.CreateSpecialization(SmkVersion.Old);
        await _specializationRepository.AddAsync(specialization);
        
        var internship = TestDataFactory.CreateInternship(
            specialization.Id.Value,
            DateTime.Today.AddDays(-30),
            DateTime.Today.AddDays(30));
        await _internshipRepository.AddAsync(internship);
        await DbContext.SaveChangesAsync();

        var command = new AddProcedure(
            InternshipId: internship.Id,
            Date: DateTime.Today,
            Year: 3,
            Code: "PROC001",
            Name: "Appendectomy",
            Location: "Hospital A",
            Status: "Pending",
            ExecutionType: "Primary",
            SupervisorName: "Dr. Smith",
            SupervisorPwz: "1234567",
            PerformingPerson: "Dr. Smith",
            PatientInitials: "JD",
            PatientGender: 'M',
            ProcedureGroup: "Surgery",
            AssistantData: "Nurse Johnson",
            ProcedureRequirementId: 1);

        // Act
        var response = await _authenticatedClient.PostAsJsonAsync("/procedures", command);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        
        var procedureId = await response.Content.ReadFromJsonAsync<int>();
        procedureId.Should().BeGreaterThan(0);
        
        var procedure = await _procedureRepository.GetByIdAsync(new ProcedureId(procedureId));
        procedure.Should().NotBeNull();
        procedure.Should().BeOfType<ProcedureOldSmk>();
        
        var oldSmkProcedure = (ProcedureOldSmk)procedure!;
        oldSmkProcedure.Code.Should().Be(command.Code);
        oldSmkProcedure.Year.Should().Be(command.Year);
        oldSmkProcedure.Location.Should().Be(command.Location);
    }

    [Fact]
    public async Task AddProcedure_WithNewSmk_ShouldCreateProcedure()
    {
        // Arrange
        var specialization = TestDataFactory.CreateSpecialization(SmkVersion.New);
        await _specializationRepository.AddAsync(specialization);
        
        var module = TestDataFactory.CreateModule(specialization.Id.Value);
        await _moduleRepository.AddAsync(module);
        
        var internship = TestDataFactory.CreateInternship(
            specialization.Id.Value,
            DateTime.Today.AddDays(-30),
            DateTime.Today.AddDays(30),
            module.Id.Value);
        await _internshipRepository.AddAsync(internship);
        await DbContext.SaveChangesAsync();

        var command = new AddProcedure(
            InternshipId: internship.Id,
            Date: DateTime.Today,
            Year: 0, // Not used in new SMK
            Code: "PROC002",
            Name: "Cholecystectomy",
            Location: "Hospital B",
            Status: "Pending",
            ExecutionType: "Primary",
            SupervisorName: "Dr. Senior",
            SupervisorPwz: "7654321",
            PerformingPerson: "Dr. Jones",
            PatientInitials: "AB",
            PatientGender: 'F',
            ProcedureRequirementId: 5,
            ModuleId: module.Id.Value,
            ProcedureName: "Complex Procedure",
            Supervisor: "Dr. Senior");

        // Act
        var response = await _authenticatedClient.PostAsJsonAsync("/procedures", command);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        
        var procedureId = await response.Content.ReadFromJsonAsync<int>();
        procedureId.Should().BeGreaterThan(0);
        
        var procedure = await _procedureRepository.GetByIdAsync(new ProcedureId(procedureId));
        procedure.Should().NotBeNull();
        procedure.Should().BeOfType<ProcedureNewSmk>();
        
        var newSmkProcedure = (ProcedureNewSmk)procedure!;
        newSmkProcedure.Code.Should().Be(command.Code);
        newSmkProcedure.ModuleId.Value.Should().Be(module.Id.Value);
        newSmkProcedure.ProcedureName.Should().Be(command.ProcedureName);
        newSmkProcedure.Supervisor.Should().Be(command.Supervisor);
    }

    [Fact]
    public async Task AddProcedure_WithInvalidInternshipId_ShouldReturnBadRequest()
    {
        // Arrange
        var command = new AddProcedure(
            InternshipId: 999999,
            Date: DateTime.Today,
            Year: 3,
            Code: "PROC001",
            Name: "Test Procedure",
            Location: "Hospital A",
            Status: "Pending",
            ExecutionType: "CodeA",
            SupervisorName: "Dr. Smith");

        // Act
        var response = await _authenticatedClient.PostAsJsonAsync("/procedures", command);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        
        var content = await response.Content.ReadAsStringAsync();
        content.Should().Contain("Internship with ID 999999 not found");
    }

    [Fact]
    public async Task AddProcedure_WithDateOutsideInternshipPeriod_ShouldReturnBadRequest()
    {
        // Arrange
        var specialization = TestDataFactory.CreateSpecialization(SmkVersion.Old);
        await _specializationRepository.AddAsync(specialization);
        
        var internship = TestDataFactory.CreateInternship(
            specialization.Id.Value,
            DateTime.Today.AddDays(-30),
            DateTime.Today.AddDays(-10)); // Ended 10 days ago
        await _internshipRepository.AddAsync(internship);
        await DbContext.SaveChangesAsync();

        var command = new AddProcedure(
            InternshipId: internship.Id,
            Date: DateTime.Today, // Today is after internship end
            Year: 3,
            Code: "PROC001",
            Name: "Test Procedure",
            Location: "Hospital A",
            Status: "Pending",
            ExecutionType: "Primary",
            SupervisorName: "Dr. Test",
            SupervisorPwz: null,
            PerformingPerson: null,
            PatientInitials: null,
            PatientGender: null);

        // Act
        var response = await _authenticatedClient.PostAsJsonAsync("/procedures", command);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        
        var content = await response.Content.ReadAsStringAsync();
        content.Should().Contain("Procedure date must be within the internship period");
    }

    [Fact]
    public async Task GetProcedure_WithExistingId_ShouldReturnProcedure()
    {
        // Arrange
        var specialization = TestDataFactory.CreateSpecialization(SmkVersion.Old);
        await _specializationRepository.AddAsync(specialization);
        
        var internship = TestDataFactory.CreateInternship(specialization.Id.Value);
        await _internshipRepository.AddAsync(internship);
        
        var procedure = TestDataFactory.CreateProcedure(
            internship.Id.Value,
            SmkVersion.Old,
            DateTime.Today,
            ProcedureStatus.Pending,
            null,
            3);
        await _procedureRepository.AddAsync(procedure);
        await DbContext.SaveChangesAsync();

        // Act
        var response = await _authenticatedClient.GetAsync($"/procedures/{procedure.Id.Value}");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        
        var content = await response.Content.ReadAsStringAsync();
        content.Should().Contain(procedure.Code);
        content.Should().Contain(procedure.Location);
    }

    [Fact]
    public async Task GetProcedure_WithNonExistentId_ShouldReturnNotFound()
    {
        // Act
        var response = await _authenticatedClient.GetAsync("/procedures/999999");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task GetProceduresByInternship_ShouldReturnProcedures()
    {
        // Arrange
        var specialization = TestDataFactory.CreateSpecialization(SmkVersion.Old);
        await _specializationRepository.AddAsync(specialization);
        
        var internship = TestDataFactory.CreateInternship(specialization.Id.Value);
        await _internshipRepository.AddAsync(internship);
        
        // Create multiple procedures
        for (int i = 0; i < 3; i++)
        {
            var procedure = TestDataFactory.CreateProcedure(
                internship.Id.Value,
                SmkVersion.Old,
                DateTime.Today.AddDays(-i),
                ProcedureStatus.Pending,
                null,
                3);
            await _procedureRepository.AddAsync(procedure);
        }
        
        await DbContext.SaveChangesAsync();

        // Act
        var response = await _authenticatedClient.GetAsync($"/procedures?internshipId={internship.Id.Value}");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        
        var procedures = await response.Content.ReadFromJsonAsync<dynamic[]>();
        procedures.Should().NotBeNull();
        procedures!.Length.Should().Be(3);
    }

    [Fact]
    public async Task UpdateProcedure_WithValidData_ShouldUpdateProcedure()
    {
        // Arrange
        var specialization = TestDataFactory.CreateSpecialization(SmkVersion.Old);
        await _specializationRepository.AddAsync(specialization);
        
        var internship = TestDataFactory.CreateInternship(specialization.Id.Value);
        await _internshipRepository.AddAsync(internship);
        
        var procedure = TestDataFactory.CreateProcedure(
            internship.Id.Value,
            SmkVersion.Old,
            DateTime.Today,
            ProcedureStatus.Pending,
            null,
            3);
        await _procedureRepository.AddAsync(procedure);
        await DbContext.SaveChangesAsync();

        var command = new UpdateProcedure(
            ProcedureId: procedure.Id.Value,
            Date: DateTime.Today.AddDays(1),
            Code: "UPDATED001",
            Location: "Updated Hospital",
            Status: "Completed",
            ExecutionType: "CodeB",
            PerformingPerson: "Dr. Updated",
            PatientGender: 'M',
            ProcedureGroup: "Updated Group",
            AssistantData: "Updated Assistant",
            ProcedureRequirementId: 10);

        // Act
        var response = await _authenticatedClient.PutAsJsonAsync($"/procedures/{procedure.Id.Value}", command);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        
        var updatedProcedure = await _procedureRepository.GetByIdAsync(new ProcedureId(procedure.Id.Value));
        updatedProcedure.Should().NotBeNull();
        updatedProcedure!.Code.Should().Be(command.Code);
        updatedProcedure.Location.Should().Be(command.Location);
        updatedProcedure.Status.Should().Be(ProcedureStatus.Completed);
    }

    [Fact]
    public async Task DeleteProcedure_WithExistingId_ShouldDeleteProcedure()
    {
        // Arrange
        var specialization = TestDataFactory.CreateSpecialization(SmkVersion.Old);
        await _specializationRepository.AddAsync(specialization);
        
        var internship = TestDataFactory.CreateInternship(specialization.Id.Value);
        await _internshipRepository.AddAsync(internship);
        
        var procedure = TestDataFactory.CreateProcedure(
            internship.Id.Value,
            SmkVersion.Old,
            DateTime.Today,
            ProcedureStatus.Pending,
            null,
            3);
        await _procedureRepository.AddAsync(procedure);
        await DbContext.SaveChangesAsync();

        // Act
        var response = await _authenticatedClient.DeleteAsync($"/procedures/{procedure.Id.Value}");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        
        // Verify deletion
        var deletedProcedure = await _procedureRepository.GetByIdAsync(procedure.Id.Value);
        deletedProcedure.Should().BeNull();
    }
}
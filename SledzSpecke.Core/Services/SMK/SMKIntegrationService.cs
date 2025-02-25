using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using SledzSpecke.Core.Interfaces.Services;
using SledzSpecke.Core.Models.Domain;

namespace SledzSpecke.Core.Services.SMK
{
    public class SMKIntegrationService : ISMKIntegrationService
    {
        private readonly HttpClient _httpClient;
        private readonly ILogger<SMKIntegrationService> _logger;
        private readonly IUserService _userService;
        private readonly ISpecializationRepository _specializationRepository;
        
        // Konfiguracja
        private readonly string _baseUrl; // URL do API SMK
        private string _authToken; // Token uwierzytelniający
        
        public SMKIntegrationService(
            HttpClient httpClient,
            ILogger<SMKIntegrationService> logger,
            IUserService userService,
            ISpecializationRepository specializationRepository,
            SMKConfiguration configuration)
        {
            _httpClient = httpClient;
            _logger = logger;
            _userService = userService;
            _specializationRepository = specializationRepository;
            
            _baseUrl = configuration.BaseUrl;
            _httpClient.BaseAddress = new Uri(_baseUrl);
        }
        
        public async Task<bool> AuthenticateWithSMKAsync(string username, string password)
        {
            try
            {
                var authRequest = new
                {
                    Username = username,
                    Password = password
                };
                
                var content = new StringContent(
                    JsonSerializer.Serialize(authRequest),
                    Encoding.UTF8,
                    "application/json");
                
                var response = await _httpClient.PostAsync("/api/auth", content);
                
                if (response.IsSuccessStatusCode)
                {
                    var authResult = await JsonSerializer.DeserializeAsync<SMKAuthResult>(
                        await response.Content.ReadAsStreamAsync());
                    
                    _authToken = authResult.Token;
                    _httpClient.DefaultRequestHeaders.Authorization = 
                        new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", _authToken);
                    
                    return true;
                }
                
                return false;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error authenticating with SMK");
                return false;
            }
        }
        
        public async Task<List<ProcedureExecution>> SynchronizeProceduresAsync(int userId)
        {
            try
            {
                // Sprawdź, czy użytkownik jest uwierzytelniony
                if (string.IsNullOrEmpty(_authToken))
                {
                    throw new InvalidOperationException("User is not authenticated with SMK");
                }
                
                // Pobierz procedury z SMK
                var response = await _httpClient.GetAsync($"/api/procedures/user/{userId}");
                
                if (response.IsSuccessStatusCode)
                {
                    var smkProcedures = await JsonSerializer.DeserializeAsync<List<SMKProcedure>>(
                        await response.Content.ReadAsStreamAsync());
                    
                    // Konwertuj procedury SMK na procedury aplikacji
                    var procedures = new List<ProcedureExecution>();
                    foreach (var smkProcedure in smkProcedures)
                    {
                        procedures.Add(new ProcedureExecution
                        {
                            UserId = userId,
                            Name = smkProcedure.Name,
                            ExecutionDate = smkProcedure.ExecutionDate,
                            Type = smkProcedure.Type == "Execution" ? 
                                ProcedureType.Execution : ProcedureType.Assistance,
                            Location = smkProcedure.Location,
                            SupervisorId = smkProcedure.SupervisorId,
                            Notes = smkProcedure.Notes,
                            IsSimulation = smkProcedure.IsSimulation,
                            Category = smkProcedure.Category,
                            Stage = smkProcedure.Stage
                        });
                    }
                    
                    return procedures;
                }
                
                return new List<ProcedureExecution>();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error synchronizing procedures with SMK");
                throw;
            }
        }
        
        public async Task<bool> ExportToSMKAsync(int userId, DateTime startDate, DateTime endDate)
        {
            try
            {
                // Implementacja eksportu danych do SMK
                // Tutaj można dodać kod do zbierania danych i wysyłania ich do SMK
                
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error exporting data to SMK");
                return false;
            }
        }
        
        public async Task<List<SMKSpecialization>> GetAvailableSpecializationsFromSMKAsync()
        {
            try
            {
                var response = await _httpClient.GetAsync("/api/specializations");
                
                if (response.IsSuccessStatusCode)
                {
                    return await JsonSerializer.DeserializeAsync<List<SMKSpecialization>>(
                        await response.Content.ReadAsStreamAsync());
                }
                
                return new List<SMKSpecialization>();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting specializations from SMK");
                return new List<SMKSpecialization>();
            }
        }
        
        // Klasa pomocnicza do deserializacji odpowiedzi uwierzytelniania
        private class SMKAuthResult
        {
            public string Token { get; set; }
            public DateTime Expiration { get; set; }
        }
        
        // Klasa pomocnicza do deserializacji procedur SMK
        private class SMKProcedure
        {
            public string Name { get; set; }
            public DateTime ExecutionDate { get; set; }
            public string Type { get; set; } // "Execution" lub "Assistance"
            public string Location { get; set; }
            public int? SupervisorId { get; set; }
            public string Notes { get; set; }
            public bool IsSimulation { get; set; }
            public string Category { get; set; }
            public string Stage { get; set; }
        }
    }
    
    // Klasa konfiguracyjna
    public class SMKConfiguration
    {
        public string BaseUrl { get; set; }
        public string ApiKey { get; set; }
    }
}

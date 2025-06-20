using Microsoft.Extensions.Logging;
using SledzSpecke.Application.Abstractions;
using SledzSpecke.Application.DTO;
using SledzSpecke.Core.Abstractions;
using SledzSpecke.Core.Repositories;
using SledzSpecke.Core.ValueObjects;

namespace SledzSpecke.Application.Queries.Handlers;

internal sealed class GetUserProceduresHandler : IQueryHandler<GetUserProceduresQuery, UserProceduresDto>
{
    private readonly IUserRepository _userRepository;
    private readonly IModuleRepository _moduleRepository;
    private readonly ISpecializationRepository _specializationRepository;
    private readonly IProcedureRequirementRepository _procedureRequirementRepository;
    private readonly IProcedureRealizationRepository _procedureRealizationRepository;
    private readonly ILogger<GetUserProceduresHandler> _logger;

    public GetUserProceduresHandler(
        IUserRepository userRepository,
        IModuleRepository moduleRepository,
        ISpecializationRepository specializationRepository,
        IProcedureRequirementRepository procedureRequirementRepository,
        IProcedureRealizationRepository procedureRealizationRepository,
        ILogger<GetUserProceduresHandler> logger)
    {
        _userRepository = userRepository;
        _moduleRepository = moduleRepository;
        _specializationRepository = specializationRepository;
        _procedureRequirementRepository = procedureRequirementRepository;
        _procedureRealizationRepository = procedureRealizationRepository;
        _logger = logger;
    }

    public async Task<UserProceduresDto> HandleAsync(GetUserProceduresQuery query)
    {
        // Get user
        var user = await _userRepository.GetByIdAsync(query.UserId);
        if (user is null)
        {
            _logger.LogWarning("User {UserId} not found", query.UserId);
            throw new InvalidOperationException($"Użytkownik o ID {query.UserId} nie został znaleziony");
        }

            // Get modules based on specialization filter
            var modules = query.SpecializationId.HasValue
                ? await _moduleRepository.GetBySpecializationIdAsync(new SpecializationId(query.SpecializationId.Value))
                : await _moduleRepository.GetAllAsync();

            // Get all user realizations
            var allUserRealizations = await _procedureRealizationRepository.GetByUserIdAsync(query.UserId);
            
            // Group realizations by module
            var realizationsByModule = new Dictionary<ModuleId, List<Core.Entities.ProcedureRealization>>();
            foreach (var realization in allUserRealizations)
            {
                if (realization.Requirement != null && realization.Requirement.ModuleId != null)
                {
                    if (!realizationsByModule.ContainsKey(realization.Requirement.ModuleId))
                    {
                        realizationsByModule[realization.Requirement.ModuleId] = new List<Core.Entities.ProcedureRealization>();
                    }
                    realizationsByModule[realization.Requirement.ModuleId].Add(realization);
                }
            }

            // Build module DTOs
            var moduleDtos = new List<ModuleProceduresDto>();
            foreach (var module in modules)
            {
                // Get procedure requirements for this module
                var requirements = await _procedureRequirementRepository.GetByModuleIdAsync(module.Id);
                
                // Get realizations for this module
                var moduleRealizations = realizationsByModule.ContainsKey(module.Id)
                    ? realizationsByModule[module.Id]
                    : new List<Core.Entities.ProcedureRealization>();

                // Group realizations by requirement
                var realizationsByRequirement = moduleRealizations.GroupBy(r => r.RequirementId)
                    .ToDictionary(g => g.Key, g => g.ToList());

                // Build procedure DTOs
                var procedureDtos = new List<ProcedureDetailsDto>();
                foreach (var requirement in requirements)
                {
                    var realizationsForReq = realizationsByRequirement.ContainsKey(requirement.Id)
                        ? realizationsByRequirement[requirement.Id]
                        : new List<Core.Entities.ProcedureRealization>();

                    var completedAsOperator = realizationsForReq.Count(r => r.Role == ProcedureRole.Operator);
                    var completedAsAssistant = realizationsForReq.Count(r => r.Role == ProcedureRole.Assistant);

                    var procedureDto = new ProcedureDetailsDto
                    {
                        RequirementId = requirement.Id.Value,
                        Code = requirement.Code,
                        Name = requirement.Name,
                        RequiredAsOperator = requirement.RequiredAsOperator,
                        RequiredAsAssistant = requirement.RequiredAsAssistant,
                        CompletedAsOperator = completedAsOperator,
                        CompletedAsAssistant = completedAsAssistant,
                        IsCompleted = completedAsOperator >= requirement.RequiredAsOperator &&
                                      completedAsAssistant >= requirement.RequiredAsAssistant,
                        Realizations = realizationsForReq.Select(r => new ProcedureRealizationDto
                        {
                            Id = r.Id.Value,
                            Date = r.Date,
                            Location = r.Location,
                            Role = r.Role.ToString(),
                            Year = r.Year
                        }).ToList()
                    };

                    procedureDtos.Add(procedureDto);
                }

                // Calculate module summary
                var moduleSummary = new ModuleProcedureSummaryDto
                {
                    TotalProcedures = procedureDtos.Count,
                    CompletedProcedures = procedureDtos.Count(p => p.IsCompleted),
                    TotalRequiredAsOperator = procedureDtos.Sum(p => p.RequiredAsOperator),
                    TotalRequiredAsAssistant = procedureDtos.Sum(p => p.RequiredAsAssistant),
                    TotalCompletedAsOperator = procedureDtos.Sum(p => p.CompletedAsOperator),
                    TotalCompletedAsAssistant = procedureDtos.Sum(p => p.CompletedAsAssistant),
                    CompletionPercentage = procedureDtos.Count > 0
                        ? (decimal)procedureDtos.Count(p => p.IsCompleted) / procedureDtos.Count * 100
                        : 0
                };

                var moduleDto = new ModuleProceduresDto
                {
                    ModuleId = module.Id.Value,
                    ModuleName = module.Name,
                    ModuleType = module.Type.ToString(),
                    Procedures = procedureDtos,
                    Summary = moduleSummary
                };

                moduleDtos.Add(moduleDto);
            }

            // Calculate overall summary
            var overallSummary = new UserProcedureSummaryDto
            {
                TotalModules = moduleDtos.Count,
                TotalProcedures = moduleDtos.Sum(m => m.Summary.TotalProcedures),
                CompletedProcedures = moduleDtos.Sum(m => m.Summary.CompletedProcedures),
                TotalRealizationsAsOperator = allUserRealizations.Count(r => r.Role == ProcedureRole.Operator),
                TotalRealizationsAsAssistant = allUserRealizations.Count(r => r.Role == ProcedureRole.Assistant),
                OverallCompletionPercentage = moduleDtos.Any() && moduleDtos.Sum(m => m.Summary.TotalProcedures) > 0
                    ? (decimal)moduleDtos.Sum(m => m.Summary.CompletedProcedures) / moduleDtos.Sum(m => m.Summary.TotalProcedures) * 100
                    : 0,
                LastRealizationDate = allUserRealizations.Any() 
                    ? allUserRealizations.Max(r => r.Date) 
                    : null
            };

            var result = new UserProceduresDto
            {
                UserId = user.Id.Value,
                UserName = $"{user.FirstName} {user.LastName}",
                Modules = moduleDtos,
                OverallSummary = overallSummary
            };

            return result;
    }
}
using Microsoft.Extensions.Logging;
using SledzSpecke.Application.Abstractions;
using SledzSpecke.Application.DTO;
using SledzSpecke.Core.Abstractions;
using SledzSpecke.Core.Repositories;
using SledzSpecke.Core.ValueObjects;

namespace SledzSpecke.Application.Queries.Handlers;

internal sealed class GetModuleProceduresHandler : IQueryHandler<GetModuleProceduresQuery, ModuleProceduresDto>
{
    private readonly IProcedureRequirementRepository _procedureRequirementRepository;
    private readonly IProcedureRealizationRepository _procedureRealizationRepository;
    private readonly IModuleRepository _moduleRepository;
    private readonly ILogger<GetModuleProceduresHandler> _logger;

    public GetModuleProceduresHandler(
        IProcedureRequirementRepository procedureRequirementRepository,
        IProcedureRealizationRepository procedureRealizationRepository,
        IModuleRepository moduleRepository,
        ILogger<GetModuleProceduresHandler> logger)
    {
        _procedureRequirementRepository = procedureRequirementRepository;
        _procedureRealizationRepository = procedureRealizationRepository;
        _moduleRepository = moduleRepository;
        _logger = logger;
    }

    public async Task<ModuleProceduresDto> HandleAsync(GetModuleProceduresQuery query)
    {
        // Get module
        var module = await _moduleRepository.GetByIdAsync(query.ModuleId);
        if (module is null)
        {
            _logger.LogWarning("Module {ModuleId} not found", query.ModuleId);
            throw new InvalidOperationException($"Moduł o ID {query.ModuleId} nie został znaleziony");
        }

            // Get all procedure requirements for the module
            var requirements = await _procedureRequirementRepository.GetByModuleIdAsync(query.ModuleId);

            // Get all realizations for the user and module
            var userRealizations = await _procedureRealizationRepository.GetByUserAndModuleAsync(query.UserId, query.ModuleId);

            // Group realizations by requirement
            var realizationsByRequirement = userRealizations.GroupBy(r => r.RequirementId)
                .ToDictionary(g => g.Key, g => g.ToList());

            // Build DTOs
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

            // Calculate summary
            var summary = new ModuleProcedureSummaryDto
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

            var result = new ModuleProceduresDto
            {
                ModuleId = module.Id.Value,
                ModuleName = module.Name,
                ModuleType = module.Type.ToString(),
                Procedures = procedureDtos,
                Summary = summary
            };

            return result;
    }
}
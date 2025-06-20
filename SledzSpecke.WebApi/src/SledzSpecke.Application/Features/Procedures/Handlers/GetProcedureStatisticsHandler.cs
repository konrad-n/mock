using SledzSpecke.Application.Abstractions;
using SledzSpecke.Application.DTO;
using SledzSpecke.Application.Queries;
using SledzSpecke.Core.Repositories;
using SledzSpecke.Core.ValueObjects;
using SledzSpecke.Core.Entities;
using SledzSpecke.Core.Enums;

namespace SledzSpecke.Application.Features.Procedures.Handlers;

public class GetProcedureStatisticsHandler : IQueryHandler<GetProcedureStatistics, ProcedureSummaryDto>
{
    private readonly IProcedureRepository _procedureRepository;
    private readonly IUserRepository _userRepository;
    private readonly IModuleRepository _moduleRepository;
    private readonly IUserContextService _userContextService;
    private readonly ISpecializationTemplateService _templateService;
    private readonly ISpecializationRepository _specializationRepository;

    public GetProcedureStatisticsHandler(
        IProcedureRepository procedureRepository,
        IUserRepository userRepository,
        IModuleRepository moduleRepository,
        IUserContextService userContextService,
        ISpecializationTemplateService templateService,
        ISpecializationRepository specializationRepository)
    {
        _procedureRepository = procedureRepository;
        _userRepository = userRepository;
        _moduleRepository = moduleRepository;
        _userContextService = userContextService;
        _templateService = templateService;
        _specializationRepository = specializationRepository;
    }

    public async Task<ProcedureSummaryDto> HandleAsync(GetProcedureStatistics query)
    {
        var userId = _userContextService.GetUserId();
        var user = await _userRepository.GetByIdAsync(new UserId(userId));
        if (user is null)
        {
            throw new UnauthorizedAccessException("User not found");
        }

        var summary = new ProcedureSummaryDto();

        // Get required counts from JSON templates
        if (query.ModuleId.HasValue)
        {
            var module = await _moduleRepository.GetByIdAsync(query.ModuleId.Value);
            if (module is not null)
            {
                // Get specialization to access template
                var specialization = await _specializationRepository.GetByIdAsync(module.SpecializationId);
                if (specialization != null && query.ProcedureRequirementId.HasValue)
                {
                    var procedureTemplate = await _templateService.GetProcedureTemplateAsync(
                        specialization.ProgramCode,
                        new Core.ValueObjects.SmkVersion(specialization.SmkVersion.ToString()),
                        query.ProcedureRequirementId.Value);

                    if (procedureTemplate != null)
                    {
                        summary.RequiredCountA = procedureTemplate.RequiredCountA;
                        summary.RequiredCountB = procedureTemplate.RequiredCountB;
                    }
                }
            }
        }

        // TODO: User-Specialization relationship needs to be redesigned
        // // Get completed counts from database
        // var procedures = query.ModuleId.HasValue
        //     ? await GetProceduresForModule(user.SpecializationId, query.ModuleId.Value, query.ProcedureRequirementId)
        //     : await _procedureRepository.GetByUserAsync(new UserId(userId));
        var procedures = await _procedureRepository.GetByUserAsync(new UserId(userId));

        foreach (var procedure in procedures)
        {
            // Check if procedure is Code A (operator) or Code B (assistant)
            if (procedure.IsCodeA)
            {
                summary.CompletedCountA++;
                if (procedure.Status == ProcedureStatus.Approved || procedure.SyncStatus == Core.ValueObjects.SyncStatus.Synced)
                {
                    summary.ApprovedCountA++;
                }
            }
            else if (procedure.IsCodeB)
            {
                summary.CompletedCountB++;
                if (procedure.Status == ProcedureStatus.Approved || procedure.SyncStatus == Core.ValueObjects.SyncStatus.Synced)
                {
                    summary.ApprovedCountB++;
                }
            }
        }

        return summary;
    }

    private async Task<IEnumerable<ProcedureBase>> GetProceduresForModule(
        SpecializationId specializationId,
        int moduleId,
        int? procedureRequirementId)
    {
        var module = await _moduleRepository.GetByIdAsync(moduleId);
        if (module is null)
        {
            return Enumerable.Empty<ProcedureBase>();
        }

        // For Old SMK, we need to filter by year ranges based on module type
        if (module.SmkVersion == Core.Enums.SmkVersion.Old)
        {
            // Basic module: years 1-2, Specialist module: years 3-6
            int startYear = module.Type == Core.Enums.ModuleType.Basic ? 1 : 3;
            int endYear = module.Type == Core.Enums.ModuleType.Basic ? 2 : 6;

            var allProcedures = await _procedureRepository.GetByUserIdAsync(specializationId.Value);
            return allProcedures.Where(p => p.Year >= startYear && p.Year <= endYear);
        }
        else
        {
            // For New SMK, filter by module ID
            var allProcedures = await _procedureRepository.GetByUserIdAsync(specializationId.Value);

            // Get internships for this module
            var moduleInternshipIds = allProcedures
                .Where(p => p.InternshipId > 0) // TODO: Need to join with internships to filter by module
                .Select(p => p.InternshipId)
                .Distinct();

            return allProcedures.Where(p => moduleInternshipIds.Contains(p.InternshipId));
        }
    }
}
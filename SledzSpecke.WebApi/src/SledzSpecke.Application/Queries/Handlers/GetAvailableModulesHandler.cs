using SledzSpecke.Application.Abstractions;
using SledzSpecke.Application.DTO;
using SledzSpecke.Application.Exceptions;
using SledzSpecke.Core.Repositories;
using SledzSpecke.Core.ValueObjects;

namespace SledzSpecke.Application.Queries.Handlers;

public sealed class GetAvailableModulesHandler : IQueryHandler<GetAvailableModules, IEnumerable<ModuleListDto>>
{
    private readonly ISpecializationRepository _specializationRepository;
    private readonly IUserContextService _userContextService;
    private readonly IUserRepository _userRepository;

    public GetAvailableModulesHandler(
        ISpecializationRepository specializationRepository,
        IUserContextService userContextService,
        IUserRepository userRepository)
    {
        _specializationRepository = specializationRepository;
        _userContextService = userContextService;
        _userRepository = userRepository;
    }

    public async Task<IEnumerable<ModuleListDto>> HandleAsync(GetAvailableModules query)
    {
        // Get current user
        var userId = _userContextService.GetUserId();
        var user = await _userRepository.GetByIdAsync(new UserId(userId));
        if (user is null)
        {
            throw new NotFoundException("User not found.");
        }

        // Verify user owns this specialization
        if (user.SpecializationId.Value != query.SpecializationId)
        {
            throw new UnauthorizedAccessException("You can only view modules for your own specialization.");
        }

        // Get specialization with modules
        var specialization = await _specializationRepository.GetByIdAsync(new SpecializationId(query.SpecializationId));
        if (specialization is null)
        {
            throw new NotFoundException($"Specialization with ID {query.SpecializationId} not found.");
        }

        // Map modules to DTOs
        return specialization.Modules.Select(module => new ModuleListDto
        {
            Id = module.Id.Value,
            SpecializationId = module.SpecializationId.Value,
            Name = module.Name,
            Type = module.Type.ToString(),
            SmkVersion = module.SmkVersion.Value,
            Version = module.Version,
            StartDate = module.StartDate,
            EndDate = module.EndDate,
            IsActive = module.Id == specialization.CurrentModuleId
        }).ToList();
    }
}
using SledzSpecke.Core.Abstractions;
using SledzSpecke.Core.ValueObjects;

namespace SledzSpecke.Core.Policies;

public interface ISmkPolicy<TEntity>
{
    SmkVersion ApplicableVersion { get; }
    Result Validate(TEntity entity, SpecializationContext context);
}

public class SpecializationContext
{
    public SpecializationId SpecializationId { get; init; }
    public UserId UserId { get; init; }
    public SmkVersion SmkVersion { get; init; }
    public ModuleId? CurrentModuleId { get; init; }
    public DateTime ContextDate { get; init; }
    
    public SpecializationContext(
        SpecializationId specializationId, 
        UserId userId, 
        SmkVersion smkVersion,
        ModuleId? currentModuleId = null,
        DateTime? contextDate = null)
    {
        SpecializationId = specializationId;
        UserId = userId;
        SmkVersion = smkVersion;
        CurrentModuleId = currentModuleId;
        ContextDate = contextDate ?? DateTime.UtcNow;
    }
}
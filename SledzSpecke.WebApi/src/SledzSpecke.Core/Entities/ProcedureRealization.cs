using SledzSpecke.Core.Entities.Base;
using SledzSpecke.Core.ValueObjects;
using SledzSpecke.Core.Abstractions;

namespace SledzSpecke.Core.Entities;

public sealed class ProcedureRealization : Entity
{
    public ProcedureRealizationId Id { get; private set; }
    public ProcedureRequirementId RequirementId { get; private set; }
    public UserId UserId { get; private set; }
    public DateTime Date { get; private set; }
    public string Location { get; private set; }
    public ProcedureRole Role { get; private set; }
    public int? Year { get; private set; } // For Stary SMK
    public DateTime CreatedAt { get; private set; }

    // Navigation
    public ProcedureRequirement? Requirement { get; private set; }
    public User? User { get; private set; }

    private ProcedureRealization() { }

    public static Result<ProcedureRealization> Create(
        ProcedureRequirementId requirementId,
        UserId userId,
        DateTime date,
        string location,
        ProcedureRole role,
        int? year = null)
    {
        if (string.IsNullOrWhiteSpace(location))
            return Result<ProcedureRealization>.Failure("Location is required", "LOCATION_REQUIRED");

        var realization = new ProcedureRealization
        {
            Id = new ProcedureRealizationId(0), // Will be set by database
            RequirementId = requirementId,
            UserId = userId,
            Date = date.Date,
            Location = location,
            Role = role,
            Year = year,
            CreatedAt = DateTime.UtcNow
        };

        return Result<ProcedureRealization>.Success(realization);
    }
}
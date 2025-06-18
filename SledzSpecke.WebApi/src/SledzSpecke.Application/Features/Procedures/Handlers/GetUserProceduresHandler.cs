using SledzSpecke.Application.Abstractions;
using SledzSpecke.Application.DTO;
using SledzSpecke.Application.Features.Procedures.Extensions;
using SledzSpecke.Application.Queries;
using SledzSpecke.Core.Entities;
using SledzSpecke.Core.Repositories;
using SledzSpecke.Core.ValueObjects;

namespace SledzSpecke.Application.Features.Procedures.Handlers;

public sealed class GetUserProceduresHandler : IQueryHandler<GetUserProcedures, IEnumerable<ProcedureDto>>
{
    private readonly IProcedureRepository _procedureRepository;
    private readonly IInternshipRepository _internshipRepository;
    private readonly ISpecializationRepository _specializationRepository;

    public GetUserProceduresHandler(
        IProcedureRepository procedureRepository,
        IInternshipRepository internshipRepository,
        ISpecializationRepository specializationRepository)
    {
        _procedureRepository = procedureRepository;
        _internshipRepository = internshipRepository;
        _specializationRepository = specializationRepository;
    }

    public async Task<IEnumerable<ProcedureDto>> HandleAsync(GetUserProcedures query)
    {
        var userId = new UserId(query.UserId);

        // Get all procedures for the user
        var procedures = await _procedureRepository.GetByUserAsync(userId);

        // Filter by internship ID if provided
        if (query.InternshipId.HasValue)
        {
            procedures = procedures.Where(p => p.InternshipId.Value == query.InternshipId.Value);
        }

        // Filter by status if provided
        if (!string.IsNullOrEmpty(query.Status))
        {
            if (Enum.TryParse<ProcedureStatus>(query.Status, out var status))
            {
                procedures = procedures.Where(p => p.Status == status);
            }
        }

        // Apply date range filter if provided
        if (query.StartDate.HasValue && query.EndDate.HasValue)
        {
            procedures = procedures.Where(p => p.Date >= query.StartDate.Value && p.Date <= query.EndDate.Value);
        }
        else if (query.StartDate.HasValue)
        {
            procedures = procedures.Where(p => p.Date >= query.StartDate.Value);
        }
        else if (query.EndDate.HasValue)
        {
            procedures = procedures.Where(p => p.Date <= query.EndDate.Value);
        }

        // Get user's specializations to filter by SMK version
        var userSpecializations = await _specializationRepository.GetByUserIdAsync(userId);

        // If user has specializations, filter procedures by SMK version
        if (userSpecializations.Any())
        {
            var proceduresList = procedures.ToList();
            var filteredProcedures = new List<ProcedureBase>();

            // Get all unique internship IDs from procedures
            var internshipIds = proceduresList.Select(p => p.InternshipId).Distinct().ToList();

            // Batch fetch all internships
            var internships = new Dictionary<InternshipId, Core.Entities.Internship>();
            foreach (var internshipId in internshipIds)
            {
                var internship = await _internshipRepository.GetByIdAsync(internshipId);
                if (internship != null)
                {
                    internships[internshipId] = internship;
                }
            }

            // Filter procedures based on SMK version
            foreach (var procedure in proceduresList)
            {
                if (internships.TryGetValue(procedure.InternshipId, out var internship))
                {
                    // Find the specialization this internship belongs to
                    var specialization = userSpecializations.FirstOrDefault(s => s.Id == internship.SpecializationId);
                    if (specialization != null)
                    {
                        // Only include procedures that match the specialization's SMK version
                        if (procedure.SmkVersion == specialization.SmkVersion)
                        {
                            filteredProcedures.Add(procedure);
                        }
                    }
                }
            }

            procedures = filteredProcedures;
        }

        // Order by date descending
        var orderedProcedures = procedures.OrderByDescending(p => p.Date);

        // Map to DTOs
        return orderedProcedures.Select(p => p.ToDto()).ToList();
    }
}
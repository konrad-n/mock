using SledzSpecke.Application.Abstractions;
using SledzSpecke.Application.DTO;
using SledzSpecke.Application.Queries;
using SledzSpecke.Core.Repositories;
using SledzSpecke.Core.ValueObjects;

namespace SledzSpecke.Application.Queries.Handlers;

internal sealed class GetUserRecognitionsHandler : IQueryHandler<GetUserRecognitions, IEnumerable<RecognitionDto>>
{
    private readonly IRecognitionRepository _recognitionRepository;

    public GetUserRecognitionsHandler(IRecognitionRepository recognitionRepository)
    {
        _recognitionRepository = recognitionRepository;
    }

    public async Task<IEnumerable<RecognitionDto>> HandleAsync(GetUserRecognitions query)
    {
        var userId = new UserId(query.UserId);
        var recognitions = await _recognitionRepository.GetByUserIdAsync(userId);

        if (query.SpecializationId.HasValue)
        {
            recognitions = recognitions.Where(r => r.SpecializationId.Value == query.SpecializationId.Value);
        }

        return recognitions
            .OrderByDescending(r => r.StartDate)
            .Select(r => new RecognitionDto
            {
                Id = r.Id.Value,
                SpecializationId = r.SpecializationId.Value,
                UserId = r.UserId.Value,
                Type = r.Type.ToString(),
                Title = r.Title,
                Description = r.Description,
                Institution = r.Institution,
                StartDate = r.StartDate,
                EndDate = r.EndDate,
                DaysReduction = r.DaysReduction,
                IsApproved = r.IsApproved,
                ApprovedAt = r.ApprovedAt,
                ApprovedBy = r.ApprovedBy?.Value,
                DocumentPath = r.DocumentPath,
                SyncStatus = r.SyncStatus.ToString(),
                CreatedAt = r.CreatedAt,
                UpdatedAt = r.UpdatedAt
            });
    }
}
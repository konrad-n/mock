using SledzSpecke.Application.Abstractions;
using SledzSpecke.Application.DTO;
using SledzSpecke.Core.Repositories;
using Microsoft.Extensions.Logging;

namespace SledzSpecke.Application.Queries.Handlers.AdditionalSelfEducationDays;

public sealed class GetAdditionalSelfEducationDaysBySpecializationHandler 
    : IQueryHandler<GetAdditionalSelfEducationDaysBySpecialization, IEnumerable<AdditionalSelfEducationDaysDto>>
{
    private readonly IAdditionalSelfEducationDaysRepository _repository;
    private readonly IModuleRepository _moduleRepository;
    private readonly ILogger<GetAdditionalSelfEducationDaysBySpecializationHandler> _logger;

    public GetAdditionalSelfEducationDaysBySpecializationHandler(
        IAdditionalSelfEducationDaysRepository repository,
        IModuleRepository moduleRepository,
        ILogger<GetAdditionalSelfEducationDaysBySpecializationHandler> logger)
    {
        _repository = repository;
        _moduleRepository = moduleRepository;
        _logger = logger;
    }

    public async Task<IEnumerable<AdditionalSelfEducationDaysDto>> HandleAsync(
        GetAdditionalSelfEducationDaysBySpecialization query)
    {
        try
        {
            // Get all modules for the specialization
            var modules = await _moduleRepository.GetBySpecializationIdAsync(
                new Core.ValueObjects.SpecializationId(query.SpecializationId));

            var allDays = new List<AdditionalSelfEducationDaysDto>();

            foreach (var module in modules)
            {
                // Get all additional self-education days for each module
                var moduleDays = await _repository.GetByModuleIdAsync(module.Id.Value);

                // Group by year and sum the days
                var daysByYear = moduleDays
                    .GroupBy(d => d.StartDate.Year)
                    .Select(g => new AdditionalSelfEducationDaysDto(
                        Id: g.First().Id, // Use the first ID in the group
                        SpecializationId: query.SpecializationId,
                        Year: g.Key,
                        DaysUsed: g.Sum(d => d.NumberOfDays),
                        Comment: string.Join("; ", g.Select(d => d.Purpose).Distinct())
                    ));

                allDays.AddRange(daysByYear);
            }

            // If there are multiple entries for the same year across modules, combine them
            var result = allDays
                .GroupBy(d => d.Year)
                .Select(g => new AdditionalSelfEducationDaysDto(
                    Id: g.First().Id,
                    SpecializationId: query.SpecializationId,
                    Year: g.Key,
                    DaysUsed: g.Sum(d => d.DaysUsed),
                    Comment: string.Join("; ", g.Select(d => d.Comment).Distinct())
                ))
                .OrderBy(d => d.Year)
                .ToList();

            _logger.LogInformation(
                "Retrieved {Count} additional self-education days records for specialization {SpecializationId}",
                result.Count, query.SpecializationId);

            return result;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving additional self-education days for specialization {SpecializationId}", 
                query.SpecializationId);
            return Enumerable.Empty<AdditionalSelfEducationDaysDto>();
        }
    }
}
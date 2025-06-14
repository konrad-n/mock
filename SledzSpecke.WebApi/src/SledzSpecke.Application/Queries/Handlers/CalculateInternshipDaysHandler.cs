using SledzSpecke.Application.Abstractions;

namespace SledzSpecke.Application.Queries.Handlers;

internal sealed class CalculateInternshipDaysHandler : IQueryHandler<CalculateInternshipDays, int>
{
    public Task<int> HandleAsync(CalculateInternshipDays query)
    {
        if (query.EndDate < query.StartDate)
        {
            throw new ArgumentException("End date must be after start date.");
        }

        // Calculate inclusive days (both start and end date count)
        var days = (query.EndDate.Date - query.StartDate.Date).Days + 1;
        
        return Task.FromResult(days);
    }
}
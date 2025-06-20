using SledzSpecke.Application.Abstractions;
using SledzSpecke.Application.Features.MedicalShifts.DTOs;
using SledzSpecke.Core.Abstractions;
using SledzSpecke.Core.Repositories;
using SledzSpecke.Core.ValueObjects;
using SledzSpecke.Core.Entities;

namespace SledzSpecke.Application.Features.MedicalShifts.Queries.GetMedicalShiftStatistics;

public sealed class GetMedicalShiftStatisticsHandler : IResultQueryHandler<GetMedicalShiftStatistics, MedicalShiftStatisticsDto>
{
    private readonly IMedicalShiftRepository _medicalShiftRepository;

    public GetMedicalShiftStatisticsHandler(IMedicalShiftRepository medicalShiftRepository)
    {
        _medicalShiftRepository = medicalShiftRepository;
    }

    public async Task<Result<MedicalShiftStatisticsDto>> HandleAsync(GetMedicalShiftStatistics query, CancellationToken cancellationToken = default)
    {
        try
        {
            // Filter by year and month
            DateTime startDate, endDate;
            
            if (query.Month.HasValue)
            {
                startDate = new DateTime(query.Year, query.Month.Value, 1);
                endDate = startDate.AddMonths(1).AddDays(-1);
            }
            else
            {
                startDate = new DateTime(query.Year, 1, 1);
                endDate = new DateTime(query.Year, 12, 31);
            }

            // Get medical shifts
            IEnumerable<MedicalShift> medicalShifts;
            if (query.UserId.HasValue)
            {
                medicalShifts = await _medicalShiftRepository.GetByUserIdAndDateRangeAsync(
                    new UserId(query.UserId.Value), startDate, endDate);
            }
            else
            {
                medicalShifts = await _medicalShiftRepository.GetByDateRangeAsync(startDate, endDate, 0);
            }
            
            var shiftsList = medicalShifts.ToList();

            if (!shiftsList.Any())
            {
                return Result<MedicalShiftStatisticsDto>.Success(new MedicalShiftStatisticsDto
                {
                    TotalShifts = 0,
                    TotalHours = 0,
                    TotalMinutes = 0,
                    AverageShiftDuration = 0,
                    ShiftsByLocation = new Dictionary<string, int>(),
                    ShiftsByMonth = new Dictionary<string, int>()
                });
            }

            // Calculate statistics
            var totalShifts = shiftsList.Count;
            var totalMinutes = shiftsList.Sum(s => s.TotalMinutes);
            var totalHours = totalMinutes / 60;
            var remainingMinutes = totalMinutes % 60;
            var averageShiftDuration = totalMinutes / (double)totalShifts;

            // Group by location
            var shiftsByLocation = shiftsList
                .GroupBy(s => s.Location)
                .ToDictionary(g => g.Key, g => g.Count());

            // Group by month
            var shiftsByMonth = shiftsList
                .GroupBy(s => s.Date.ToString("yyyy-MM"))
                .ToDictionary(g => g.Key, g => g.Count());

            var statistics = new MedicalShiftStatisticsDto
            {
                TotalShifts = totalShifts,
                TotalHours = totalHours,
                TotalMinutes = remainingMinutes,
                AverageShiftDuration = averageShiftDuration,
                ShiftsByLocation = shiftsByLocation,
                ShiftsByMonth = shiftsByMonth
            };

            return Result<MedicalShiftStatisticsDto>.Success(statistics);
        }
        catch (Exception ex)
        {
            return Result<MedicalShiftStatisticsDto>.Failure(
                $"An error occurred while retrieving medical shift statistics: {ex.Message}", 
                "STATISTICS_RETRIEVAL_FAILED");
        }
    }
}
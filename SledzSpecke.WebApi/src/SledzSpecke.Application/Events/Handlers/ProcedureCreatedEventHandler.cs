using MediatR;
using Microsoft.Extensions.Logging;
using SledzSpecke.Core.Events;
using SledzSpecke.Core.Repositories;
using SledzSpecke.Application.Abstractions;
using SledzSpecke.Core.ValueObjects;
using SledzSpecke.Core.Entities;
using SledzSpecke.Application.Extensions;

namespace SledzSpecke.Application.Events.Handlers;

public sealed class ProcedureCreatedEventHandler : INotificationHandler<ProcedureCreatedEvent>
{
    private readonly ILogger<ProcedureCreatedEventHandler> _logger;
    private readonly IProcedureRepository _procedureRepository;
    private readonly IInternshipRepository _internshipRepository;
    private readonly IStatisticsService _statisticsService;
    private readonly IValidationService _validationService;
    private readonly INotificationService _notificationService;

    public ProcedureCreatedEventHandler(
        ILogger<ProcedureCreatedEventHandler> logger,
        IProcedureRepository procedureRepository,
        IInternshipRepository internshipRepository,
        IStatisticsService statisticsService,
        IValidationService validationService,
        INotificationService notificationService)
    {
        _logger = logger;
        _procedureRepository = procedureRepository;
        _internshipRepository = internshipRepository;
        _statisticsService = statisticsService;
        _validationService = validationService;
        _notificationService = notificationService;
    }

    public async Task Handle(ProcedureCreatedEvent notification, CancellationToken cancellationToken)
    {
        _logger.LogInformation(
            "Processing ProcedureCreated event: ProcedureId={ProcedureId}, Code={Code}, Date={Date}",
            notification.ProcedureId.Value,
            notification.Code,
            notification.Date);

        try
        {
            // 1. Validate procedure code against SMK requirements
            var validationResult = await _validationService.ValidateProcedureCodeAsync(
                notification.Code,
                notification.SmkVersion.ToString(),
                cancellationToken);

            if (!validationResult.IsValid)
            {
                _logger.LogWarning(
                    "Invalid procedure code detected: Code={Code}, SMKVersion={Version}, Reason={Reason}",
                    notification.Code,
                    notification.SmkVersion.ToString(),
                    string.Join(", ", validationResult.Errors));

                await _notificationService.SendInvalidProcedureWarningAsync(
                    notification.InternshipId,
                    notification.Code,
                    string.Join(", ", validationResult.Errors),
                    cancellationToken);
            }

            // 2. Check for duplicate procedures on the same day
            var dailyProcedures = await _procedureRepository.GetByInternshipIdAndDateAsync(
                notification.InternshipId.Value,
                notification.Date);

            var duplicates = dailyProcedures.Where(p => 
                p.Code == notification.Code && 
                p.Id != notification.ProcedureId.Value).ToList();

            if (duplicates.Any())
            {
                _logger.LogInformation(
                    "Duplicate procedure detected: Code={Code}, Date={Date}, Count={Count}",
                    notification.Code,
                    notification.Date,
                    duplicates.Count);

                // This might be intentional (multiple procedures of same type per day)
                // but worth tracking for statistics
                await _statisticsService.TrackDuplicateProcedureAsync(
                    notification.InternshipId,
                    notification.Code,
                    notification.Date,
                    cancellationToken);
            }

            // 3. Update daily procedure count statistics
            await _statisticsService.UpdateDailyProcedureCountAsync(
                notification.InternshipId,
                notification.Date,
                cancellationToken);

            // 4. Check if this is the first procedure of its type
            var existingProcedures = await _procedureRepository.GetByInternshipIdAsync(
                notification.InternshipId.Value);

            var isFirstOfType = !existingProcedures.Any(p => 
                p.Code == notification.Code && 
                p.Id != notification.ProcedureId.Value);

            if (isFirstOfType)
            {
                await _statisticsService.TrackNewProcedureTypeAsync(
                    notification.InternshipId,
                    notification.Code,
                    cancellationToken);

                // Send achievement notification for first procedure of this type
                await _notificationService.SendFirstProcedureAchievementAsync(
                    notification.InternshipId,
                    notification.Code,
                    "Procedure " + notification.Code,
                    cancellationToken);
            }

            // 5. Update location-based statistics
            if (!string.IsNullOrEmpty(notification.Location))
            {
                await _statisticsService.UpdateLocationStatisticsAsync(
                    notification.Location,
                    notification.Code,
                    cancellationToken);
            }

            // 6. Check daily procedure limits (some procedures have daily limits)
            var dailyLimit = await _validationService.GetDailyProcedureLimitAsync(
                notification.Code,
                cancellationToken);

            if (dailyLimit > 0)
            {
                var todayCount = dailyProcedures.Count(p => p.Code == notification.Code);
                if (todayCount > dailyLimit)
                {
                    await _notificationService.SendDailyLimitExceededWarningAsync(
                        notification.InternshipId,
                        notification.Code,
                        dailyLimit,
                        todayCount,
                        cancellationToken);
                }
            }

            // 7. Track procedure patterns for recommendations
            await TrackProcedurePatterns(notification, cancellationToken);

            _logger.LogInformation(
                "Successfully processed ProcedureCreated event for ProcedureId={ProcedureId}",
                notification.ProcedureId.Value);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex,
                "Error processing ProcedureCreated event for ProcedureId={ProcedureId}",
                notification.ProcedureId.Value);
        }
    }

    private async Task TrackProcedurePatterns(ProcedureCreatedEvent notification, CancellationToken cancellationToken)
    {
        // Track patterns for intelligent recommendations
        var recentProcedures = await _procedureRepository.GetRecentByInternshipIdAsync(
            notification.InternshipId.Value,
            days: 30);

        var patterns = new ProcedurePattern
        {
            MostCommonTime = CalculateMostCommonTime(recentProcedures),
            MostCommonLocation = CalculateMostCommonLocation(recentProcedures),
            AverageDailyCount = CalculateAverageDailyCount(recentProcedures),
            CommonlyPairedProcedures = await FindCommonlyPairedProcedures(
                notification.InternshipId,
                notification.Code,
                recentProcedures,
                cancellationToken)
        };

        await _statisticsService.UpdateProcedurePatternsAsync(
            notification.InternshipId,
            patterns,
            cancellationToken);
    }

    private TimeSpan CalculateMostCommonTime(IEnumerable<ProcedureBase> procedures)
    {
        return procedures
            .GroupBy(p => p.Date.Hour)
            .OrderByDescending(g => g.Count())
            .Select(g => TimeSpan.FromHours(g.Key))
            .FirstOrDefault();
    }

    private string CalculateMostCommonLocation(IEnumerable<ProcedureBase> procedures)
    {
        return procedures
            .Where(p => !string.IsNullOrEmpty(p.Location))
            .GroupBy(p => p.Location)
            .OrderByDescending(g => g.Count())
            .Select(g => g.Key)
            .FirstOrDefault() ?? string.Empty;
    }

    private double CalculateAverageDailyCount(IEnumerable<ProcedureBase> procedures)
    {
        var dailyCounts = procedures
            .GroupBy(p => p.Date.Date)
            .Select(g => g.Count());

        return dailyCounts.Any() ? dailyCounts.Average() : 0;
    }

    private async Task<List<string>> FindCommonlyPairedProcedures(
        InternshipId internshipId,
        string procedureCode,
        IEnumerable<ProcedureBase> recentProcedures,
        CancellationToken cancellationToken)
    {
        var datesWithTargetProcedure = recentProcedures
            .Where(p => p.Code == procedureCode)
            .Select(p => p.Date.Date)
            .Distinct();

        var pairedProcedures = recentProcedures
            .Where(p => p.Code != procedureCode && datesWithTargetProcedure.Contains(p.Date.Date))
            .GroupBy(p => p.Code)
            .Where(g => g.Count() >= 3) // At least 3 occurrences to be considered a pattern
            .OrderByDescending(g => g.Count())
            .Take(5)
            .Select(g => g.Key)
            .ToList();

        return pairedProcedures;
    }

    private record ProcedurePattern
    {
        public TimeSpan MostCommonTime { get; init; }
        public string MostCommonLocation { get; init; }
        public double AverageDailyCount { get; init; }
        public List<string> CommonlyPairedProcedures { get; init; } = new();
    }

}
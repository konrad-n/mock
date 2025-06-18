using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using SledzSpecke.Core.Abstractions;
using SledzSpecke.Core.Entities;
using SledzSpecke.Core.Repositories;
using SledzSpecke.Core.ValueObjects;

namespace SledzSpecke.Core.Sagas;

public class SMKMonthlyReportSagaData
{
    public int InternshipId { get; set; }
    public int Year { get; set; }
    public int Month { get; set; }
    public List<MedicalShift> Shifts { get; set; } = new();
    public List<Procedure> Procedures { get; set; } = new();
    public int? GeneratedReportId { get; set; }
    public decimal TotalHours { get; set; }
    public Dictionary<int, decimal> WeeklyHours { get; set; } = new();
}

public class SMKMonthlyReportSaga : SagaBase<SMKMonthlyReportSagaData>
{
    public SMKMonthlyReportSaga() : base("SMKMonthlyReport")
    {
        AddStep(new ValidateMonthlyHoursStep());
        AddStep(new ValidateProceduresStep());
        AddStep(new GenerateMonthlyReportStep());
        AddStep(new NotifySupervisorStep());
        AddStep(new ArchiveMonthlyDataStep());
    }
}

public class ValidateMonthlyHoursStep : ISagaStep<SMKMonthlyReportSagaData>
{
    public string Name => "ValidateMonthlyHours";
    
    public async Task<Result> ExecuteAsync(SMKMonthlyReportSagaData data, CancellationToken cancellationToken)
    {
        // Calculate total hours
        data.TotalHours = data.Shifts.Sum(s => s.Duration.Hours + (s.Duration.Minutes / 60m));
        
        // Validate 160 hours minimum
        if (data.TotalHours < 160)
        {
            return Result.Failure($"Niewystarczajca liczba godzin: {data.TotalHours}/160");
        }
        
        // Check weekly limits
        foreach (var shift in data.Shifts)
        {
            var weekOfYear = CultureInfo.CurrentCulture.Calendar.GetWeekOfYear(
                shift.Date, 
                CalendarWeekRule.FirstDay, 
                DayOfWeek.Monday);
            
            if (!data.WeeklyHours.ContainsKey(weekOfYear))
                data.WeeklyHours[weekOfYear] = 0;
                
            data.WeeklyHours[weekOfYear] += shift.Duration.Hours + (shift.Duration.Minutes / 60m);
        }
        
        if (data.WeeklyHours.Any(kvp => kvp.Value > 48))
        {
            var exceededWeeks = data.WeeklyHours.Where(kvp => kvp.Value > 48);
            return Result.Failure($"Przekroczono limit 48 godzin tygodniowo w tygodniach: {string.Join(", ", exceededWeeks.Select(w => w.Key))}");
        }
        
        return Result.Success();
    }
    
    public async Task<Result> CompensateAsync(SMKMonthlyReportSagaData data, CancellationToken cancellationToken)
    {
        // No compensation needed for validation
        return Result.Success();
    }
}

public class ValidateProceduresStep : ISagaStep<SMKMonthlyReportSagaData>
{
    public string Name => "ValidateProcedures";
    
    public async Task<Result> ExecuteAsync(SMKMonthlyReportSagaData data, CancellationToken cancellationToken)
    {
        // Validate procedures are within the reporting period
        var startDate = new DateTime(data.Year, data.Month, 1);
        var endDate = startDate.AddMonths(1).AddDays(-1);
        
        var invalidProcedures = data.Procedures
            .Where(p => p.Date < startDate || p.Date > endDate)
            .ToList();
        
        if (invalidProcedures.Any())
        {
            return Result.Failure($"Znaleziono {invalidProcedures.Count} procedur poza okresem sprawozdawczym");
        }
        
        // Additional validation can be added here
        return Result.Success();
    }
    
    public async Task<Result> CompensateAsync(SMKMonthlyReportSagaData data, CancellationToken cancellationToken)
    {
        // No compensation needed for validation
        return Result.Success();
    }
}

public class GenerateMonthlyReportStep : ISagaStep<SMKMonthlyReportSagaData>
{
    public string Name => "GenerateMonthlyReport";
    
    public async Task<Result> ExecuteAsync(SMKMonthlyReportSagaData data, CancellationToken cancellationToken)
    {
        // TODO: Generate actual report
        // For now, we'll simulate report generation
        data.GeneratedReportId = Random.Shared.Next(1000, 9999);
        
        // In real implementation, this would:
        // 1. Create a PDF/XML report
        // 2. Save it to storage
        // 3. Return the report ID
        
        return Result.Success();
    }
    
    public async Task<Result> CompensateAsync(SMKMonthlyReportSagaData data, CancellationToken cancellationToken)
    {
        if (data.GeneratedReportId.HasValue)
        {
            // TODO: Delete the generated report
            data.GeneratedReportId = null;
        }
        
        return Result.Success();
    }
}

public class NotifySupervisorStep : ISagaStep<SMKMonthlyReportSagaData>
{
    public string Name => "NotifySupervisor";
    
    public async Task<Result> ExecuteAsync(SMKMonthlyReportSagaData data, CancellationToken cancellationToken)
    {
        // TODO: Send notification to supervisor
        // For now, we'll simulate notification
        
        // In real implementation, this would:
        // 1. Send email to supervisor
        // 2. Create in-app notification
        // 3. Log the notification
        
        return Result.Success();
    }
    
    public async Task<Result> CompensateAsync(SMKMonthlyReportSagaData data, CancellationToken cancellationToken)
    {
        // TODO: Send cancellation notification
        return Result.Success();
    }
}

public class ArchiveMonthlyDataStep : ISagaStep<SMKMonthlyReportSagaData>
{
    public string Name => "ArchiveMonthlyData";
    
    public async Task<Result> ExecuteAsync(SMKMonthlyReportSagaData data, CancellationToken cancellationToken)
    {
        // TODO: Archive the monthly data
        // For now, we'll simulate archival
        
        // In real implementation, this would:
        // 1. Move data to archive tables
        // 2. Create backup
        // 3. Update status flags
        
        return Result.Success();
    }
    
    public async Task<Result> CompensateAsync(SMKMonthlyReportSagaData data, CancellationToken cancellationToken)
    {
        // TODO: Restore from archive
        return Result.Success();
    }
}
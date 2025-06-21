using System.Linq.Expressions;
using SledzSpecke.Core.Abstractions;
using SledzSpecke.Core.Entities;
using SledzSpecke.Core.Enums;
using SledzSpecke.Core.ValueObjects;

namespace SledzSpecke.Core.Specifications;

/// <summary>
/// Specification for finding active internships
/// </summary>
public sealed class ActiveInternshipSpecification : Specification<Internship>
{
    public override Expression<Func<Internship, bool>> ToExpression()
    {
        return internship => !internship.IsCompleted && 
                           !internship.IsApproved &&
                           internship.EndDate >= DateTime.UtcNow;
    }
}

/// <summary>
/// Specification for finding internships by specialization
/// </summary>
public sealed class InternshipBySpecializationSpecification : Specification<Internship>
{
    private readonly SpecializationId _specializationId;

    public InternshipBySpecializationSpecification(SpecializationId specializationId)
    {
        _specializationId = specializationId;
    }

    public override Expression<Func<Internship, bool>> ToExpression()
    {
        return internship => internship.SpecializationId == _specializationId;
    }
}

/// <summary>
/// Specification for finding internships within a date range
/// </summary>
public sealed class InternshipInDateRangeSpecification : Specification<Internship>
{
    private readonly DateTime _startDate;
    private readonly DateTime _endDate;

    public InternshipInDateRangeSpecification(DateTime startDate, DateTime endDate)
    {
        _startDate = startDate;
        _endDate = endDate;
    }

    public override Expression<Func<Internship, bool>> ToExpression()
    {
        return internship => internship.StartDate >= _startDate && 
                           internship.EndDate <= _endDate;
    }
}

/// <summary>
/// Specification for finding completed but not approved internships
/// </summary>
public sealed class CompletedNotApprovedInternshipSpecification : Specification<Internship>
{
    public override Expression<Func<Internship, bool>> ToExpression()
    {
        return internship => internship.IsCompleted && !internship.IsApproved;
    }
}

/// <summary>
/// Specification for finding internships that need syncing
/// </summary>
public sealed class InternshipNeedsSyncSpecification : Specification<Internship>
{
    public override Expression<Func<Internship, bool>> ToExpression()
    {
        return internship => internship.SyncStatus == Enums.SyncStatus.Unsynced ||
                           internship.SyncStatus == Enums.SyncStatus.Modified ||
                           internship.SyncStatus == Enums.SyncStatus.SyncFailed;
    }
}
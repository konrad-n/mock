using System.Linq.Expressions;
using SledzSpecke.Core.Abstractions;
using SledzSpecke.Core.Entities;
using SledzSpecke.Core.ValueObjects;

namespace SledzSpecke.Core.Specifications;

/// <summary>
/// Specification for finding medical shifts by internship
/// </summary>
public sealed class MedicalShiftByInternshipSpecification : Specification<MedicalShift>
{
    private readonly InternshipId _internshipId;

    public MedicalShiftByInternshipSpecification(InternshipId internshipId)
    {
        _internshipId = internshipId;
    }

    public override Expression<Func<MedicalShift, bool>> ToExpression()
    {
        return shift => shift.InternshipId == _internshipId;
    }
}

/// <summary>
/// Specification for finding medical shifts in a date range
/// </summary>
public sealed class MedicalShiftInDateRangeSpecification : Specification<MedicalShift>
{
    private readonly DateTime _startDate;
    private readonly DateTime _endDate;

    public MedicalShiftInDateRangeSpecification(DateTime startDate, DateTime endDate)
    {
        _startDate = startDate;
        _endDate = endDate;
    }

    public override Expression<Func<MedicalShift, bool>> ToExpression()
    {
        return shift => shift.Date >= _startDate && shift.Date <= _endDate;
    }
}

/// <summary>
/// Specification for finding approved medical shifts
/// </summary>
public sealed class ApprovedMedicalShiftSpecification : Specification<MedicalShift>
{
    public override Expression<Func<MedicalShift, bool>> ToExpression()
    {
        return shift => shift.IsApproved;
    }
}

/// <summary>
/// Specification for finding medical shifts by year
/// </summary>
public sealed class MedicalShiftByYearSpecification : Specification<MedicalShift>
{
    private readonly int _year;

    public MedicalShiftByYearSpecification(int year)
    {
        _year = year;
    }

    public override Expression<Func<MedicalShift, bool>> ToExpression()
    {
        return shift => shift.Year == _year;
    }
}

/// <summary>
/// Specification for finding medical shifts that exceed a certain duration
/// </summary>
public sealed class LongMedicalShiftSpecification : Specification<MedicalShift>
{
    private readonly int _minHours;

    public LongMedicalShiftSpecification(int minHours)
    {
        _minHours = minHours;
    }

    public override Expression<Func<MedicalShift, bool>> ToExpression()
    {
        return shift => shift.Hours >= _minHours;
    }
}
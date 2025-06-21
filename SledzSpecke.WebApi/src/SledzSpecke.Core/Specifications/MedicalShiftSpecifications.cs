using SledzSpecke.Core.Abstractions;
using SledzSpecke.Core.Entities;
using SledzSpecke.Core.ValueObjects;
using System.Linq.Expressions;

namespace SledzSpecke.Core.Specifications;

public class MedicalShiftByInternshipSpecification : Specification<MedicalShift>
{
    private readonly int _internshipId;

    public MedicalShiftByInternshipSpecification(int internshipId)
    {
        _internshipId = internshipId;
    }

    public override Expression<Func<MedicalShift, bool>> ToExpression()
    {
        return shift => shift.InternshipId == _internshipId;
    }
}

public class MedicalShiftByDateRangeSpecification : Specification<MedicalShift>
{
    private readonly DateTime _startDate;
    private readonly DateTime _endDate;

    public MedicalShiftByDateRangeSpecification(DateTime startDate, DateTime endDate)
    {
        _startDate = startDate;
        _endDate = endDate;
    }

    public override Expression<Func<MedicalShift, bool>> ToExpression()
    {
        return shift => shift.Date >= _startDate && shift.Date <= _endDate;
    }
}

public class MedicalShiftByApprovalStatusSpecification : Specification<MedicalShift>
{
    private readonly bool _isApproved;

    public MedicalShiftByApprovalStatusSpecification(bool isApproved)
    {
        _isApproved = isApproved;
    }

    public override Expression<Func<MedicalShift, bool>> ToExpression()
    {
        return shift => shift.IsApproved == _isApproved;
    }
}

public class MedicalShiftByMonthSpecification : Specification<MedicalShift>
{
    private readonly int _year;
    private readonly int _month;

    public MedicalShiftByMonthSpecification(int year, int month)
    {
        _year = year;
        _month = month;
    }

    public override Expression<Func<MedicalShift, bool>> ToExpression()
    {
        return shift => shift.Date.Year == _year && shift.Date.Month == _month;
    }
}

public class MedicalShiftByHoursRangeSpecification : Specification<MedicalShift>
{
    private readonly int _minHours;
    private readonly int _maxHours;

    public MedicalShiftByHoursRangeSpecification(int minHours, int maxHours)
    {
        _minHours = minHours;
        _maxHours = maxHours;
    }

    public override Expression<Func<MedicalShift, bool>> ToExpression()
    {
        return shift => shift.Hours >= _minHours && shift.Hours <= _maxHours;
    }
}

public class MedicalShiftByInternshipIdsSpecification : Specification<MedicalShift>
{
    private readonly List<int> _internshipIds;

    public MedicalShiftByInternshipIdsSpecification(IEnumerable<int> internshipIds)
    {
        _internshipIds = internshipIds.ToList();
    }

    public override Expression<Func<MedicalShift, bool>> ToExpression()
    {
        return shift => _internshipIds.Contains(shift.InternshipId);
    }
}

// Composite specifications for common scenarios
public static class MedicalShiftSpecificationExtensions
{
    public static ISpecification<MedicalShift> GetApprovedShiftsForMonth(int internshipId, int year, int month)
    {
        return new MedicalShiftByInternshipSpecification(internshipId)
            .And(new MedicalShiftByMonthSpecification(year, month))
            .And(new MedicalShiftByApprovalStatusSpecification(true));
    }

    public static ISpecification<MedicalShift> GetPendingShiftsForInternship(int internshipId)
    {
        return new MedicalShiftByInternshipSpecification(internshipId)
            .And(new MedicalShiftByApprovalStatusSpecification(false));
    }

    public static ISpecification<MedicalShift> GetShiftsForDateRange(int internshipId, DateTime startDate, DateTime endDate)
    {
        return new MedicalShiftByInternshipSpecification(internshipId)
            .And(new MedicalShiftByDateRangeSpecification(startDate, endDate));
    }

    public static ISpecification<MedicalShift> GetShiftsForMultipleInternships(IEnumerable<int> internshipIds)
    {
        return new MedicalShiftByInternshipIdsSpecification(internshipIds);
    }
}
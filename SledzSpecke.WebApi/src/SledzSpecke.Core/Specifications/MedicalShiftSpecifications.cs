using SledzSpecke.Core.Abstractions;
using SledzSpecke.Core.Entities;
using SledzSpecke.Core.ValueObjects;
using System.Linq.Expressions;

namespace SledzSpecke.Core.Specifications;

public class MedicalShiftByYearSpecification : Specification<MedicalShift>
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

public class MedicalShiftByInternshipSpecification : Specification<MedicalShift>
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

public class MedicalShiftInDateRangeSpecification : Specification<MedicalShift>
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

public class ApprovedMedicalShiftsSpecification : Specification<MedicalShift>
{
    public override Expression<Func<MedicalShift, bool>> ToExpression()
    {
        return shift => shift.SyncStatus == SyncStatus.Synced && shift.ApprovalDate.HasValue;
    }
}

public class UnsyncedMedicalShiftsSpecification : Specification<MedicalShift>
{
    public override Expression<Func<MedicalShift, bool>> ToExpression()
    {
        return shift => shift.SyncStatus != SyncStatus.Synced;
    }
}

public class MedicalShiftWithMinimumDurationSpecification : Specification<MedicalShift>
{
    private readonly int _minimumHours;

    public MedicalShiftWithMinimumDurationSpecification(int minimumHours)
    {
        _minimumHours = minimumHours;
    }

    public override Expression<Func<MedicalShift, bool>> ToExpression()
    {
        return shift => shift.Hours >= _minimumHours || (shift.Hours == _minimumHours - 1 && shift.Minutes > 0);
    }
}

// Composite specifications
public static class MedicalShiftSpecificationExtensions
{
    public static ISpecification<MedicalShift> GetShiftsForYearAndInternship(int year, InternshipId internshipId)
    {
        return new MedicalShiftByYearSpecification(year)
            .And(new MedicalShiftByInternshipSpecification(internshipId));
    }

    public static ISpecification<MedicalShift> GetApprovedShiftsInDateRange(DateTime startDate, DateTime endDate)
    {
        return new ApprovedMedicalShiftsSpecification()
            .And(new MedicalShiftInDateRangeSpecification(startDate, endDate));
    }

    public static ISpecification<MedicalShift> GetLongShiftsNeedingApproval(int minimumHours = 12)
    {
        return new UnsyncedMedicalShiftsSpecification()
            .And(new MedicalShiftWithMinimumDurationSpecification(minimumHours));
    }
}
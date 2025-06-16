using SledzSpecke.Core.Abstractions;
using SledzSpecke.Core.Entities;
using SledzSpecke.Core.ValueObjects;
using System.Linq.Expressions;

namespace SledzSpecke.Core.Specifications;

public class ProcedureByStatusSpecification : Specification<ProcedureBase>
{
    private readonly ProcedureStatus _status;

    public ProcedureByStatusSpecification(ProcedureStatus status)
    {
        _status = status;
    }

    public override Expression<Func<ProcedureBase, bool>> ToExpression()
    {
        return procedure => procedure.Status == _status;
    }
}

public class ProcedureByYearSpecification : Specification<ProcedureBase>
{
    private readonly int _year;

    public ProcedureByYearSpecification(int year)
    {
        _year = year;
    }

    public override Expression<Func<ProcedureBase, bool>> ToExpression()
    {
        return procedure => procedure.Year == _year;
    }
}

public class ProcedureByInternshipSpecification : Specification<ProcedureBase>
{
    private readonly InternshipId _internshipId;

    public ProcedureByInternshipSpecification(InternshipId internshipId)
    {
        _internshipId = internshipId;
    }

    public override Expression<Func<ProcedureBase, bool>> ToExpression()
    {
        return procedure => procedure.InternshipId == _internshipId;
    }
}

public class CompletedProceduresInDateRangeSpecification : Specification<ProcedureBase>
{
    private readonly DateTime _startDate;
    private readonly DateTime _endDate;

    public CompletedProceduresInDateRangeSpecification(DateTime startDate, DateTime endDate)
    {
        _startDate = startDate;
        _endDate = endDate;
    }

    public override Expression<Func<ProcedureBase, bool>> ToExpression()
    {
        return procedure =>
            procedure.Status == ProcedureStatus.Completed &&
            procedure.Date >= _startDate &&
            procedure.Date <= _endDate;
    }
}

public class UnsyncedProceduresSpecification : Specification<ProcedureBase>
{
    public override Expression<Func<ProcedureBase, bool>> ToExpression()
    {
        return procedure => procedure.SyncStatus != SyncStatus.Synced;
    }
}

public class TypeAProceduresSpecification : Specification<ProcedureBase>
{
    public override Expression<Func<ProcedureBase, bool>> ToExpression()
    {
        return procedure => !string.IsNullOrEmpty(procedure.OperatorCode);
    }
}

// Usage example:
public static class ProcedureSpecificationExamples
{
    public static ISpecification<ProcedureBase> GetCompletedProceduresForYear(int year)
    {
        return new ProcedureByYearSpecification(year)
            .And(new ProcedureByStatusSpecification(ProcedureStatus.Completed));
    }

    public static ISpecification<ProcedureBase> GetUnsyncedTypeAProcedures()
    {
        return new UnsyncedProceduresSpecification()
            .And(new TypeAProceduresSpecification());
    }

    public static ISpecification<ProcedureBase> GetProceduresNeedingReview(DateTime cutoffDate)
    {
        return new ProcedureByStatusSpecification(ProcedureStatus.Pending)
            .And(new ProcedureBeforeDateSpecification(cutoffDate));
    }
}

// Additional specification for date comparison
public class ProcedureBeforeDateSpecification : Specification<ProcedureBase>
{
    private readonly DateTime _cutoffDate;

    public ProcedureBeforeDateSpecification(DateTime cutoffDate)
    {
        _cutoffDate = cutoffDate;
    }

    public override Expression<Func<ProcedureBase, bool>> ToExpression()
    {
        return procedure => procedure.Date < _cutoffDate;
    }
}

public class ProcedureByCodeSpecification : Specification<ProcedureBase>
{
    private readonly string _code;

    public ProcedureByCodeSpecification(string code)
    {
        _code = code;
    }

    public override Expression<Func<ProcedureBase, bool>> ToExpression()
    {
        return procedure => procedure.Code == _code;
    }
}

public class ProcedureByDateRangeSpecification : Specification<ProcedureBase>
{
    private readonly DateTime _startDate;
    private readonly DateTime _endDate;

    public ProcedureByDateRangeSpecification(DateTime startDate, DateTime endDate)
    {
        _startDate = startDate;
        _endDate = endDate;
    }

    public override Expression<Func<ProcedureBase, bool>> ToExpression()
    {
        return procedure => procedure.Date >= _startDate && procedure.Date <= _endDate;
    }
}
using SledzSpecke.Core.Abstractions;
using SledzSpecke.Core.Entities;
using SledzSpecke.Core.ValueObjects;
using System.Linq.Expressions;

namespace SledzSpecke.Core.Specifications;

public sealed class AbsenceByIdSpecification : Specification<Absence>
{
    private readonly AbsenceId _id;

    public AbsenceByIdSpecification(AbsenceId id)
    {
        _id = id;
    }

    public override Expression<Func<Absence, bool>> ToExpression()
    {
        return absence => absence.Id == _id;
    }
}

public sealed class AbsenceByUserSpecification : Specification<Absence>
{
    private readonly UserId _userId;

    public AbsenceByUserSpecification(UserId userId)
    {
        _userId = userId;
    }

    public override Expression<Func<Absence, bool>> ToExpression()
    {
        return absence => absence.UserId == _userId;
    }
}

public sealed class AbsenceBySpecializationSpecification : Specification<Absence>
{
    private readonly SpecializationId _specializationId;

    public AbsenceBySpecializationSpecification(SpecializationId specializationId)
    {
        _specializationId = specializationId;
    }

    public override Expression<Func<Absence, bool>> ToExpression()
    {
        return absence => absence.SpecializationId == _specializationId;
    }
}

public sealed class AbsenceByUserAndSpecializationSpecification : Specification<Absence>
{
    private readonly UserId _userId;
    private readonly SpecializationId _specializationId;

    public AbsenceByUserAndSpecializationSpecification(UserId userId, SpecializationId specializationId)
    {
        _userId = userId;
        _specializationId = specializationId;
    }

    public override Expression<Func<Absence, bool>> ToExpression()
    {
        return absence => absence.UserId == _userId && absence.SpecializationId == _specializationId;
    }
}

public sealed class ActiveAbsenceSpecification : Specification<Absence>
{
    private readonly DateTime _date;

    public ActiveAbsenceSpecification(DateTime date)
    {
        _date = date.Date;
    }

    public override Expression<Func<Absence, bool>> ToExpression()
    {
        return absence => absence.StartDate <= _date && absence.EndDate >= _date && absence.IsApproved;
    }
}

public sealed class ApprovedAbsenceSpecification : Specification<Absence>
{
    public override Expression<Func<Absence, bool>> ToExpression()
    {
        return absence => absence.IsApproved;
    }
}

public sealed class OverlappingAbsenceSpecification : Specification<Absence>
{
    private readonly DateTime _startDate;
    private readonly DateTime _endDate;

    public OverlappingAbsenceSpecification(DateTime startDate, DateTime endDate)
    {
        _startDate = startDate;
        _endDate = endDate;
    }

    public override Expression<Func<Absence, bool>> ToExpression()
    {
        return absence => absence.StartDate <= _endDate && absence.EndDate >= _startDate;
    }
}

public sealed class AbsenceExcludingIdSpecification : Specification<Absence>
{
    private readonly AbsenceId _excludeId;

    public AbsenceExcludingIdSpecification(AbsenceId excludeId)
    {
        _excludeId = excludeId;
    }

    public override Expression<Func<Absence, bool>> ToExpression()
    {
        return absence => absence.Id != _excludeId;
    }
}

public sealed class AbsenceByTypeSpecification : Specification<Absence>
{
    private readonly AbsenceType _type;

    public AbsenceByTypeSpecification(AbsenceType type)
    {
        _type = type;
    }

    public override Expression<Func<Absence, bool>> ToExpression()
    {
        return absence => absence.Type == _type;
    }
}

public static class AbsenceSpecificationExtensions
{
    public static ISpecification<Absence> GetActiveAbsencesForUser(UserId userId)
    {
        return new AbsenceByUserSpecification(userId)
            .And(new ActiveAbsenceSpecification(DateTime.UtcNow));
    }

    public static ISpecification<Absence> GetActiveAbsencesForUserAtDate(UserId userId, DateTime date)
    {
        return new AbsenceByUserSpecification(userId)
            .And(new ActiveAbsenceSpecification(date));
    }

    public static ISpecification<Absence> GetOverlappingAbsencesForUser(
        UserId userId, 
        DateTime startDate, 
        DateTime endDate, 
        AbsenceId? excludeId = null)
    {
        var spec = new AbsenceByUserSpecification(userId)
            .And(new OverlappingAbsenceSpecification(startDate, endDate));

        if (excludeId != null)
        {
            spec = spec.And(new AbsenceExcludingIdSpecification(excludeId).Not());
        }

        return spec;
    }

    public static ISpecification<Absence> GetApprovedAbsencesForUserAndSpecialization(
        UserId userId, 
        SpecializationId specializationId)
    {
        return new AbsenceByUserAndSpecializationSpecification(userId, specializationId)
            .And(new ApprovedAbsenceSpecification());
    }

    public static ISpecification<Absence> GetPendingAbsences()
    {
        return new ApprovedAbsenceSpecification().Not();
    }

    public static ISpecification<Absence> GetAbsencesInDateRange(DateTime startDate, DateTime endDate)
    {
        return new OverlappingAbsenceSpecification(startDate, endDate);
    }
}
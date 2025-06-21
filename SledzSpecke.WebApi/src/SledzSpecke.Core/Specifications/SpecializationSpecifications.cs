using SledzSpecke.Core.Abstractions;
using SledzSpecke.Core.Entities;
using SledzSpecke.Core.ValueObjects;
using System.Linq.Expressions;

namespace SledzSpecke.Core.Specifications;

public sealed class SpecializationByIdSpecification : Specification<Specialization>
{
    private readonly SpecializationId _id;

    public SpecializationByIdSpecification(SpecializationId id)
    {
        _id = id;
    }

    public override Expression<Func<Specialization, bool>> ToExpression()
    {
        return specialization => specialization.SpecializationId == _id.Value;
    }
}

public sealed class SpecializationByUserSpecification : Specification<Specialization>
{
    private readonly UserId _userId;

    public SpecializationByUserSpecification(UserId userId)
    {
        _userId = userId;
    }

    public override Expression<Func<Specialization, bool>> ToExpression()
    {
        return specialization => specialization.UserId == _userId;
    }
}

public sealed class ActiveSpecializationSpecification : Specification<Specialization>
{
    private readonly DateTime _currentDate;

    public ActiveSpecializationSpecification(DateTime currentDate)
    {
        _currentDate = currentDate;
    }

    public override Expression<Func<Specialization, bool>> ToExpression()
    {
        return specialization => specialization.Status == "Active" && 
            (specialization.ActualEndDate == null || specialization.ActualEndDate > _currentDate);
    }
}

public static class SpecializationSpecificationExtensions
{
    public static ISpecification<Specialization> GetSpecializationForUser(SpecializationId id, UserId userId)
    {
        return new SpecializationByIdSpecification(id)
            .And(new SpecializationByUserSpecification(userId));
    }

    public static ISpecification<Specialization> GetActiveSpecializations(UserId userId)
    {
        return new SpecializationByUserSpecification(userId)
            .And(new ActiveSpecializationSpecification(DateTime.UtcNow));
    }
}
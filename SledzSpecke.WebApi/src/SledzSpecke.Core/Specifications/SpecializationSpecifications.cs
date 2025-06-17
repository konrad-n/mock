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
        return specialization => specialization.Id == _id;
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

public static class SpecializationSpecificationExtensions
{
    public static ISpecification<Specialization> GetSpecializationForUser(SpecializationId id, UserId userId)
    {
        return new SpecializationByIdSpecification(id)
            .And(new SpecializationByUserSpecification(userId));
    }
}
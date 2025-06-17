using System.Linq.Expressions;
using SledzSpecke.Core.Abstractions;
using SledzSpecke.Core.Entities;
using SledzSpecke.Core.ValueObjects;

namespace SledzSpecke.Infrastructure.DAL.Specifications.EducationalActivity;

public class EducationalActivityByIdSpecification : Specification<Core.Entities.EducationalActivity>
{
    private readonly EducationalActivityId _id;

    public EducationalActivityByIdSpecification(EducationalActivityId id)
    {
        _id = id;
    }

    public override Expression<Func<Core.Entities.EducationalActivity, bool>> ToExpression()
        => activity => activity.Id == _id;
}
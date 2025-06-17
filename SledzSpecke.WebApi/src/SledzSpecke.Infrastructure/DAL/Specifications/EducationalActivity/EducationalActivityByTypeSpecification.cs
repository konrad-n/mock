using System.Linq.Expressions;
using SledzSpecke.Core.Abstractions;
using SledzSpecke.Core.ValueObjects;

namespace SledzSpecke.Infrastructure.DAL.Specifications.EducationalActivity;

public class EducationalActivityByTypeSpecification : Specification<Core.Entities.EducationalActivity>
{
    private readonly SpecializationId _specializationId;
    private readonly EducationalActivityType _type;

    public EducationalActivityByTypeSpecification(SpecializationId specializationId, EducationalActivityType type)
    {
        _specializationId = specializationId;
        _type = type;
    }

    public override Expression<Func<Core.Entities.EducationalActivity, bool>> ToExpression()
        => activity => activity.SpecializationId == _specializationId && activity.Type == _type;
}
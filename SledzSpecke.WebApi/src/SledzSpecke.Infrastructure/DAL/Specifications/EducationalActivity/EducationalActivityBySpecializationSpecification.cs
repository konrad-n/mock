using System.Linq.Expressions;
using SledzSpecke.Core.Abstractions;
using SledzSpecke.Core.ValueObjects;

namespace SledzSpecke.Infrastructure.DAL.Specifications.EducationalActivity;

public class EducationalActivityBySpecializationSpecification : Specification<Core.Entities.EducationalActivity>
{
    private readonly SpecializationId _specializationId;

    public EducationalActivityBySpecializationSpecification(SpecializationId specializationId)
    {
        _specializationId = specializationId;
    }

    public override Expression<Func<Core.Entities.EducationalActivity, bool>> ToExpression()
        => activity => activity.SpecializationId == _specializationId;
}
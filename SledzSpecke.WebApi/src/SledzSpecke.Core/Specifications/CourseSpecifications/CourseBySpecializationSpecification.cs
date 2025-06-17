using System.Linq.Expressions;
using SledzSpecke.Core.Entities;
using SledzSpecke.Core.ValueObjects;

namespace SledzSpecke.Core.Specifications.CourseSpecifications;

public sealed class CourseBySpecializationSpecification : Specification<Course>
{
    private readonly SpecializationId _specializationId;

    public CourseBySpecializationSpecification(SpecializationId specializationId)
    {
        _specializationId = specializationId;
    }

    public override Expression<Func<Course, bool>> ToExpression()
    {
        return course => course.SpecializationId == _specializationId;
    }
}
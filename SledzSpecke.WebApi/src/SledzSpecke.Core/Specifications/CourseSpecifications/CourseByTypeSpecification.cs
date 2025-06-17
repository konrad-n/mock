using System.Linq.Expressions;
using SledzSpecke.Core.Entities;
using SledzSpecke.Core.ValueObjects;
using SledzSpecke.Core.Abstractions;

namespace SledzSpecke.Core.Specifications.CourseSpecifications;

public sealed class CourseByTypeSpecification : Specification<Course>
{
    private readonly CourseType _courseType;

    public CourseByTypeSpecification(CourseType courseType)
    {
        _courseType = courseType;
    }

    public override Expression<Func<Course, bool>> ToExpression()
    {
        return course => course.CourseType == _courseType;
    }
}
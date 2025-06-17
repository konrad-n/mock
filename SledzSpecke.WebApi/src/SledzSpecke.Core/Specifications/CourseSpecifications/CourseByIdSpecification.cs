using System.Linq.Expressions;
using SledzSpecke.Core.Entities;
using SledzSpecke.Core.ValueObjects;

namespace SledzSpecke.Core.Specifications.CourseSpecifications;

public sealed class CourseByIdSpecification : Specification<Course>
{
    private readonly CourseId _courseId;

    public CourseByIdSpecification(CourseId courseId)
    {
        _courseId = courseId;
    }

    public override Expression<Func<Course, bool>> ToExpression()
    {
        return course => course.Id == _courseId;
    }
}
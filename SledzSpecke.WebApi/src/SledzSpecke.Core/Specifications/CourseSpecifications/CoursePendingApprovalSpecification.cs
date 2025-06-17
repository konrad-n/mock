using System.Linq.Expressions;
using SledzSpecke.Core.Entities;
using SledzSpecke.Core.Abstractions;

namespace SledzSpecke.Core.Specifications.CourseSpecifications;

public sealed class CoursePendingApprovalSpecification : Specification<Course>
{
    public override Expression<Func<Course, bool>> ToExpression()
    {
        return course => !course.IsApproved;
    }
}
using System.Linq.Expressions;
using SledzSpecke.Core.Entities;
using SledzSpecke.Core.ValueObjects;
using SledzSpecke.Core.Abstractions;

namespace SledzSpecke.Core.Specifications.CourseSpecifications;

public static class CourseSpecificationExtensions
{
    public static Specification<Course> GetMandatoryCourses(SpecializationId specializationId)
    {
        return new CourseBySpecializationSpecification(specializationId)
            .And(new MandatoryCourseSpecification());
    }

    public static Specification<Course> GetPendingApprovalForSpecialization(SpecializationId specializationId)
    {
        return new CourseBySpecializationSpecification(specializationId)
            .And(new CoursePendingApprovalSpecification());
    }

    public static Specification<Course> GetValidCoursesForModule(ModuleId moduleId)
    {
        return new CourseByModuleSpecification(moduleId)
            .And(new ApprovedCourseSpecification());
    }

    public static Specification<Course> GetCoursesToSync()
    {
        // Courses that need syncing are those that have been modified recently
        // or have specific sync flags set
        return new CoursesNeedingSyncSpecification();
    }

    public static Specification<Course> GetActiveCoursesForSpecialization(SpecializationId specializationId)
    {
        return new CourseBySpecializationSpecification(specializationId)
            .And(new ActiveCourseSpecification());
    }

    public static Specification<Course> GetCoursesByDateRange(DateTime startDate, DateTime endDate)
    {
        return new CourseByDateRangeSpecification(startDate, endDate);
    }
}

public sealed class MandatoryCourseSpecification : Specification<Course>
{
    public override Expression<Func<Course, bool>> ToExpression()
    {
        return course => course.IsMandatory();
    }
}

public sealed class ApprovedCourseSpecification : Specification<Course>
{
    public override Expression<Func<Course, bool>> ToExpression()
    {
        return course => course.IsApproved;
    }
}

public sealed class ActiveCourseSpecification : Specification<Course>
{
    public override Expression<Func<Course, bool>> ToExpression()
    {
        return course => course.StartDate <= DateTime.UtcNow && course.EndDate >= DateTime.UtcNow;
    }
}

public sealed class CourseByDateRangeSpecification : Specification<Course>
{
    private readonly DateTime _startDate;
    private readonly DateTime _endDate;

    public CourseByDateRangeSpecification(DateTime startDate, DateTime endDate)
    {
        _startDate = startDate;
        _endDate = endDate;
    }

    public override Expression<Func<Course, bool>> ToExpression()
    {
        return course => course.StartDate >= _startDate && course.EndDate <= _endDate;
    }
}

public sealed class CoursesNeedingSyncSpecification : Specification<Course>
{
    public override Expression<Func<Course, bool>> ToExpression()
    {
        // Courses that have been modified or failed sync
        return course => course.SyncStatus == SyncStatus.Modified || 
                        course.SyncStatus == SyncStatus.SyncFailed ||
                        course.SyncStatus == SyncStatus.NotSynced;
    }
}
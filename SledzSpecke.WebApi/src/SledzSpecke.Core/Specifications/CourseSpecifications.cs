using SledzSpecke.Core.Abstractions;
using SledzSpecke.Core.Entities;
using SledzSpecke.Core.ValueObjects;
using System.Linq.Expressions;

namespace SledzSpecke.Core.Specifications;

public sealed class CourseByIdSpecification : Specification<Course>
{
    private readonly CourseId _id;

    public CourseByIdSpecification(CourseId id)
    {
        _id = id;
    }

    public override Expression<Func<Course, bool>> ToExpression()
    {
        return course => course.Id == _id;
    }
}

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

public sealed class CourseByModuleSpecification : Specification<Course>
{
    private readonly ModuleId _moduleId;

    public CourseByModuleSpecification(ModuleId moduleId)
    {
        _moduleId = moduleId;
    }

    public override Expression<Func<Course, bool>> ToExpression()
    {
        return course => course.ModuleId == _moduleId;
    }
}

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

public sealed class CoursePendingApprovalSpecification : Specification<Course>
{
    public override Expression<Func<Course, bool>> ToExpression()
    {
        return course => !course.IsApproved;
    }
}

public sealed class CourseApprovedSpecification : Specification<Course>
{
    public override Expression<Func<Course, bool>> ToExpression()
    {
        return course => course.IsApproved;
    }
}

public sealed class CourseWithCertificateSpecification : Specification<Course>
{
    public override Expression<Func<Course, bool>> ToExpression()
    {
        return course => course.HasCertificate;
    }
}

public sealed class CourseVerifiedByCmkpSpecification : Specification<Course>
{
    public override Expression<Func<Course, bool>> ToExpression()
    {
        return course => course.IsVerifiedByCmkp;
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

public sealed class CourseBySyncStatusSpecification : Specification<Course>
{
    private readonly SyncStatus _syncStatus;

    public CourseBySyncStatusSpecification(SyncStatus syncStatus)
    {
        _syncStatus = syncStatus;
    }

    public override Expression<Func<Course, bool>> ToExpression()
    {
        return course => course.SyncStatus == _syncStatus;
    }
}

public static class CourseSpecificationExtensions
{
    public static ISpecification<Course> GetMandatoryCourses(SpecializationId specializationId)
    {
        return new CourseBySpecializationSpecification(specializationId)
            .And(new CourseByTypeSpecification(CourseType.Specialization)
                .Or(new CourseByTypeSpecification(CourseType.Certification)));
    }

    public static ISpecification<Course> GetPendingApprovalForSpecialization(SpecializationId specializationId)
    {
        return new CourseBySpecializationSpecification(specializationId)
            .And(new CoursePendingApprovalSpecification());
    }

    public static ISpecification<Course> GetValidCoursesForModule(ModuleId moduleId)
    {
        return new CourseByModuleSpecification(moduleId)
            .And(new CourseApprovedSpecification())
            .And(new CourseVerifiedByCmkpSpecification());
    }

    public static ISpecification<Course> GetCoursesToSync()
    {
        return new CourseBySyncStatusSpecification(SyncStatus.NotSynced)
            .Or(new CourseBySyncStatusSpecification(SyncStatus.Modified));
    }

    public static ISpecification<Course> GetRecentCourses(DateTime afterDate)
    {
        return new CourseCreatedAfterSpecification(afterDate);
    }
}

public sealed class CourseCreatedAfterSpecification : Specification<Course>
{
    private readonly DateTime _afterDate;

    public CourseCreatedAfterSpecification(DateTime afterDate)
    {
        _afterDate = afterDate;
    }

    public override Expression<Func<Course, bool>> ToExpression()
    {
        return course => course.CreatedAt >= _afterDate;
    }
}
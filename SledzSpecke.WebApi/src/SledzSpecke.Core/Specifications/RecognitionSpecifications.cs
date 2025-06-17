using SledzSpecke.Core.Abstractions;
using SledzSpecke.Core.Entities;
using SledzSpecke.Core.ValueObjects;
using System.Linq.Expressions;

namespace SledzSpecke.Core.Specifications;

public sealed class RecognitionByIdSpecification : Specification<Recognition>
{
    private readonly RecognitionId _id;

    public RecognitionByIdSpecification(RecognitionId id)
    {
        _id = id;
    }

    public override Expression<Func<Recognition, bool>> ToExpression()
    {
        return recognition => recognition.Id == _id;
    }
}

public sealed class RecognitionByUserSpecification : Specification<Recognition>
{
    private readonly UserId _userId;

    public RecognitionByUserSpecification(UserId userId)
    {
        _userId = userId;
    }

    public override Expression<Func<Recognition, bool>> ToExpression()
    {
        return recognition => recognition.UserId == _userId;
    }
}

public sealed class RecognitionBySpecializationSpecification : Specification<Recognition>
{
    private readonly SpecializationId _specializationId;

    public RecognitionBySpecializationSpecification(SpecializationId specializationId)
    {
        _specializationId = specializationId;
    }

    public override Expression<Func<Recognition, bool>> ToExpression()
    {
        return recognition => recognition.SpecializationId == _specializationId;
    }
}

public sealed class RecognitionByTypeSpecification : Specification<Recognition>
{
    private readonly RecognitionType _type;

    public RecognitionByTypeSpecification(RecognitionType type)
    {
        _type = type;
    }

    public override Expression<Func<Recognition, bool>> ToExpression()
    {
        return recognition => recognition.Type == _type;
    }
}

public sealed class ApprovedRecognitionSpecification : Specification<Recognition>
{
    public override Expression<Func<Recognition, bool>> ToExpression()
    {
        return recognition => recognition.IsApproved;
    }
}

public sealed class PendingRecognitionSpecification : Specification<Recognition>
{
    public override Expression<Func<Recognition, bool>> ToExpression()
    {
        return recognition => !recognition.IsApproved;
    }
}

public sealed class RecognitionWithDocumentSpecification : Specification<Recognition>
{
    public override Expression<Func<Recognition, bool>> ToExpression()
    {
        return recognition => !string.IsNullOrEmpty(recognition.DocumentPath);
    }
}

public sealed class RecognitionWithReductionSpecification : Specification<Recognition>
{
    public override Expression<Func<Recognition, bool>> ToExpression()
    {
        return recognition => recognition.DaysReduction > 0;
    }
}

public sealed class RecognitionInDateRangeSpecification : Specification<Recognition>
{
    private readonly DateTime _startDate;
    private readonly DateTime _endDate;

    public RecognitionInDateRangeSpecification(DateTime startDate, DateTime endDate)
    {
        _startDate = startDate;
        _endDate = endDate;
    }

    public override Expression<Func<Recognition, bool>> ToExpression()
    {
        return recognition => recognition.StartDate <= _endDate && recognition.EndDate >= _startDate;
    }
}

public static class RecognitionSpecificationExtensions
{
    public static ISpecification<Recognition> GetApprovedRecognitionsForUser(UserId userId)
    {
        return new RecognitionByUserSpecification(userId)
            .And(new ApprovedRecognitionSpecification());
    }

    public static ISpecification<Recognition> GetPendingRecognitionsForUser(UserId userId)
    {
        return new RecognitionByUserSpecification(userId)
            .And(new PendingRecognitionSpecification());
    }

    public static ISpecification<Recognition> GetRecognitionsForUserAndSpecialization(UserId userId, SpecializationId specializationId)
    {
        return new RecognitionByUserSpecification(userId)
            .And(new RecognitionBySpecializationSpecification(specializationId));
    }

    public static ISpecification<Recognition> GetApprovedRecognitionsWithReduction(UserId userId)
    {
        return new RecognitionByUserSpecification(userId)
            .And(new ApprovedRecognitionSpecification())
            .And(new RecognitionWithReductionSpecification());
    }

    public static ISpecification<Recognition> GetRecognitionsByTypeForUser(UserId userId, RecognitionType type)
    {
        return new RecognitionByUserSpecification(userId)
            .And(new RecognitionByTypeSpecification(type));
    }

    public static ISpecification<Recognition> GetOverlappingRecognitions(UserId userId, DateTime startDate, DateTime endDate, RecognitionId? excludeId = null)
    {
        var spec = new RecognitionByUserSpecification(userId)
            .And(new RecognitionInDateRangeSpecification(startDate, endDate));

        if (excludeId != null)
        {
            spec = spec.And(new RecognitionByIdSpecification(excludeId).Not());
        }

        return spec;
    }

    public static ISpecification<Recognition> GetRecognitionsRequiringDocument()
    {
        return new PendingRecognitionSpecification()
            .And(new RecognitionWithDocumentSpecification().Not());
    }
}
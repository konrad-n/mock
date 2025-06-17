using System.Linq.Expressions;
using SledzSpecke.Core.Abstractions;
using SledzSpecke.Core.ValueObjects;

namespace SledzSpecke.Infrastructure.DAL.Specifications.FileMetadata;

public class FileMetadataByUserSpecification : Specification<Core.Entities.FileMetadata>
{
    private readonly UserId _userId;

    public FileMetadataByUserSpecification(UserId userId)
    {
        _userId = userId;
    }

    public override Expression<Func<Core.Entities.FileMetadata, bool>> ToExpression()
        => metadata => metadata.UploadedByUserId == _userId;
}
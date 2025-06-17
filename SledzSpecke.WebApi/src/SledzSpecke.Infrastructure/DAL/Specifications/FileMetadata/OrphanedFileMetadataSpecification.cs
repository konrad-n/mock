using System.Linq.Expressions;
using SledzSpecke.Core.Abstractions;

namespace SledzSpecke.Infrastructure.DAL.Specifications.FileMetadata;

public class OrphanedFileMetadataSpecification : Specification<Core.Entities.FileMetadata>
{
    private readonly DateTime _olderThan;

    public OrphanedFileMetadataSpecification(DateTime olderThan)
    {
        _olderThan = olderThan;
    }

    public override Expression<Func<Core.Entities.FileMetadata, bool>> ToExpression()
        => metadata => metadata.IsDeleted && metadata.DeletedAt < _olderThan;
}
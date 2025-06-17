using System.Linq.Expressions;
using SledzSpecke.Core.Abstractions;

namespace SledzSpecke.Infrastructure.DAL.Specifications.FileMetadata;

public class FileMetadataByEntitySpecification : Specification<Core.Entities.FileMetadata>
{
    private readonly string _entityType;
    private readonly int _entityId;

    public FileMetadataByEntitySpecification(string entityType, int entityId)
    {
        _entityType = entityType;
        _entityId = entityId;
    }

    public override Expression<Func<Core.Entities.FileMetadata, bool>> ToExpression()
        => metadata => metadata.EntityType == _entityType && metadata.EntityId == _entityId;
}
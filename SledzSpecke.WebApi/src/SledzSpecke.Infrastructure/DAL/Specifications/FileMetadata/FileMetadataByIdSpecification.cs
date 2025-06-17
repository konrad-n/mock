using System.Linq.Expressions;
using SledzSpecke.Core.Abstractions;

namespace SledzSpecke.Infrastructure.DAL.Specifications.FileMetadata;

public class FileMetadataByIdSpecification : Specification<Core.Entities.FileMetadata>
{
    private readonly int _id;

    public FileMetadataByIdSpecification(int id)
    {
        _id = id;
    }

    public override Expression<Func<Core.Entities.FileMetadata, bool>> ToExpression()
        => metadata => metadata.Id == _id;
}
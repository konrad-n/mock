using System.Linq.Expressions;
using SledzSpecke.Core.Abstractions;
using SledzSpecke.Core.ValueObjects;

namespace SledzSpecke.Infrastructure.DAL.Specifications.FileMetadata;

public class FileMetadataByFilePathSpecification : Specification<Core.Entities.FileMetadata>
{
    private readonly FilePath _filePath;

    public FileMetadataByFilePathSpecification(FilePath filePath)
    {
        _filePath = filePath;
    }

    public override Expression<Func<Core.Entities.FileMetadata, bool>> ToExpression()
        => metadata => metadata.FilePath == _filePath;
}
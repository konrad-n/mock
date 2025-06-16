using SledzSpecke.Application.Abstractions;
using SledzSpecke.Application.DTO;

namespace SledzSpecke.Application.Queries;

public record ValidateSpecializationForSmk(int SpecializationId) : IQuery<SmkValidationResultDto>;
public record ExportSpecializationToXlsx(int SpecializationId) : IQuery<byte[]>;
public record PreviewSmkExport(int SpecializationId) : IQuery<SmkExportPreviewDto>;
public record GetSmkRequirements(string Specialization, string SmkVersion) : IQuery<SmkRequirementsDto>;
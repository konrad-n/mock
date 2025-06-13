using SledzSpecke.Application.Abstractions;
using SledzSpecke.Application.DTO;

namespace SledzSpecke.Application.Queries;

public record GetCourses(int SpecializationId, int? ModuleId = null, string? CourseType = null) : IQuery<IEnumerable<CourseDto>>;
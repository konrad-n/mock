namespace SledzSpecke.Application.DTO;

public record AdditionalSelfEducationDaysDto(
    int Id,
    int SpecializationId,
    int Year,
    int DaysUsed,
    string Comment
);

public record CreateAdditionalSelfEducationDaysDto(
    int SpecializationId,
    int Year,
    int DaysUsed,
    string? Comment
);

public record UpdateAdditionalSelfEducationDaysDto(
    int DaysUsed,
    string? Comment
);
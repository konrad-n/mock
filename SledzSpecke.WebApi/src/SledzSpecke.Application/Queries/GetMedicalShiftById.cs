using SledzSpecke.Application.Abstractions;
using SledzSpecke.Application.DTO;

namespace SledzSpecke.Application.Queries;

public record GetMedicalShiftById(int ShiftId) : IQuery<MedicalShiftDto>;
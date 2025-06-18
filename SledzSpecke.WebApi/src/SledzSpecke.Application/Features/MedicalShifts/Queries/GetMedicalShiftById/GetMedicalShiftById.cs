using SledzSpecke.Application.Abstractions;
using SledzSpecke.Application.Features.MedicalShifts.DTOs;

namespace SledzSpecke.Application.Features.MedicalShifts.Queries.GetMedicalShiftById;

public record GetMedicalShiftById(int Id) : IQuery<MedicalShiftDto>;
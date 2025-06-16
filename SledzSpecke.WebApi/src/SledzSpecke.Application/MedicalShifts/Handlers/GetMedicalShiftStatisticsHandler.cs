using SledzSpecke.Application.Abstractions;
using SledzSpecke.Application.DTO;
using SledzSpecke.Application.Queries;
using SledzSpecke.Core.Repositories;
using SledzSpecke.Core.ValueObjects;
using SledzSpecke.Core.Entities;

namespace SledzSpecke.Application.MedicalShifts.Handlers;

public class GetMedicalShiftStatisticsHandler : IQueryHandler<GetMedicalShiftStatistics, MedicalShiftSummaryDto>
{
    private readonly IMedicalShiftRepository _medicalShiftRepository;
    private readonly IUserRepository _userRepository;
    private readonly IUserContextService _userContextService;
    private readonly ISpecializationRepository _specializationRepository;

    public GetMedicalShiftStatisticsHandler(
        IMedicalShiftRepository medicalShiftRepository,
        IUserRepository userRepository,
        IUserContextService userContextService,
        ISpecializationRepository specializationRepository)
    {
        _medicalShiftRepository = medicalShiftRepository;
        _userRepository = userRepository;
        _userContextService = userContextService;
        _specializationRepository = specializationRepository;
    }

    public async Task<MedicalShiftSummaryDto> HandleAsync(GetMedicalShiftStatistics query)
    {
        var userId = _userContextService.GetUserId();
        var user = await _userRepository.GetByIdAsync(new UserId(userId));
        if (user is null)
        {
            throw new UnauthorizedAccessException("User not found");
        }

        // TODO: User-Specialization relationship needs to be redesigned
        // var specialization = await _specializationRepository.GetByIdAsync(user.SpecializationId);
        // if (specialization is null)
        // {
        //     throw new InvalidOperationException("User specialization not found");
        // }
        throw new InvalidOperationException("User-Specialization relationship needs to be redesigned. Cannot retrieve statistics.");
    }
}
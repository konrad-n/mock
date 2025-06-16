using SledzSpecke.Application.Abstractions;
using SledzSpecke.Application.DTO;

namespace SledzSpecke.Application.Queries;

public record GetUserSmkDetails(int UserId) : IQuery<UserSmkDto>;
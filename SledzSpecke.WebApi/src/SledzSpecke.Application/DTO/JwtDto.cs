namespace SledzSpecke.Application.DTO;

public record JwtDto(
    string AccessToken,
    string RefreshToken,
    long Expires,
    int UserId,
    string Role,
    IDictionary<string, IEnumerable<string>> Claims
);
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using SledzSpecke.Application.DTO;
using SledzSpecke.Application.Security;
using SledzSpecke.Core.Abstractions;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace SledzSpecke.Infrastructure.Auth;

internal sealed class Authenticator : IAuthenticator
{
    private readonly AuthOptions _options;
    private readonly IClock _clock;
    private readonly SigningCredentials _signingCredentials;
    private readonly string _issuer;

    public Authenticator(IOptions<AuthOptions> options, IClock clock)
    {
        _options = options.Value;
        _clock = clock;
        _signingCredentials = new SigningCredentials(new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_options.SigningKey)),
            SecurityAlgorithms.HmacSha256);
        _issuer = _options.Issuer;
    }

    public JwtDto CreateToken(int userId, string role, IDictionary<string, IEnumerable<string>>? claims = null)
    {
        var now = _clock.Current();
        var expires = _options.Expiry.HasValue ? now.Add(_options.Expiry.Value) : now.AddHours(1);

        var jwtClaims = new List<Claim>
        {
            new(JwtRegisteredClaimNames.Sub, userId.ToString()),
            new(JwtRegisteredClaimNames.UniqueName, userId.ToString()),
            new(ClaimTypes.Role, role),
            new(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
            new(JwtRegisteredClaimNames.Iat, new DateTimeOffset(now).ToUnixTimeSeconds().ToString())
        };

        if (claims?.Any() is true)
        {
            var customClaims = claims.SelectMany(x => x.Value.Select(v => new Claim(x.Key, v)));
            jwtClaims.AddRange(customClaims);
        }

        var jwt = new JwtSecurityToken(_issuer, _options.Audience, jwtClaims, now, expires, _signingCredentials);
        var accessToken = new JwtSecurityTokenHandler().WriteToken(jwt);

        return new JwtDto(accessToken, string.Empty, new DateTimeOffset(expires).ToUnixTimeSeconds(), userId, role, claims ?? new Dictionary<string, IEnumerable<string>>());
    }
}
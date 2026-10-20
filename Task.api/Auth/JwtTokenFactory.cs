using System.Security.Claims;
using System.Text;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.JsonWebTokens;
using Microsoft.IdentityModel.Tokens;
using TaskManager.Api.Options;
using TaskManager.Core.DTOs.Responses;

namespace TaskManager.Api.Auth;

// GN-04-R1: the API creates the token from the user the Service returned
public class JwtTokenFactory(IOptions<JwtOptions> options, TimeProvider time)
{
    private readonly JsonWebTokenHandler _handler = new();

    // GN-04: claims sub, role and email; HMAC-SHA256; valid for ExpiryMinutes
    public string Create(UserResponse user)
    {
        var jwt = options.Value;
        var now = time.GetUtcNow().UtcDateTime;
        return _handler.CreateToken(new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(
            [
                new Claim(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
                new Claim("role", user.Role.ToString()),
                new Claim(JwtRegisteredClaimNames.Email, user.Email),
            ]),
            Issuer = jwt.Issuer,
            Audience = jwt.Audience,
            NotBefore = now,
            IssuedAt = now,
            Expires = now.AddMinutes(jwt.ExpiryMinutes),
            SigningCredentials = new SigningCredentials(
                new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwt.Key)), SecurityAlgorithms.HmacSha256),
        });
    }
}

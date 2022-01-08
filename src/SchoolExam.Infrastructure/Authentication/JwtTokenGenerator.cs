using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Microsoft.IdentityModel.Tokens;
using SchoolExam.Application.Authentication;
using SchoolExam.Domain.Entities.UserAggregate;

namespace SchoolExam.Infrastructure.Authentication;

public class JwtTokenGenerator : ITokenGenerator
{
    public string Generate(User user)
    {
        var tokenHandler = new JwtSecurityTokenHandler();
        var key = Base64UrlEncoder.DecodeBytes("gLGtlGNQw8n7iHxUFjuDmHFcPRDUteRROdqhbhCstxEOIiit6kBT6exFo0Lm5uR");
        SymmetricSecurityKey signingKey = new SymmetricSecurityKey(key);
        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(new[]
            {
                new Claim(CustomClaimTypes.UserId, user.Id.ToString()),
                new Claim(CustomClaimTypes.PersonId, user.PersonId?.ToString() ?? string.Empty),
                new Claim(ClaimTypes.Role, user.Role)
            }),
            Expires = DateTime.UtcNow.AddHours(1),
            SigningCredentials = new SigningCredentials(signingKey, SecurityAlgorithms.HmacSha256)
        };
        var token = tokenHandler.CreateToken(tokenDescriptor);
        return tokenHandler.WriteToken(token);
    }
}
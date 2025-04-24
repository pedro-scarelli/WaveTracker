namespace LoginApi.Services;

using System;
using System.Text;
using System.Security.Claims;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;

public class JwtService
{
    private readonly string _secret = Environment.GetEnvironmentVariable("JWT_KEY")
                   ?? throw new InvalidOperationException("JWT Key must be provided");
    private readonly string _issuer = Environment.GetEnvironmentVariable("JWT_ISSUER")
                   ?? throw new InvalidOperationException("JWT Issuer must be provided");

    public string GenerateToken(string userId, string email)
    {
        var claims = new[]
        {
            new Claim(ClaimTypes.NameIdentifier, userId),
            new Claim(ClaimTypes.Email, email)
        };

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_secret));
        var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var token = new JwtSecurityToken(
            issuer: _issuer,
            audience: _issuer,
            claims: claims,
            expires: DateTime.Now.AddHours(1),
            signingCredentials: credentials
        );

        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}

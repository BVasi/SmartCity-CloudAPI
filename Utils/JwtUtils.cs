using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Domain.models;
using Microsoft.IdentityModel.Tokens;

namespace utils;

public static class JwtUtils
{
    public static string GenerateTokenForUser(User user, IConfiguration configuration)
    {
        var claims = new[]
        {
            new Claim(ClaimTypes.NameIdentifier, user.RowKey),
            new Claim(ClaimTypes.Role, user.Type!),
        };

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuration.GetValue<string>(JWT_KEY)!));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
        var token = new JwtSecurityToken(
            claims: claims,
            expires: DateTime.Now.AddHours(VALABILITY_TIME),
            signingCredentials: creds
        );
        return new JwtSecurityTokenHandler().WriteToken(token);
    }

    public static string? GetUserEmailFromToken(string token, IConfiguration configuration)
    {
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuration.GetValue<string>(JWT_KEY)!));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
        try
        {
            var validationParameters = new TokenValidationParameters
            {
                ValidateIssuer = false,
                ValidateAudience = false,
                ValidateLifetime = true,
                ClockSkew = TimeSpan.Zero,
                IssuerSigningKey = creds.Key
            };
            return new JwtSecurityTokenHandler()
                .ValidateToken(token, validationParameters, out var validatedToken)
                .FindFirst(ClaimTypes.NameIdentifier)?.Value;
        }
        catch (Exception)
        {
            return null;
        }
    }

    private const string JWT_KEY = "Jwt:Key";
    private const int VALABILITY_TIME = 1;
}
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using LastDungeon.Api.Models;
using Microsoft.IdentityModel.Tokens;

namespace LastDungeon.Api.Services;

public class JwtService : IJwtService
{
    private readonly IConfiguration _configuration;

    public JwtService(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    public (string Token, DateTime ExpiraEn) GenerateToken(Jugador jugador)
    {
        var secretKey = _configuration["Jwt:Key"] ?? "LastDungeonSecretKeyForJwtAuthenticationDefault2026";
        var issuer = _configuration["Jwt:Issuer"] ?? "LastDungeonApi";
        var audience = _configuration["Jwt:Audience"] ?? "LastDungeonClient";
        var expireDays = _configuration.GetValue<int>("Jwt:ExpireDays", 7);

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey));
        var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var expires = DateTime.UtcNow.AddDays(expireDays);

        var claims = new[]
        {
            new Claim(ClaimTypes.NameIdentifier, jugador.Id.ToString()),
            new Claim("id", jugador.Id.ToString()),
            new Claim(ClaimTypes.Email, jugador.Correo),
            new Claim(ClaimTypes.Name, jugador.NombreUsuario),
            new Claim("verificado", jugador.Verificado.ToString().ToLower())
        };

        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(claims),
            Expires = expires,
            Issuer = issuer,
            Audience = audience,
            SigningCredentials = credentials
        };

        var tokenHandler = new JwtSecurityTokenHandler();
        var token = tokenHandler.CreateToken(tokenDescriptor);

        return (tokenHandler.WriteToken(token), expires);
    }

    public ClaimsPrincipal? ValidateToken(string token)
    {
        var secretKey = _configuration["Jwt:Key"] ?? "LastDungeonSecretKeyForJwtAuthenticationDefault2026";
        var issuer = _configuration["Jwt:Issuer"] ?? "LastDungeonApi";
        var audience = _configuration["Jwt:Audience"] ?? "LastDungeonClient";

        var tokenHandler = new JwtSecurityTokenHandler();
        var validationParameters = new TokenValidationParameters
        {
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey)),
            ValidateIssuer = true,
            ValidIssuer = issuer,
            ValidateAudience = true,
            ValidAudience = audience,
            ValidateLifetime = true,
            ClockSkew = TimeSpan.Zero
        };

        try
        {
            var principal = tokenHandler.ValidateToken(token, validationParameters, out _);
            return principal;
        }
        catch
        {
            return null;
        }
    }
}

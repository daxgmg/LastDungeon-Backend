using System.Security.Claims;
using LastDungeon.Api.Models;

namespace LastDungeon.Api.Services;

public interface IJwtService
{
    (string Token, DateTime ExpiraEn) GenerateToken(Jugador jugador);
    ClaimsPrincipal? ValidateToken(string token);
}

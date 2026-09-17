using LastDungeon.Api.DTOs.Jugadores;

namespace LastDungeon.Api.DTOs.Auth;

public class TokenResponse
{
    public string Token { get; set; } = string.Empty;
    public string Tipo { get; set; } = "Bearer";
    public DateTime ExpiraEn { get; set; }
    public JugadorResponse? Jugador { get; set; }
}

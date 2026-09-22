using LastDungeon.Api.DTOs.Jugadores;
using LastDungeon.Api.Models;

namespace LastDungeon.Api.Mappings;

public static class JugadorMappings
{
    public static JugadorResponse ToResponse(this Jugador jugador)
    {
        return new JugadorResponse(
            jugador.Id,
            jugador.Correo,
            jugador.NombreUsuario,
            jugador.Verificado,
            jugador.Monedas,
            jugador.MejorPiso,
            jugador.FechaRegistro,
            jugador.TieneBotas,
            jugador.BotasEquipadas
        );
    }
}

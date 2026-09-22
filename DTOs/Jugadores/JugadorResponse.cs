namespace LastDungeon.Api.DTOs.Jugadores;

public record JugadorResponse(
    int Id,
    string Correo,
    string NombreUsuario,
    bool Verificado,
    int Monedas,
    int MejorPiso,
    DateTime FechaRegistro,
    bool TieneBotas,
    bool BotasEquipadas
);

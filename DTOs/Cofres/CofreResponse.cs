namespace LastDungeon.Api.DTOs.Cofres;

public record CofreResponse(
    int Id,
    int RunId,
    int Piso,
    string TipoRecompensa,
    int Valor,
    bool Abierto,
    DateTime? FechaApertura
);

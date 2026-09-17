namespace LastDungeon.Api.DTOs.Runs;

public record CombateResponse(
    int Id,
    int RunId,
    string Enemigo,
    string Resultado,
    int HpRestante,
    int MonedasDropeadas,
    int VidaDropeada,
    DateTime Fecha
);

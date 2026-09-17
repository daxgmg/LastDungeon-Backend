namespace LastDungeon.Api.DTOs.Runs;

public record RunResponse(
    int Id,
    int JugadorId,
    string Semilla,
    int PisoAlcanzado,
    string Estado,
    int MonedasObtenidas,
    DateTime FechaInicio,
    DateTime? FechaFin
);

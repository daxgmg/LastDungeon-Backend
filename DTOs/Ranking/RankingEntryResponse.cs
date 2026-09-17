namespace LastDungeon.Api.DTOs.Ranking;

public record RankingEntryResponse(
    int Posicion,
    int JugadorId,
    string NombreUsuario,
    int MejorPiso,
    int Monedas
);

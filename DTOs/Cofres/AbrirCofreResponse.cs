namespace LastDungeon.Api.DTOs.Cofres;

public record AbrirCofreResponse(
    int CofreId,
    string TipoRecompensa,
    int Valor,
    string Mensaje,
    int MonedasObtenidasEnRun
);

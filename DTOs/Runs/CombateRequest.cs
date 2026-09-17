using System.ComponentModel.DataAnnotations;

namespace LastDungeon.Api.DTOs.Runs;

public record CombateRequest(
    [Required] string Enemigo,
    [Required] string Resultado,
    int HpRestante,
    int MonedasDropeadas,
    int VidaDropeada
);

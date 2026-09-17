using System.ComponentModel.DataAnnotations;

namespace LastDungeon.Api.DTOs.Runs;

public record FinalizarRunRequest(
    [Required] string Estado,
    int PisoAlcanzado
);

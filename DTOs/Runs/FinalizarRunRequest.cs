using System.ComponentModel.DataAnnotations;

namespace LastDungeon.Api.DTOs.Runs;

public class FinalizarRunRequest
{
    [Required]
    public string Estado { get; set; }
    public int MonedasObtenidas { get; set; }
    public int PisoAlcanzado { get; set; }
}

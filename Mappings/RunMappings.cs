using LastDungeon.Api.DTOs.Runs;
using LastDungeon.Api.Models;

namespace LastDungeon.Api.Mappings;

public static class RunMappings
{
    public static RunResponse ToResponse(this Run run) =>
        new(
            run.Id,
            run.JugadorId,
            run.Semilla,
            run.PisoAlcanzado,
            run.Estado,
            run.MonedasObtenidas,
            run.FechaInicio,
            run.FechaFin
        );

    public static CombateResponse ToResponse(this Combate combate) =>
        new(
            combate.Id,
            combate.RunId,
            combate.Enemigo,
            combate.Resultado,
            combate.HpRestante,
            combate.MonedasDropeadas,
            combate.VidaDropeada,
            combate.Fecha
        );
}

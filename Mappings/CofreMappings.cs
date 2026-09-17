using LastDungeon.Api.DTOs.Cofres;
using LastDungeon.Api.Models;

namespace LastDungeon.Api.Mappings;

public static class CofreMappings
{
    public static CofreResponse ToResponse(this Cofre cofre) =>
        new(
            cofre.Id,
            cofre.RunId,
            cofre.Piso,
            cofre.TipoRecompensa,
            cofre.Valor,
            cofre.Abierto,
            cofre.FechaApertura
        );
}

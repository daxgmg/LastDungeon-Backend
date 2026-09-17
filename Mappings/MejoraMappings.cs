using LastDungeon.Api.DTOs.Mejoras;
using LastDungeon.Api.Models;

namespace LastDungeon.Api.Mappings;

public static class MejoraMappings
{
    public static MejoraResponse ToResponse(this Mejora mejora, bool yaComprada = false) =>
        new(
            mejora.Id,
            mejora.Nombre,
            mejora.Descripcion,
            mejora.Costo,
            mejora.TipoEfecto,
            yaComprada
        );
}

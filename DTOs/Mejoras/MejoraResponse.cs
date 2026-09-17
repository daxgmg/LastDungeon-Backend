namespace LastDungeon.Api.DTOs.Mejoras;

public record MejoraResponse(
    int Id,
    string Nombre,
    string? Descripcion,
    int Costo,
    string TipoEfecto,
    bool YaComprada
);

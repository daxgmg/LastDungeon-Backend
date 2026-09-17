using LastDungeon.Api.DTOs.Mejoras;

namespace LastDungeon.Api.Services;

public interface IMejoraService
{
    /// <summary>RF08 – Listar catálogo completo de mejoras (indica cuáles ya compró el jugador).</summary>
    Task<ServiceResult<IEnumerable<MejoraResponse>>> ListarMejorasAsync(int jugadorId);

    /// <summary>RF09 – Comprar una mejora del catálogo.</summary>
    Task<ServiceResult<MejoraResponse>> ComprarMejoraAsync(int jugadorId, ComprarMejoraRequest request);
}

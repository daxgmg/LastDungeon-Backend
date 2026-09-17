using LastDungeon.Api.DTOs.Cofres;

namespace LastDungeon.Api.Services;

public interface ICofreService
{
    /// <summary>Listar todos los cofres de una run (la run debe pertenecer al jugador).</summary>
    Task<ServiceResult<IEnumerable<CofreResponse>>> ListarCofresAsync(int runId, int jugadorId);

    /// <summary>Abrir un cofre: valida estado, aplica recompensa a la run si es tipo 'monedas'.</summary>
    Task<ServiceResult<AbrirCofreResponse>> AbrirCofreAsync(int cofreId, int jugadorId);
}

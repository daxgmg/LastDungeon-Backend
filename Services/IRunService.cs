using LastDungeon.Api.DTOs.Runs;

namespace LastDungeon.Api.Services;

public interface IRunService
{
    /// <summary>RF05 – Iniciar una nueva run para el jugador autenticado.</summary>
    Task<ServiceResult<RunResponse>> IniciarRunAsync(int jugadorId);

    /// <summary>RF06 – Registrar un combate dentro de una run activa.</summary>
    Task<ServiceResult<CombateResponse>> RegistrarCombateAsync(int runId, int jugadorId, CombateRequest request);

    /// <summary>RF07 – Finalizar una run (completada o abandonada).</summary>
    Task<ServiceResult<RunResponse>> FinalizarRunAsync(int runId, int jugadorId, FinalizarRunRequest request);
}

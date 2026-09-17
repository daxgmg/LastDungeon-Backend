using LastDungeon.Api.Data;
using LastDungeon.Api.DTOs.Cofres;
using LastDungeon.Api.Mappings;
using Microsoft.EntityFrameworkCore;

namespace LastDungeon.Api.Services;

public class CofreService : ICofreService
{
    private readonly AppDbContext _context;
    private readonly ILogger<CofreService> _logger;

    public CofreService(AppDbContext context, ILogger<CofreService> logger)
    {
        _context = context;
        _logger = logger;
    }

    /// <inheritdoc/>
    public async Task<ServiceResult<IEnumerable<CofreResponse>>> ListarCofresAsync(int runId, int jugadorId)
    {
        var run = await _context.Runs.AsNoTracking().FirstOrDefaultAsync(r => r.Id == runId);

        if (run == null)
        {
            return ServiceResult<IEnumerable<CofreResponse>>.Falla("Run no encontrada.", 404);
        }

        if (run.JugadorId != jugadorId)
        {
            return ServiceResult<IEnumerable<CofreResponse>>.Falla("No tienes acceso a esta run.", 403);
        }

        var cofres = await _context.Cofres
            .AsNoTracking()
            .Where(c => c.RunId == runId)
            .ToListAsync();

        return ServiceResult<IEnumerable<CofreResponse>>.Ok(
            cofres.Select(c => c.ToResponse()),
            "Cofres de la run obtenidos.");
    }

    /// <inheritdoc/>
    public async Task<ServiceResult<AbrirCofreResponse>> AbrirCofreAsync(int cofreId, int jugadorId)
    {
        var cofre = await _context.Cofres
            .Include(c => c.Run)
            .FirstOrDefaultAsync(c => c.Id == cofreId);

        if (cofre == null)
        {
            return ServiceResult<AbrirCofreResponse>.Falla("Cofre no encontrado.", 404);
        }

        // Verificar que la run pertenece al jugador autenticado
        if (cofre.Run.JugadorId != jugadorId)
        {
            return ServiceResult<AbrirCofreResponse>.Falla("No tienes acceso a este cofre.", 403);
        }

        if (cofre.Abierto)
        {
            return ServiceResult<AbrirCofreResponse>.Falla("Este cofre ya fue abierto.", 409);
        }

        // Marcar cofre como abierto
        cofre.Abierto = true;
        cofre.FechaApertura = DateTime.UtcNow;

        string mensaje;

        // Aplicar recompensa si es tipo monedas
        if (cofre.TipoRecompensa == "monedas")
        {
            cofre.Run.MonedasObtenidas += cofre.Valor;
            mensaje = $"¡Cofre abierto! Obtuviste {cofre.Valor} monedas.";
        }
        else
        {
            // vida u otro tipo: se notifica pero la lógica de aplicarlo queda en el cliente
            mensaje = $"¡Cofre abierto! Obtuviste {cofre.Valor} de vida.";
        }

        await _context.SaveChangesAsync();

        _logger.LogInformation(
            "Cofre {CofreId} de run {RunId} abierto por jugador {JugadorId}. Recompensa: {Tipo} x{Valor}.",
            cofreId, cofre.RunId, jugadorId, cofre.TipoRecompensa, cofre.Valor);

        var respuesta = new AbrirCofreResponse(
            cofre.Id,
            cofre.TipoRecompensa,
            cofre.Valor,
            mensaje,
            cofre.Run.MonedasObtenidas
        );

        return ServiceResult<AbrirCofreResponse>.Ok(respuesta, mensaje);
    }
}

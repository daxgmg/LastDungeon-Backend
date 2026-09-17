using LastDungeon.Api.Data;
using LastDungeon.Api.DTOs.Mejoras;
using LastDungeon.Api.Mappings;
using LastDungeon.Api.Models;
using Microsoft.EntityFrameworkCore;

namespace LastDungeon.Api.Services;

public class MejoraService : IMejoraService
{
    private readonly AppDbContext _context;
    private readonly ILogger<MejoraService> _logger;

    public MejoraService(AppDbContext context, ILogger<MejoraService> logger)
    {
        _context = context;
        _logger = logger;
    }

    /// <inheritdoc/>
    public async Task<ServiceResult<IEnumerable<MejoraResponse>>> ListarMejorasAsync(int jugadorId)
    {
        // Obtener IDs de mejoras ya compradas por el jugador
        var compradasLista = await _context.JugadorMejoras
            .Where(jm => jm.JugadorId == jugadorId)
            .Select(jm => jm.MejoraId)
            .ToListAsync();
        var compradas = compradasLista.ToHashSet();

        var mejoras = await _context.Mejoras.AsNoTracking().ToListAsync();

        var respuesta = mejoras.Select(m => m.ToResponse(compradas.Contains(m.Id)));

        return ServiceResult<IEnumerable<MejoraResponse>>.Ok(respuesta, "Catálogo de mejoras obtenido.");
    }

    /// <inheritdoc/>
    public async Task<ServiceResult<MejoraResponse>> ComprarMejoraAsync(int jugadorId, ComprarMejoraRequest request)
    {
        var jugador = await _context.Jugadores.FindAsync(jugadorId);
        if (jugador == null)
        {
            return ServiceResult<MejoraResponse>.Falla("Jugador no encontrado.", 404);
        }

        var mejora = await _context.Mejoras.FindAsync(request.MejoraId);
        if (mejora == null)
        {
            return ServiceResult<MejoraResponse>.Falla("Mejora no encontrada.", 404);
        }

        // Verificar que no la tenga ya comprada
        var yaComprada = await _context.JugadorMejoras
            .AnyAsync(jm => jm.JugadorId == jugadorId && jm.MejoraId == request.MejoraId);

        if (yaComprada)
        {
            return ServiceResult<MejoraResponse>.Falla("Ya tienes esta mejora comprada.", 409);
        }

        // Verificar saldo suficiente
        if (jugador.Monedas < mejora.Costo)
        {
            return ServiceResult<MejoraResponse>.Falla(
                $"Monedas insuficientes. Necesitas {mejora.Costo} monedas y tienes {jugador.Monedas}.", 402);
        }

        // Descontar costo y registrar compra
        jugador.Monedas -= mejora.Costo;

        var jugadorMejora = new JugadorMejora
        {
            JugadorId = jugadorId,
            MejoraId = mejora.Id,
            FechaCompra = DateTime.UtcNow
        };

        _context.JugadorMejoras.Add(jugadorMejora);
        await _context.SaveChangesAsync();

        _logger.LogInformation(
            "Jugador {JugadorId} compró mejora {MejoraId} '{Nombre}' por {Costo} monedas. Saldo restante: {Saldo}.",
            jugadorId, mejora.Id, mejora.Nombre, mejora.Costo, jugador.Monedas);

        return ServiceResult<MejoraResponse>.Creado(mejora.ToResponse(yaComprada: true), "Mejora comprada exitosamente.");
    }
}

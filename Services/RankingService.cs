using LastDungeon.Api.Data;
using LastDungeon.Api.DTOs.Ranking;
using Microsoft.EntityFrameworkCore;

namespace LastDungeon.Api.Services;

public class RankingService : IRankingService
{
    private readonly AppDbContext _context;

    public RankingService(AppDbContext context)
    {
        _context = context;
    }

    /// <inheritdoc/>
    public async Task<ServiceResult<IEnumerable<RankingEntryResponse>>> ObtenerRankingAsync()
    {
        var jugadores = await _context.Jugadores
            .AsNoTracking()
            .Where(j => j.Verificado && j.MejorPiso > 0)
            .OrderByDescending(j => j.MejorPiso)
            .ThenByDescending(j => j.Monedas)
            .Take(10)
            .ToListAsync();

        var ranking = jugadores
            .Select((j, index) => new RankingEntryResponse(
                Posicion: index + 1,
                JugadorId: j.Id,
                NombreUsuario: j.NombreUsuario,
                MejorPiso: j.MejorPiso,
                Monedas: j.Monedas
            ));

        return ServiceResult<IEnumerable<RankingEntryResponse>>.Ok(ranking, "Ranking obtenido.");
    }
}

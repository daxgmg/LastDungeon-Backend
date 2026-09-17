using LastDungeon.Api.DTOs.Ranking;

namespace LastDungeon.Api.Services;

public interface IRankingService
{
    /// <summary>RF10 – Obtener top 10 jugadores ordenados por mejor_piso descendente.</summary>
    Task<ServiceResult<IEnumerable<RankingEntryResponse>>> ObtenerRankingAsync();
}

using LastDungeon.Api.Services;

namespace LastDungeon.Api.Endpoints;

public static class RankingEndpoints
{
    public static IEndpointRouteBuilder MapRankingEndpoints(this IEndpointRouteBuilder app)
    {
        // GET /api/ranking – Top 10 jugadores por mejor_piso (RF10) – endpoint público
        app.MapGet("/api/ranking", async (IRankingService rankingService) =>
        {
            var resultado = await rankingService.ObtenerRankingAsync();

            return resultado.Exito
                ? Results.Ok(resultado.Datos)
                : Results.Json(new { mensaje = resultado.Mensaje }, statusCode: resultado.CodigoEstado);
        })
        .WithName("ObtenerRanking")
        .WithSummary("Obtener top 10 jugadores ordenados por mejor piso alcanzado (RF10)")
        .WithTags("Ranking")
        .WithOpenApi();

        return app;
    }
}

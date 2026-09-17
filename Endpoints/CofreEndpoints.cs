using System.Security.Claims;
using LastDungeon.Api.Services;

namespace LastDungeon.Api.Endpoints;

public static class CofreEndpoints
{
    public static IEndpointRouteBuilder MapCofreEndpoints(this IEndpointRouteBuilder app)
    {
        // GET /api/runs/{id}/cofres – Listar cofres de una run
        app.MapGet("/api/runs/{id:int}/cofres", async (
            int id,
            ClaimsPrincipal user,
            ICofreService cofreService) =>
        {
            var jugadorId = ObtenerJugadorId(user);
            if (jugadorId == null) return Results.Unauthorized();

            var resultado = await cofreService.ListarCofresAsync(id, jugadorId.Value);

            return resultado.Exito
                ? Results.Ok(resultado.Datos)
                : Results.Json(new { mensaje = resultado.Mensaje }, statusCode: resultado.CodigoEstado);
        })
        .WithName("ListarCofresDeRun")
        .WithSummary("Listar cofres de una run (solo accesible por el jugador dueño de la run)")
        .WithTags("Cofres")
        .RequireAuthorization()
        .WithOpenApi();

        // POST /api/cofres/{id}/abrir – Abrir un cofre
        app.MapPost("/api/cofres/{id:int}/abrir", async (
            int id,
            ClaimsPrincipal user,
            ICofreService cofreService) =>
        {
            var jugadorId = ObtenerJugadorId(user);
            if (jugadorId == null) return Results.Unauthorized();

            var resultado = await cofreService.AbrirCofreAsync(id, jugadorId.Value);

            return resultado.Exito
                ? Results.Ok(resultado.Datos)
                : Results.Json(new { mensaje = resultado.Mensaje }, statusCode: resultado.CodigoEstado);
        })
        .WithName("AbrirCofre")
        .WithSummary("Abrir un cofre de una run activa y aplicar recompensa")
        .WithTags("Cofres")
        .RequireAuthorization()
        .WithOpenApi();

        return app;
    }

    private static int? ObtenerJugadorId(ClaimsPrincipal user)
    {
        var claim = user.FindFirst(ClaimTypes.NameIdentifier)?.Value
                    ?? user.FindFirst("id")?.Value;
        return int.TryParse(claim, out var id) ? id : null;
    }
}

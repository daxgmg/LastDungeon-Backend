using System.Security.Claims;
using LastDungeon.Api.DTOs.Mejoras;
using LastDungeon.Api.Services;
using Microsoft.AspNetCore.Mvc;

namespace LastDungeon.Api.Endpoints;

public static class MejoraEndpoints
{
    public static IEndpointRouteBuilder MapMejoraEndpoints(this IEndpointRouteBuilder app)
    {
        // GET /api/mejoras – Listar catálogo (RF08) – requiere autenticación para saber cuáles ya compró el jugador
        app.MapGet("/api/mejoras", async (
            ClaimsPrincipal user,
            IMejoraService mejoraService) =>
        {
            var jugadorId = ObtenerJugadorId(user);
            if (jugadorId == null) return Results.Unauthorized();

            var resultado = await mejoraService.ListarMejorasAsync(jugadorId.Value);

            return resultado.Exito
                ? Results.Ok(resultado.Datos)
                : Results.Json(new { mensaje = resultado.Mensaje }, statusCode: resultado.CodigoEstado);
        })
        .WithName("ListarMejoras")
        .WithSummary("Listar catálogo de mejoras disponibles (RF08)")
        .WithTags("Mejoras")
        .RequireAuthorization()
        .WithOpenApi();

        // POST /api/jugadores/{id}/mejoras – Comprar mejora (RF09)
        app.MapPost("/api/jugadores/{id:int}/mejoras", async (
            int id,
            [FromBody] ComprarMejoraRequest request,
            ClaimsPrincipal user,
            IMejoraService mejoraService) =>
        {
            var jugadorId = ObtenerJugadorId(user);
            if (jugadorId == null) return Results.Unauthorized();

            // Verificar que el jugador del token coincide con el id en la ruta
            if (jugadorId.Value != id)
            {
                return Results.Json(new { mensaje = "No puedes comprar mejoras para otro jugador." }, statusCode: 403);
            }

            var resultado = await mejoraService.ComprarMejoraAsync(jugadorId.Value, request);

            return resultado.Exito
                ? Results.Json(resultado.Datos, statusCode: resultado.CodigoEstado)
                : Results.Json(new { mensaje = resultado.Mensaje }, statusCode: resultado.CodigoEstado);
        })
        .WithName("ComprarMejora")
        .WithSummary("Comprar una mejora del catálogo para el jugador autenticado (RF09)")
        .WithTags("Mejoras")
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

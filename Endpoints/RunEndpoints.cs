using System.Security.Claims;
using LastDungeon.Api.DTOs.Runs;
using LastDungeon.Api.Services;
using Microsoft.AspNetCore.Mvc;

namespace LastDungeon.Api.Endpoints;

public static class RunEndpoints
{
    public static IEndpointRouteBuilder MapRunEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/runs")
            .WithTags("Runs")
            .RequireAuthorization();

        // POST /api/runs – Iniciar run (RF05)
        group.MapPost("/", async (
            ClaimsPrincipal user,
            IRunService runService) =>
        {
            var jugadorId = ObtenerJugadorId(user);
            if (jugadorId == null) return Results.Unauthorized();

            var resultado = await runService.IniciarRunAsync(jugadorId.Value);

            return resultado.Exito
                ? Results.Json(resultado.Datos, statusCode: resultado.CodigoEstado)
                : Results.Json(new { mensaje = resultado.Mensaje }, statusCode: resultado.CodigoEstado);
        })
        .WithName("IniciarRun")
        .WithSummary("Iniciar una nueva run para el jugador autenticado (RF05)")
        .WithOpenApi();

        // PATCH /api/runs/{id}/combate – Registrar combate (RF06)
        group.MapMethods("/{id:int}/combate", ["PATCH"], async (
            int id,
            [FromBody] CombateRequest request,
            ClaimsPrincipal user,
            IRunService runService) =>
        {
            var jugadorId = ObtenerJugadorId(user);
            if (jugadorId == null) return Results.Unauthorized();

            var resultado = await runService.RegistrarCombateAsync(id, jugadorId.Value, request);

            return resultado.Exito
                ? Results.Json(resultado.Datos, statusCode: resultado.CodigoEstado)
                : Results.Json(new { mensaje = resultado.Mensaje }, statusCode: resultado.CodigoEstado);
        })
        .WithName("RegistrarCombate")
        .WithSummary("Registrar un combate dentro de una run activa (RF06)")
        .WithOpenApi();

        // PUT /api/runs/{id} – Finalizar run (RF07)
        group.MapPut("/{id:int}", async (
            int id,
            [FromBody] FinalizarRunRequest request,
            ClaimsPrincipal user,
            IRunService runService) =>
        {
            var jugadorId = ObtenerJugadorId(user);
            if (jugadorId == null) return Results.Unauthorized();

            var resultado = await runService.FinalizarRunAsync(id, jugadorId.Value, request);

            return resultado.Exito
                ? Results.Json(resultado.Datos, statusCode: resultado.CodigoEstado)
                : Results.Json(new { mensaje = resultado.Mensaje }, statusCode: resultado.CodigoEstado);
        })
        .WithName("FinalizarRun")
        .WithSummary("Finalizar una run (completada o abandonada) (RF07)")
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

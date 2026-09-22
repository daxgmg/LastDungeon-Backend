using System.Security.Claims;
using LastDungeon.Api.Data;
using LastDungeon.Api.Models;
using Microsoft.EntityFrameworkCore;

namespace LastDungeon.Api.Endpoints;

public static class CofrePermanenteEndpoints
{
    public static IEndpointRouteBuilder MapCofrePermanenteEndpoints(this IEndpointRouteBuilder app)
    {
        // GET /api/jugadores/{id}/cofres-abiertos
        app.MapGet("/api/jugadores/{id:int}/cofres-abiertos", async (
            int id,
            ClaimsPrincipal user,
            AppDbContext db) =>
        {
            var jugadorId = ObtenerJugadorId(user);
            if (jugadorId == null) return Results.Unauthorized();

            if (jugadorId.Value != id)
                return Results.Json(new { mensaje = "No puedes consultar los cofres de otro jugador." }, statusCode: 403);

            var claves = await db.CofresAbiertos
                .Where(c => c.JugadorId == id)
                .Select(c => c.CofreClave)
                .ToListAsync();

            return Results.Ok(claves);
        })
        .WithName("ObtenerCofresAbiertos")
        .WithSummary("Obtener las claves de los cofres permanentes ya abiertos por el jugador")
        .WithTags("CofresPermantentes")
        .RequireAuthorization()
        .WithOpenApi();

        // POST /api/jugadores/{id}/cofres-abiertos
        app.MapPost("/api/jugadores/{id:int}/cofres-abiertos", async (
            int id,
            AbrirCofrePermanenteRequest body,
            ClaimsPrincipal user,
            AppDbContext db) =>
        {
            var jugadorId = ObtenerJugadorId(user);
            if (jugadorId == null) return Results.Unauthorized();

            if (jugadorId.Value != id)
                return Results.Json(new { mensaje = "No puedes abrir cofres de otro jugador." }, statusCode: 403);

            var yaExiste = await db.CofresAbiertos
                .AnyAsync(c => c.JugadorId == id && c.CofreClave == body.CofreClave);

            if (yaExiste)
                return Results.Ok(new { mensaje = "El cofre ya estaba abierto." });

            var registro = new CofreAbierto
            {
                JugadorId = id,
                CofreClave = body.CofreClave,
                FechaApertura = DateTime.UtcNow
            };

            db.CofresAbiertos.Add(registro);
            await db.SaveChangesAsync();

            return Results.Created($"/api/jugadores/{id}/cofres-abiertos", new { cofreClave = registro.CofreClave });
        })
        .WithName("AbrirCofrePermanente")
        .WithSummary("Registrar la apertura de un cofre permanente para el jugador")
        .WithTags("CofresPermantentes")
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

public record AbrirCofrePermanenteRequest(string CofreClave);

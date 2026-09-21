using System.Security.Claims;
using LastDungeon.Api.Data;
using LastDungeon.Api.Mappings;

namespace LastDungeon.Api.Endpoints;

public static class JugadorEndpoints
{
    public static IEndpointRouteBuilder MapJugadorEndpoints(this IEndpointRouteBuilder app)
    {
        app.MapGet("/api/jugadores/{id:int}", async (
            int id,
            ClaimsPrincipal user,
            AppDbContext db) =>
        {
            var jugadorId = ObtenerJugadorId(user);
            if (jugadorId == null) return Results.Unauthorized();

            if (jugadorId.Value != id)
                return Results.Json(new { mensaje = "No puedes consultar el perfil de otro jugador." }, statusCode: 403);

            var jugador = await db.Jugadores.FindAsync(id);
            if (jugador is null)
                return Results.NotFound(new { mensaje = "Jugador no encontrado." });

            return Results.Ok(jugador.ToResponse());
        })
        .WithName("ObtenerPerfilJugador")
        .WithSummary("Consultar el perfil del jugador autenticado (RF04)")
        .WithTags("Jugadores")
        .RequireAuthorization()
        .WithOpenApi();

        app.MapPost("/api/jugadores/{id:int}/desbloquear-botas", async (
            int id,
            ClaimsPrincipal user,
            AppDbContext db) =>
        {
            var jugadorId = ObtenerJugadorId(user);
            if (jugadorId == null) return Results.Unauthorized();

            if (jugadorId.Value != id)
                return Results.Json(new { mensaje = "No puedes desbloquear las botas de otro jugador." }, statusCode: 403);

            var jugador = await db.Jugadores.FindAsync(id);
            if (jugador is null)
                return Results.NotFound(new { mensaje = "Jugador no encontrado." });

            jugador.TieneBotas = true;
            await db.SaveChangesAsync();

            return Results.Ok(jugador.ToResponse());
        })
        .WithName("DesbloquearBotas")
        .WithSummary("Desbloquear las Botas del Doble Salto para el jugador autenticado")
        .WithTags("Jugadores")
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
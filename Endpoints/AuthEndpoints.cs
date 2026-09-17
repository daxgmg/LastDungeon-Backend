using LastDungeon.Api.DTOs.Auth;
using LastDungeon.Api.Services;
using Microsoft.AspNetCore.Mvc;

namespace LastDungeon.Api.Endpoints;

public static class AuthEndpoints
{
    public static IEndpointRouteBuilder MapAuthEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/auth")
            .WithTags("Autenticación");

        group.MapPost("/registro", async (
            [FromBody] RegistroRequest request,
            IAuthService authService) =>
        {
            var resultado = await authService.RegistrarAsync(request);
            if (!resultado.Exito)
            {
                return Results.Json(new { mensaje = resultado.Mensaje }, statusCode: resultado.CodigoEstado);
            }

            return Results.Ok(new
            {
                mensaje = resultado.Mensaje,
                correo = resultado.Datos
            });
        })
        .WithName("RegistrarJugador")
        .WithSummary("Registrar un nuevo jugador o solicitar código si está pendiente de verificación")
        .WithOpenApi();

        group.MapPost("/login", async (
            [FromBody] LoginRequest request,
            IAuthService authService) =>
        {
            var resultado = await authService.LoginAsync(request);
            if (!resultado.Exito)
            {
                return Results.Json(new { mensaje = resultado.Mensaje }, statusCode: resultado.CodigoEstado);
            }

            return Results.Ok(new
            {
                mensaje = resultado.Mensaje,
                correo = resultado.Datos
            });
        })
        .WithName("IniciarSesion")
        .WithSummary("Solicitar código de verificación para inicio de sesión")
        .WithOpenApi();

        group.MapPost("/verificar", async (
            [FromBody] VerificarRequest request,
            IAuthService authService) =>
        {
            var resultado = await authService.VerificarCodigoAsync(request);
            if (!resultado.Exito)
            {
                return Results.Json(new { mensaje = resultado.Mensaje }, statusCode: resultado.CodigoEstado);
            }

            return Results.Ok(resultado.Datos);
        })
        .WithName("VerificarCodigo")
        .WithSummary("Verificar código de 6 dígitos y obtener token JWT de autenticación")
        .WithOpenApi();

        return app;
    }
}

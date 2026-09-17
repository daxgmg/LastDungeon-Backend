using System.Security.Cryptography;
using LastDungeon.Api.Data;
using LastDungeon.Api.DTOs.Auth;
using LastDungeon.Api.Mappings;
using LastDungeon.Api.Models;
using Microsoft.EntityFrameworkCore;

namespace LastDungeon.Api.Services;

public class AuthService : IAuthService
{
    private readonly AppDbContext _context;
    private readonly IEmailService _emailService;
    private readonly IJwtService _jwtService;
    private readonly ILogger<AuthService> _logger;

    public AuthService(
        AppDbContext context,
        IEmailService emailService,
        IJwtService jwtService,
        ILogger<AuthService> logger)
    {
        _context = context;
        _emailService = emailService;
        _jwtService = jwtService;
        _logger = logger;
    }

    public async Task<AuthResult<string>> RegistrarAsync(RegistroRequest request)
    {
        var correoNormalizado = request.Correo.Trim().ToLowerInvariant();
        var jugador = await _context.Jugadores
            .FirstOrDefaultAsync(j => j.Correo.ToLower() == correoNormalizado);

        if (jugador != null && jugador.Verificado)
        {
            return AuthResult<string>.Falla(
                "El correo ya se encuentra registrado y verificado. Por favor inicia sesión.",
                409);
        }

        if (jugador == null)
        {
            jugador = new Jugador
            {
                Correo = correoNormalizado,
                NombreUsuario = request.NombreUsuario.Trim(),
                Verificado = false,
                Monedas = 0,
                MejorPiso = 0,
                FechaRegistro = DateTime.UtcNow
            };
            _context.Jugadores.Add(jugador);
            await _context.SaveChangesAsync();
        }
        else
        {
            // Actualizar nombre de usuario si estaba pendiente de verificación
            jugador.NombreUsuario = request.NombreUsuario.Trim();
            await _context.SaveChangesAsync();
        }

        var codigo = GenerarCodigoVerificacion();
        var codigoEntidad = new CodigoVerificacion
        {
            JugadorId = jugador.Id,
            Codigo = codigo,
            ExpiraEn = DateTime.UtcNow.AddMinutes(10),
            Usado = false,
            CreadoEn = DateTime.UtcNow
        };

        _context.CodigosVerificacion.Add(codigoEntidad);
        await _context.SaveChangesAsync();

        await _emailService.SendVerificationCodeAsync(jugador.Correo, codigo);

        return AuthResult<string>.Ok(
            jugador.Correo,
            "Código de verificación generado y enviado al correo electrónico (válido por 10 minutos).");
    }

    public async Task<AuthResult<string>> LoginAsync(LoginRequest request)
    {
        var correoNormalizado = request.Correo.Trim().ToLowerInvariant();
        var jugador = await _context.Jugadores
            .FirstOrDefaultAsync(j => j.Correo.ToLower() == correoNormalizado);

        if (jugador == null)
        {
            return AuthResult<string>.Falla(
                "No existe ninguna cuenta registrada con este correo electrónico.",
                404);
        }

        if (!jugador.Verificado)
        {
            return AuthResult<string>.Falla(
                "La cuenta aún no ha sido verificada. Por favor completa el registro primero.",
                400);
        }

        var codigo = GenerarCodigoVerificacion();
        var codigoEntidad = new CodigoVerificacion
        {
            JugadorId = jugador.Id,
            Codigo = codigo,
            ExpiraEn = DateTime.UtcNow.AddMinutes(10),
            Usado = false,
            CreadoEn = DateTime.UtcNow
        };

        _context.CodigosVerificacion.Add(codigoEntidad);
        await _context.SaveChangesAsync();

        await _emailService.SendVerificationCodeAsync(jugador.Correo, codigo);

        return AuthResult<string>.Ok(
            jugador.Correo,
            "Código de inicio de sesión enviado al correo electrónico (válido por 10 minutos).");
    }

    public async Task<AuthResult<TokenResponse>> VerificarCodigoAsync(VerificarRequest request)
    {
        var correoNormalizado = request.Correo.Trim().ToLowerInvariant();
        var jugador = await _context.Jugadores
            .FirstOrDefaultAsync(j => j.Correo.ToLower() == correoNormalizado);

        if (jugador == null)
        {
            return AuthResult<TokenResponse>.Falla(
                "Jugador no encontrado para el correo proporcionado.",
                404);
        }

        var codigoRegistro = await _context.CodigosVerificacion
            .Where(c => c.JugadorId == jugador.Id && c.Codigo == request.Codigo && !c.Usado)
            .OrderByDescending(c => c.CreadoEn)
            .FirstOrDefaultAsync();

        if (codigoRegistro == null)
        {
            return AuthResult<TokenResponse>.Falla(
                "El código de verificación proporcionado es incorrecto o ya fue utilizado.",
                400);
        }

        if (codigoRegistro.ExpiraEn < DateTime.UtcNow)
        {
            return AuthResult<TokenResponse>.Falla(
                "El código de verificación ha expirado. Por favor solicita uno nuevo.",
                400);
        }

        // Marcar código como usado y jugador como verificado
        codigoRegistro.Usado = true;
        jugador.Verificado = true;
        await _context.SaveChangesAsync();

        // Generar JWT
        var (token, expiraEn) = _jwtService.GenerateToken(jugador);

        var respuesta = new TokenResponse
        {
            Token = token,
            Tipo = "Bearer",
            ExpiraEn = expiraEn,
            Jugador = jugador.ToResponse()
        };

        return AuthResult<TokenResponse>.Ok(respuesta, "Autenticación completada con éxito.");
    }

    private static string GenerarCodigoVerificacion()
    {
        var numero = RandomNumberGenerator.GetInt32(100000, 1000000);
        return numero.ToString("D6");
    }
}

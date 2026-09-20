using LastDungeon.Api.Data;
using LastDungeon.Api.DTOs.Runs;
using LastDungeon.Api.Mappings;
using LastDungeon.Api.Models;
using Microsoft.EntityFrameworkCore;

namespace LastDungeon.Api.Services;

public class RunService : IRunService
{
    private readonly AppDbContext _context;
    private readonly ILogger<RunService> _logger;

    private static readonly string[] TiposRecompensa = { "monedas", "vida" };

    public RunService(AppDbContext context, ILogger<RunService> logger)
    {
        _context = context;
        _logger = logger;
    }

    /// <inheritdoc/>
    public async Task<ServiceResult<RunResponse>> IniciarRunAsync(int jugadorId)
    {
        var jugador = await _context.Jugadores.FindAsync(jugadorId);
        if (jugador == null)
        {
            return ServiceResult<RunResponse>.Falla("Jugador no encontrado.", 404);
        }

        // Generar semilla aleatoria
        var semilla = Guid.NewGuid().ToString("N")[..16];

        var run = new Run
        {
            JugadorId = jugadorId,
            Semilla = semilla,
            PisoAlcanzado = 1,
            Estado = "activa",
            MonedasObtenidas = 0,
            FechaInicio = DateTime.UtcNow
        };

        _context.Runs.Add(run);
        await _context.SaveChangesAsync();

        // Generar entre 3 y 5 cofres con recompensas aleatorias
        var rng = new Random();
        var cantidadCofres = rng.Next(3, 6); // 3, 4 o 5
        for (int i = 0; i < cantidadCofres; i++)
        {
            var tipo = TiposRecompensa[rng.Next(TiposRecompensa.Length)];
            var valor = tipo == "monedas"
                ? rng.Next(10, 101)  // 10-100 monedas
                : rng.Next(5, 31);   // 5-30 de vida

            var cofre = new Cofre
            {
                RunId = run.Id,
                Piso = rng.Next(1, 11), // piso aleatorio 1-10
                TipoRecompensa = tipo,
                Valor = valor,
                Abierto = false
            };
            _context.Cofres.Add(cofre);
        }

        await _context.SaveChangesAsync();

        _logger.LogInformation("Run {RunId} iniciada para jugador {JugadorId} con semilla {Semilla} y {CantidadCofres} cofres.",
            run.Id, jugadorId, semilla, cantidadCofres);

        return ServiceResult<RunResponse>.Creado(run.ToResponse(), "Run iniciada correctamente.");
    }

    /// <inheritdoc/>
    public async Task<ServiceResult<CombateResponse>> RegistrarCombateAsync(
        int runId, int jugadorId, CombateRequest request)
    {
        var run = await _context.Runs.FindAsync(runId);

        if (run == null)
        {
            return ServiceResult<CombateResponse>.Falla("Run no encontrada.", 404);
        }

        if (run.JugadorId != jugadorId)
        {
            return ServiceResult<CombateResponse>.Falla("No tienes acceso a esta run.", 403);
        }

        if (run.Estado != "activa")
        {
            return ServiceResult<CombateResponse>.Falla(
                $"No se puede registrar un combate en una run con estado '{run.Estado}'.", 409);
        }

        var combate = new Combate
        {
            RunId = runId,
            Enemigo = request.Enemigo,
            Resultado = request.Resultado,
            HpRestante = request.HpRestante,
            MonedasDropeadas = request.MonedasDropeadas,
            VidaDropeada = request.VidaDropeada,
            Fecha = DateTime.UtcNow
        };

        _context.Combates.Add(combate);

        // Sumar monedas dropeadas al total acumulado de la run
        run.MonedasObtenidas += request.MonedasDropeadas;

        await _context.SaveChangesAsync();

        _logger.LogInformation("Combate {CombateId} registrado en run {RunId}. Monedas acumuladas: {Monedas}.",
            combate.Id, runId, run.MonedasObtenidas);

        return ServiceResult<CombateResponse>.Creado(combate.ToResponse(), "Combate registrado correctamente.");
    }

    /// <inheritdoc/>
    public async Task<ServiceResult<RunResponse>> FinalizarRunAsync(
        int runId, int jugadorId, FinalizarRunRequest request)
    {
        var estadosValidos = new[] { "finalizada", "muerto" };
        if (!estadosValidos.Contains(request.Estado))
        {
            return ServiceResult<RunResponse>.Falla(
                "El estado debe ser 'finalizada' o 'muerto'.", 422);
        }

        var run = await _context.Runs.FindAsync(runId);

        if (run == null)
        {
            return ServiceResult<RunResponse>.Falla("Run no encontrada.", 404);
        }

        if (run.JugadorId != jugadorId)
        {
            return ServiceResult<RunResponse>.Falla("No tienes acceso a esta run.", 403);
        }

        if (run.Estado != "activa")
        {
            return ServiceResult<RunResponse>.Falla(
                $"La run ya se encuentra en estado '{run.Estado}'.", 409);
        }

        var jugador = await _context.Jugadores.FindAsync(jugadorId);
        if (jugador == null)
        {
            return ServiceResult<RunResponse>.Falla("Jugador no encontrado.", 404);
        }

        // Actualizar estado y piso alcanzado de la run
        run.Estado = request.Estado;
        run.PisoAlcanzado = request.PisoAlcanzado > 0 ? request.PisoAlcanzado : run.PisoAlcanzado;
        run.FechaFin = DateTime.UtcNow;

        // Transferir monedas acumuladas de la run al total permanente del jugador solo si la run fue finalizada con éxito
        if (request.Estado == "finalizada")
        {
            jugador.Monedas += run.MonedasObtenidas;
        }

        // Actualizar mejor piso si corresponde
        if (run.PisoAlcanzado > jugador.MejorPiso)
        {
            jugador.MejorPiso = run.PisoAlcanzado;
        }

        await _context.SaveChangesAsync();

        _logger.LogInformation(
            "Run {RunId} finalizada con estado '{Estado}'. Jugador {JugadorId}: +{Monedas} monedas, mejor piso {MejorPiso}.",
            run.Id, run.Estado, jugadorId, request.Estado == "finalizada" ? run.MonedasObtenidas : 0, jugador.MejorPiso);

        return ServiceResult<RunResponse>.Ok(run.ToResponse(), "Run finalizada correctamente.");
    }
}

using LastDungeon.Api.Models;
using Microsoft.EntityFrameworkCore;

namespace LastDungeon.Api.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
    }

    public DbSet<Jugador> Jugadores => Set<Jugador>();
    public DbSet<CodigoVerificacion> CodigosVerificacion => Set<CodigoVerificacion>();
    public DbSet<Run> Runs => Set<Run>();
    public DbSet<Combate> Combates => Set<Combate>();
    public DbSet<Mejora> Mejoras => Set<Mejora>();
    public DbSet<JugadorMejora> JugadorMejoras => Set<JugadorMejora>();
    public DbSet<Cofre> Cofres => Set<Cofre>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Jugador configuration
        modelBuilder.Entity<Jugador>(entity =>
        {
            entity.HasIndex(e => e.Correo)
                .IsUnique();
        });

        // CodigoVerificacion configuration
        modelBuilder.Entity<CodigoVerificacion>(entity =>
        {
            entity.HasOne(e => e.Jugador)
                .WithMany(j => j.CodigosVerificacion)
                .HasForeignKey(e => e.JugadorId)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("fk_codigo_jugador");
        });

        // Run configuration
        modelBuilder.Entity<Run>(entity =>
        {
            entity.HasOne(e => e.Jugador)
                .WithMany(j => j.Runs)
                .HasForeignKey(e => e.JugadorId)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("fk_run_jugador");
        });

        // Combate configuration
        modelBuilder.Entity<Combate>(entity =>
        {
            entity.HasOne(e => e.Run)
                .WithMany(r => r.Combates)
                .HasForeignKey(e => e.RunId)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("fk_combate_run");
        });

        // JugadorMejora configuration
        modelBuilder.Entity<JugadorMejora>(entity =>
        {
            entity.HasIndex(e => new { e.JugadorId, e.MejoraId })
                .IsUnique()
                .HasDatabaseName("uq_jugador_mejora");

            entity.HasOne(e => e.Jugador)
                .WithMany(j => j.JugadorMejoras)
                .HasForeignKey(e => e.JugadorId)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("fk_jm_jugador");

            entity.HasOne(e => e.Mejora)
                .WithMany(m => m.JugadorMejoras)
                .HasForeignKey(e => e.MejoraId)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("fk_jm_mejora");
        });

        // Cofre configuration
        modelBuilder.Entity<Cofre>(entity =>
        {
            entity.HasOne(e => e.Run)
                .WithMany(r => r.Cofres)
                .HasForeignKey(e => e.RunId)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("fk_cofre_run");
        });
    }
}

using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LastDungeon.Api.Models;

[Table("jugador_mejoras")]
public class JugadorMejora
{
    [Key]
    [Column("id")]
    public int Id { get; set; }

    [Column("jugador_id")]
    public int JugadorId { get; set; }

    [Column("mejora_id")]
    public int MejoraId { get; set; }

    [Column("fecha_compra")]
    public DateTime FechaCompra { get; set; } = DateTime.UtcNow;

    // Relaciones
    [ForeignKey(nameof(JugadorId))]
    public Jugador Jugador { get; set; } = null!;

    [ForeignKey(nameof(MejoraId))]
    public Mejora Mejora { get; set; } = null!;
}

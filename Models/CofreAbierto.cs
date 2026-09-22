using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LastDungeon.Api.Models;

[Table("cofres_abiertos")]
public class CofreAbierto
{
    [Key]
    [Column("id")]
    public int Id { get; set; }

    [Column("jugador_id")]
    public int JugadorId { get; set; }

    [Required]
    [MaxLength(200)]
    [Column("cofre_clave")]
    public string CofreClave { get; set; } = string.Empty;

    [Column("fecha_apertura")]
    public DateTime FechaApertura { get; set; } = DateTime.UtcNow;

    // Relaciones
    public Jugador Jugador { get; set; } = null!;
}

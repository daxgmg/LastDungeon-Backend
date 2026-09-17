using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LastDungeon.Api.Models;

[Table("runs")]
public class Run
{
    [Key]
    [Column("id")]
    public int Id { get; set; }

    [Column("jugador_id")]
    public int JugadorId { get; set; }

    [Required]
    [MaxLength(50)]
    [Column("semilla")]
    public string Semilla { get; set; } = string.Empty;

    [Column("piso_alcanzado")]
    public int PisoAlcanzado { get; set; } = 1;

    [Required]
    [MaxLength(20)]
    [Column("estado")]
    public string Estado { get; set; } = "activa";

    [Column("monedas_obtenidas")]
    public int MonedasObtenidas { get; set; } = 0;

    [Column("fecha_inicio")]
    public DateTime FechaInicio { get; set; } = DateTime.UtcNow;

    [Column("fecha_fin")]
    public DateTime? FechaFin { get; set; }

    // Relaciones
    [ForeignKey(nameof(JugadorId))]
    public Jugador Jugador { get; set; } = null!;

    public ICollection<Combate> Combates { get; set; } = new List<Combate>();
    public ICollection<Cofre> Cofres { get; set; } = new List<Cofre>();
}

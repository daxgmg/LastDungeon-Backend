using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LastDungeon.Api.Models;

[Table("combates")]
public class Combate
{
    [Key]
    [Column("id")]
    public int Id { get; set; }

    [Column("run_id")]
    public int RunId { get; set; }

    [Required]
    [MaxLength(80)]
    [Column("enemigo")]
    public string Enemigo { get; set; } = string.Empty;

    [Column("hp_restante")]
    public int HpRestante { get; set; }

    [Required]
    [MaxLength(20)]
    [Column("resultado")]
    public string Resultado { get; set; } = string.Empty;

    [Column("fecha")]
    public DateTime Fecha { get; set; } = DateTime.UtcNow;

    [Column("monedas_dropeadas")]
    public int MonedasDropeadas { get; set; } = 0;

    [Column("vida_dropeada")]
    public int VidaDropeada { get; set; } = 0;

    // Relaciones
    [ForeignKey(nameof(RunId))]
    public Run Run { get; set; } = null!;
}

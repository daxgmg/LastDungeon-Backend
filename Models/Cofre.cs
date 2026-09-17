using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LastDungeon.Api.Models;

[Table("cofres")]
public class Cofre
{
    [Key]
    [Column("id")]
    public int Id { get; set; }

    [Column("run_id")]
    public int RunId { get; set; }

    [Column("piso")]
    public int Piso { get; set; } = 1;

    [Required]
    [MaxLength(20)]
    [Column("tipo_recompensa")]
    public string TipoRecompensa { get; set; } = string.Empty;

    [Column("valor")]
    public int Valor { get; set; }

    [Column("abierto")]
    public bool Abierto { get; set; } = false;

    [Column("fecha_apertura")]
    public DateTime? FechaApertura { get; set; }

    // Relaciones
    [ForeignKey(nameof(RunId))]
    public Run Run { get; set; } = null!;
}

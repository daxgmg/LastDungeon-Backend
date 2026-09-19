using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LastDungeon.Api.Models;

[Table("mejoras")]
public class Mejora
{
    [Key]
    [Column("id")]
    public int Id { get; set; }

    [Required]
    [MaxLength(80)]
    [Column("nombre")]
    public string Nombre { get; set; } = string.Empty;

    [MaxLength(200)]
    [Column("descripcion")]
    public string? Descripcion { get; set; }

    [Column("costo")]
    public int Costo { get; set; }

    [Required]
    [MaxLength(50)]
    [Column("tipo_efecto")]
    public string TipoEfecto { get; set; } = string.Empty;

    [Column("valor_efecto")]
    public int ValorEfecto { get; set; }

    [Column("orden")]
    public int Orden { get; set; }

    // Relaciones
    public ICollection<JugadorMejora> JugadorMejoras { get; set; } = new List<JugadorMejora>();
}

using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LastDungeon.Api.Models;

[Table("codigos_verificacion")]
public class CodigoVerificacion
{
    [Key]
    [Column("id")]
    public int Id { get; set; }

    [Column("jugador_id")]
    public int JugadorId { get; set; }

    [Required]
    [MaxLength(6)]
    [Column("codigo")]
    public string Codigo { get; set; } = string.Empty;

    [Column("expira_en")]
    public DateTime ExpiraEn { get; set; }

    [Column("usado")]
    public bool Usado { get; set; } = false;

    [Column("creado_en")]
    public DateTime CreadoEn { get; set; } = DateTime.UtcNow;

    // Relaciones
    [ForeignKey(nameof(JugadorId))]
    public Jugador Jugador { get; set; } = null!;
}

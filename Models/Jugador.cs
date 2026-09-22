using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LastDungeon.Api.Models;

[Table("jugadores")]
public class Jugador
{
    [Key]
    [Column("id")]
    public int Id { get; set; }

    [Required]
    [MaxLength(150)]
    [Column("correo")]
    public string Correo { get; set; } = string.Empty;

    [Required]
    [MaxLength(50)]
    [Column("nombre_usuario")]
    public string NombreUsuario { get; set; } = string.Empty;

    [Column("verificado")]
    public bool Verificado { get; set; } = false;

    [Column("monedas")]
    public int Monedas { get; set; } = 0;

    [Column("mejor_piso")]
    public int MejorPiso { get; set; } = 0;

    [Column("fecha_registro")]
    public DateTime FechaRegistro { get; set; } = DateTime.UtcNow;

    [Column("tiene_botas")]
    public bool TieneBotas { get; set; } = false;

    [Column("botas_equipadas")]
    public bool BotasEquipadas { get; set; } = false;

    // Relaciones
    public ICollection<CodigoVerificacion> CodigosVerificacion { get; set; } = new List<CodigoVerificacion>();
    public ICollection<Run> Runs { get; set; } = new List<Run>();
    public ICollection<JugadorMejora> JugadorMejoras { get; set; } = new List<JugadorMejora>();
}

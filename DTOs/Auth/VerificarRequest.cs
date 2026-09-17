using System.ComponentModel.DataAnnotations;

namespace LastDungeon.Api.DTOs.Auth;

public class VerificarRequest
{
    [Required(ErrorMessage = "El correo electrónico es requerido.")]
    [EmailAddress(ErrorMessage = "El formato de correo no es válido.")]
    public string Correo { get; set; } = string.Empty;

    [Required(ErrorMessage = "El código de verificación es requerido.")]
    [StringLength(6, MinimumLength = 6, ErrorMessage = "El código debe tener exactamente 6 dígitos.")]
    public string Codigo { get; set; } = string.Empty;
}

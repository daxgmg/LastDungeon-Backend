using System.ComponentModel.DataAnnotations;

namespace LastDungeon.Api.DTOs.Auth;

public class RegistroRequest
{
    [Required(ErrorMessage = "El correo electrónico es requerido.")]
    [EmailAddress(ErrorMessage = "El formato de correo no es válido.")]
    public string Correo { get; set; } = string.Empty;

    [Required(ErrorMessage = "El nombre de usuario es requerido.")]
    [MaxLength(50, ErrorMessage = "El nombre de usuario no puede exceder los 50 caracteres.")]
    public string NombreUsuario { get; set; } = string.Empty;
}

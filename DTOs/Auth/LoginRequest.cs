using System.ComponentModel.DataAnnotations;

namespace LastDungeon.Api.DTOs.Auth;

public class LoginRequest
{
    [Required(ErrorMessage = "El correo electrónico es requerido.")]
    [EmailAddress(ErrorMessage = "El formato de correo no es válido.")]
    public string Correo { get; set; } = string.Empty;
}

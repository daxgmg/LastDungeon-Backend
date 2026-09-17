using LastDungeon.Api.DTOs.Auth;

namespace LastDungeon.Api.Services;

public class AuthResult<T>
{
    public bool Exito { get; set; }
    public string Mensaje { get; set; } = string.Empty;
    public T? Datos { get; set; }
    public int CodigoEstado { get; set; } = 200;

    public static AuthResult<T> Ok(T datos, string mensaje = "Operación exitosa") =>
        new() { Exito = true, Datos = datos, Mensaje = mensaje, CodigoEstado = 200 };

    public static AuthResult<T> Falla(string mensaje, int codigoEstado = 400) =>
        new() { Exito = false, Mensaje = mensaje, CodigoEstado = codigoEstado };
}

public interface IAuthService
{
    Task<AuthResult<string>> RegistrarAsync(RegistroRequest request);
    Task<AuthResult<string>> LoginAsync(LoginRequest request);
    Task<AuthResult<TokenResponse>> VerificarCodigoAsync(VerificarRequest request);
}

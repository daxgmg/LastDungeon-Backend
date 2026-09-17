namespace LastDungeon.Api.Services;

/// <summary>
/// Resultado genérico de operación de servicio, similar a AuthResult pero reutilizable en todos los servicios.
/// </summary>
public class ServiceResult<T>
{
    public bool Exito { get; set; }
    public string Mensaje { get; set; } = string.Empty;
    public T? Datos { get; set; }
    public int CodigoEstado { get; set; } = 200;

    public static ServiceResult<T> Ok(T datos, string mensaje = "Operación exitosa") =>
        new() { Exito = true, Datos = datos, Mensaje = mensaje, CodigoEstado = 200 };

    public static ServiceResult<T> Creado(T datos, string mensaje = "Recurso creado exitosamente") =>
        new() { Exito = true, Datos = datos, Mensaje = mensaje, CodigoEstado = 201 };

    public static ServiceResult<T> Falla(string mensaje, int codigoEstado = 400) =>
        new() { Exito = false, Mensaje = mensaje, CodigoEstado = codigoEstado };
}

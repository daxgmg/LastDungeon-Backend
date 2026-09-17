# LastDungeon API

Backend Web API para el juego **LastDungeon**, desarrollado con **ASP.NET Core 8** utilizando **Minimal APIs**, **Entity Framework Core** y autenticación basada en **JWT**.

---

## 📋 Requisitos Previos

- [.NET 8.0 SDK](https://dotnet.microsoft.com/download/dotnet/8.0)
- [Microsoft SQL Server](https://www.microsoft.com/sql-server) (o SQL Server Express / LocalDB)

---

## ⚙️ Configuración

### 1. Cadena de conexión a Base de Datos
Configura la cadena de conexión en el archivo `appsettings.json` (o a través de variables de entorno / user secrets en desarrollo):

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=localhost;Database=LastDungeonDb;Trusted_Connection=True;TrustServerCertificate=True;"
  },
  "Jwt": {
    "Key": "TU_CLAVE_SECRETA_PERSONALIZADA_AQUI_32_BYTES",
    "Issuer": "LastDungeonApi",
    "Audience": "LastDungeonClient",
    "ExpireDays": 7
  }
}
```

> **Nota:** Ajusta el parámetro `Server` al nombre de tu instancia de SQL Server (por ejemplo `localhost`, `localhost\\SQLEXPRESS` o `(localdb)\\mssqllocaldb`).

---

## 🚀 Ejecución del Proyecto

1. Restaura dependencias:
   ```bash
   dotnet restore
   ```

2. Compila la solución:
   ```bash
   dotnet build
   ```

3. Ejecuta la aplicación:
   ```bash
   dotnet run
   ```

4. Una vez iniciada, accede a la documentación interactiva de Swagger UI en:
   - `http://localhost:5000/swagger` (o el puerto configurado en `Properties/launchSettings.json`)

---

## 📁 Estructura del Proyecto

```text
LastDungeon-Backend/
├── Data/            # DbContext de Entity Framework Core
├── Models/          # Entidades de base de datos
├── DTOs/            # Data Transfer Objects
│   ├── Auth/
│   ├── Jugadores/
│   ├── Runs/
│   ├── Mejoras/
│   ├── Cofres/
│   └── Ranking/
├── Mappings/        # Métodos de extensión para mapeo de Models a DTOs
├── Services/        # Interfaces e implementaciones de lógica de negocio
├── Endpoints/       # Definición de rutas y endpoints de Minimal APIs
├── Program.cs       # Configuración del servidor e inyección de dependencias
├── appsettings.json # Configuración de la aplicación
└── README.md        # Documentación general
```
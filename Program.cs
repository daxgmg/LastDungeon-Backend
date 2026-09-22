using System.Text;
using LastDungeon.Api.Data;
using LastDungeon.Api.Endpoints;
using LastDungeon.Api.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;

var builder = WebApplication.CreateBuilder(args);

// 1. Configurar DbContext con SQL Server
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(connectionString));

// 2. Registrar Servicios de la aplicación
builder.Services.AddScoped<IEmailService, EmailService>();
builder.Services.AddScoped<IJwtService, JwtService>();
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<IRunService, RunService>();
builder.Services.AddScoped<IMejoraService, MejoraService>();
builder.Services.AddScoped<ICofreService, CofreService>();
builder.Services.AddScoped<IRankingService, RankingService>();

// 3. Configurar Autenticación JWT Bearer
var jwtKey = builder.Configuration["Jwt:Key"] ?? "LastDungeonSecretKeyForJwtAuthenticationDefault2026";
var jwtIssuer = builder.Configuration["Jwt:Issuer"] ?? "LastDungeonApi";
var jwtAudience = builder.Configuration["Jwt:Audience"] ?? "LastDungeonClient";

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.RequireHttpsMetadata = false;
    options.SaveToken = true;
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuerSigningKey = true,
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey)),
        ValidateIssuer = true,
        ValidIssuer = jwtIssuer,
        ValidateAudience = true,
        ValidAudience = jwtAudience,
        ValidateLifetime = true,
        ClockSkew = TimeSpan.Zero
    };
});

builder.Services.AddAuthorization();

// 4. Configurar Swagger / OpenAPI con soporte para JWT Bearer
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "LastDungeon API",
        Version = "v1",
        Description = "API Backend para el juego roguelike LastDungeon"
    });

    options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = SecuritySchemeType.Http,
        Scheme = "Bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Description = "Introduce el token JWT en este formato: Bearer {tu token}"
    });

    options.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            Array.Empty<string>()
        }
    });
});

var app = builder.Build();

// 5. Configuración del Pipeline HTTP
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(options =>
    {
        options.SwaggerEndpoint("/swagger/v1/swagger.json", "LastDungeon API v1");
    });
}

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

// 6. Mapear Endpoints
app.MapAuthEndpoints();
app.MapRunEndpoints();
app.MapMejoraEndpoints();
app.MapCofreEndpoints();
app.MapRankingEndpoints();
app.MapJugadorEndpoints();
app.MapCofrePermanenteEndpoints();

// Root Health Check
app.MapGet("/", () => Results.Ok(new
{
    status = "Online",
    app = "LastDungeon API",
    version = "v1",
    timestamp = DateTime.UtcNow
}))
.WithName("RootHealthCheck")
.WithTags("General")
.WithOpenApi();

app.Run();

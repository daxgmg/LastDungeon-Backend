using System.ComponentModel.DataAnnotations;

namespace LastDungeon.Api.DTOs.Mejoras;

public record ComprarMejoraRequest(
    [Required] int MejoraId
);

using Microsoft.AspNetCore.Mvc;
using Sgcae.Api.Contracts;
using Sgcae.Application.Accesos;
using Sgcae.Domain.Enums;

namespace Sgcae.Api.Controllers;

[ApiController]
[Route("api/accesos")]
public sealed class AccesosController : ControllerBase
{
    [HttpPost]
    public async Task<IActionResult> Registrar(
        [FromBody] RegistrarAccesoRequest request,
        [FromServices] RegistrarAccesoHandler handler,
        CancellationToken cancellationToken) =>
        Ok(await handler.Handle(new RegistrarAccesoCommand(
            request.CodigoCredencial, request.CodigoPuntoAcceso, request.Tipo), cancellationToken));

    [HttpGet("historial")]
    public async Task<IActionResult> ConsultarHistorial(
        [FromQuery] Guid? empleadoId,
        [FromQuery] DateTime? desdeUtc,
        [FromQuery] DateTime? hastaUtc,
        [FromQuery] Guid? puntoAccesoId,
        [FromQuery] ResultadoAcceso? resultado,
        [FromQuery] int pagina,
        [FromQuery] int tamanoPagina,
        [FromServices] ConsultarHistorialAccesosHandler handler,
        CancellationToken cancellationToken) =>
        Ok(await handler.Handle(empleadoId, desdeUtc, hastaUtc, puntoAccesoId,
            resultado, pagina, tamanoPagina, cancellationToken));
}

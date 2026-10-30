using Microsoft.AspNetCore.Mvc;
using Sgcae.Api.Contracts;
using Sgcae.Application.PuntosAcceso;

namespace Sgcae.Api.Controllers;

[ApiController]
[Route("api/puntos-acceso")]
public sealed class PuntosAccesoController : ControllerBase
{
    [HttpPost]
    public async Task<IActionResult> Registrar(
        [FromBody] RegistrarPuntoAccesoRequest request,
        [FromServices] RegistrarPuntoAccesoHandler handler,
        CancellationToken cancellationToken)
    {
        var resultado = await handler.Handle(new RegistrarPuntoAccesoCommand(
            request.Codigo, request.Nombre, request.SedeId, request.Tipo), cancellationToken);
        return CreatedAtAction(nameof(ObtenerPorId), new { id = resultado.Id }, resultado);
    }

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> ObtenerPorId(
        Guid id,
        [FromServices] ConsultarPuntoAccesoHandler handler,
        CancellationToken cancellationToken) =>
        Ok(await handler.Handle(id, cancellationToken));

    [HttpGet]
    public async Task<IActionResult> Consultar(
        [FromQuery] Guid? sedeId,
        [FromQuery] bool? habilitado,
        [FromServices] ConsultarPuntosAccesoHandler handler,
        CancellationToken cancellationToken) =>
        Ok(await handler.Handle(sedeId, habilitado, cancellationToken));

    [HttpPatch("{id:guid}/estado")]
    public async Task<IActionResult> CambiarEstado(
        Guid id,
        [FromBody] CambiarEstadoPuntoRequest request,
        [FromServices] CambiarEstadoPuntoAccesoHandler handler,
        CancellationToken cancellationToken)
    {
        await handler.Handle(id, request.Habilitado, cancellationToken);
        return NoContent();
    }
}

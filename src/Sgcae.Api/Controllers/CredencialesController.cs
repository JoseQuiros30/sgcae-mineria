using Microsoft.AspNetCore.Mvc;
using Sgcae.Api.Contracts;
using Sgcae.Application.Credenciales;
using Sgcae.Domain.Enums;

namespace Sgcae.Api.Controllers;

[ApiController]
[Route("api/credenciales")]
public sealed class CredencialesController : ControllerBase
{
    [HttpPost]
    public async Task<IActionResult> Asignar(
        [FromBody] AsignarCredencialRequest request,
        [FromServices] AsignarCredencialHandler handler,
        CancellationToken cancellationToken)
    {
        var resultado = await handler.Handle(
            new AsignarCredencialCommand(request.Codigo, request.EmpleadoId), cancellationToken);
        return CreatedAtAction(nameof(ObtenerPorId), new { id = resultado.Id }, resultado);
    }

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> ObtenerPorId(
        Guid id,
        [FromServices] ConsultarCredencialHandler handler,
        CancellationToken cancellationToken) =>
        Ok(await handler.Handle(id, cancellationToken));

    [HttpGet]
    public async Task<IActionResult> Consultar(
        [FromQuery] Guid? empleadoId,
        [FromQuery] EstadoCredencial? estado,
        [FromQuery] int pagina,
        [FromQuery] int tamanoPagina,
        [FromServices] ConsultarCredencialesHandler handler,
        CancellationToken cancellationToken) =>
        Ok(await handler.Handle(empleadoId, estado, pagina, tamanoPagina, cancellationToken));

    [HttpPatch("{id:guid}/bloquear")]
    public async Task<IActionResult> Bloquear(
        Guid id,
        [FromServices] CambiarEstadoCredencialHandler handler,
        CancellationToken cancellationToken)
    {
        await handler.Handle(id, false, cancellationToken);
        return NoContent();
    }

    [HttpPatch("{id:guid}/revocar")]
    public async Task<IActionResult> Revocar(
        Guid id,
        [FromServices] CambiarEstadoCredencialHandler handler,
        CancellationToken cancellationToken)
    {
        await handler.Handle(id, true, cancellationToken);
        return NoContent();
    }
}

using Microsoft.AspNetCore.Mvc;
using Sgcae.Api.Contracts;
using Sgcae.Application.Empleados;
using Sgcae.Domain.Enums;

namespace Sgcae.Api.Controllers;

[ApiController]
[Route("api/empleados")]
public sealed class EmpleadosController : ControllerBase
{
    [HttpPost]
    [ProducesResponseType<EmpleadoDto>(StatusCodes.Status201Created)]
    public async Task<IActionResult> Registrar(
        [FromBody] RegistrarEmpleadoRequest request,
        [FromServices] RegistrarEmpleadoHandler handler,
        CancellationToken cancellationToken)
    {
        var resultado = await handler.Handle(new RegistrarEmpleadoCommand(
            request.Identificacion, request.Nombres, request.Apellidos,
            request.Correo, request.Cargo, request.AreaOrganizacional), cancellationToken);
        return CreatedAtAction(nameof(ObtenerPorId), new { id = resultado.Id }, resultado);
    }

    [HttpGet("{id:guid}")]
    [ProducesResponseType<EmpleadoDto>(StatusCodes.Status200OK)]
    public async Task<ActionResult<EmpleadoDto>> ObtenerPorId(
        Guid id,
        [FromServices] ConsultarEmpleadoHandler handler,
        CancellationToken cancellationToken) =>
        Ok(await handler.Handle(id, cancellationToken));

    [HttpGet]
    public async Task<IActionResult> Consultar(
        [FromQuery] string? busqueda,
        [FromQuery] EstadoLaboral? estado,
        [FromQuery] int pagina,
        [FromQuery] int tamanoPagina,
        [FromServices] ConsultarEmpleadosHandler handler,
        CancellationToken cancellationToken) =>
        Ok(await handler.Handle(busqueda, estado, pagina, tamanoPagina, cancellationToken));

    [HttpPut("{id:guid}")]
    public async Task<IActionResult> Actualizar(
        Guid id,
        [FromBody] ActualizarEmpleadoRequest request,
        [FromServices] ActualizarEmpleadoHandler handler,
        CancellationToken cancellationToken) =>
        Ok(await handler.Handle(new ActualizarEmpleadoCommand(
            id, request.Nombres, request.Apellidos, request.Correo,
            request.Cargo, request.AreaOrganizacional), cancellationToken));

    [HttpPatch("{id:guid}/desactivar")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public async Task<IActionResult> Desactivar(
        Guid id,
        [FromServices] DesactivarEmpleadoHandler handler,
        CancellationToken cancellationToken)
    {
        await handler.Handle(id, cancellationToken);
        return NoContent();
    }
}

using Microsoft.AspNetCore.Mvc;
using Sgcae.Application.PuntosAcceso;

namespace Sgcae.Api.Controllers;

[ApiController]
[Route("api/sedes")]
public sealed class SedesController : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> Consultar(
        [FromServices] ConsultarSedesHandler handler,
        CancellationToken cancellationToken) =>
        Ok(await handler.Handle(cancellationToken));
}

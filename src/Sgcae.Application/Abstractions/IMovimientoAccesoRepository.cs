using Sgcae.Application.Common;
using Sgcae.Domain.Entities;
using Sgcae.Domain.Enums;

namespace Sgcae.Application.Abstractions;

public interface IMovimientoAccesoRepository
{
    Task AgregarAsync(MovimientoAcceso movimiento, CancellationToken cancellationToken);
    Task<Pagina<MovimientoAcceso>> ConsultarAsync(
        Guid? empleadoId,
        DateTime? desdeUtc,
        DateTime? hastaUtc,
        Guid? puntoAccesoId,
        ResultadoAcceso? resultado,
        int pagina,
        int tamanoPagina,
        CancellationToken cancellationToken);
}

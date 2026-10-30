using Sgcae.Domain.Entities;

namespace Sgcae.Application.Abstractions;

public interface IPuntoAccesoRepository
{
    Task AgregarAsync(PuntoAcceso punto, CancellationToken cancellationToken);
    Task<PuntoAcceso?> ObtenerPorIdAsync(Guid id, CancellationToken cancellationToken);
    Task<PuntoAcceso?> ObtenerPorCodigoAsync(string codigo, CancellationToken cancellationToken);
    Task<bool> ExisteCodigoAsync(string codigo, CancellationToken cancellationToken);
    Task<Sede?> ObtenerSedeAsync(Guid sedeId, CancellationToken cancellationToken);
    Task<IReadOnlyCollection<PuntoAcceso>> ConsultarAsync(Guid? sedeId, bool? habilitado, CancellationToken cancellationToken);
    Task<IReadOnlyCollection<Sede>> ConsultarSedesAsync(CancellationToken cancellationToken);
}

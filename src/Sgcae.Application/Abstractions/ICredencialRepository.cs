using Sgcae.Application.Common;
using Sgcae.Domain.Entities;
using Sgcae.Domain.Enums;

namespace Sgcae.Application.Abstractions;

public interface ICredencialRepository
{
    Task AgregarAsync(Credencial credencial, CancellationToken cancellationToken);
    Task<Credencial?> ObtenerPorIdAsync(Guid id, CancellationToken cancellationToken);
    Task<Credencial?> ObtenerPorCodigoConEmpleadoAsync(string codigo, CancellationToken cancellationToken);
    Task<bool> ExisteCodigoAsync(string codigo, CancellationToken cancellationToken);
    Task<bool> EmpleadoTieneCredencialVigenteAsync(Guid empleadoId, CancellationToken cancellationToken);
    Task<Pagina<Credencial>> ConsultarAsync(
        Guid? empleadoId,
        EstadoCredencial? estado,
        int pagina,
        int tamanoPagina,
        CancellationToken cancellationToken);
}

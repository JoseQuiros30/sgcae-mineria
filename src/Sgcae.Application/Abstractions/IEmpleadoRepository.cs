using Sgcae.Application.Common;
using Sgcae.Domain.Entities;
using Sgcae.Domain.Enums;

namespace Sgcae.Application.Abstractions;

public interface IEmpleadoRepository
{
    Task AgregarAsync(Empleado empleado, CancellationToken cancellationToken);
    Task<Empleado?> ObtenerPorIdAsync(Guid id, CancellationToken cancellationToken);
    Task<bool> ExisteIdentificacionAsync(string identificacion, Guid? exceptoId, CancellationToken cancellationToken);
    Task<bool> ExisteCorreoAsync(string correo, Guid? exceptoId, CancellationToken cancellationToken);
    Task<Pagina<Empleado>> ConsultarAsync(
        string? busqueda,
        EstadoLaboral? estado,
        int pagina,
        int tamanoPagina,
        CancellationToken cancellationToken);
}

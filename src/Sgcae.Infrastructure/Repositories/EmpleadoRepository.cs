using Microsoft.EntityFrameworkCore;
using Sgcae.Application.Abstractions;
using Sgcae.Application.Common;
using Sgcae.Domain.Entities;
using Sgcae.Domain.Enums;
using Sgcae.Infrastructure.Persistence;

namespace Sgcae.Infrastructure.Repositories;

public sealed class EmpleadoRepository(SgcaeDbContext context) : IEmpleadoRepository
{
    public async Task AgregarAsync(Empleado empleado, CancellationToken cancellationToken) =>
        await context.Empleados.AddAsync(empleado, cancellationToken);

    public Task<Empleado?> ObtenerPorIdAsync(Guid id, CancellationToken cancellationToken) =>
        context.Empleados.FirstOrDefaultAsync(x => x.Id == id, cancellationToken);

    public Task<bool> ExisteIdentificacionAsync(
        string identificacion, Guid? exceptoId, CancellationToken cancellationToken)
    {
        var valor = identificacion.Trim();
        return context.Empleados.AnyAsync(
            x => x.Identificacion == valor && (!exceptoId.HasValue || x.Id != exceptoId), cancellationToken);
    }

    public Task<bool> ExisteCorreoAsync(string correo, Guid? exceptoId, CancellationToken cancellationToken)
    {
        var valor = correo.Trim().ToLowerInvariant();
        return context.Empleados.AnyAsync(
            x => x.Correo.Valor == valor && (!exceptoId.HasValue || x.Id != exceptoId), cancellationToken);
    }

    public async Task<Pagina<Empleado>> ConsultarAsync(
        string? busqueda, EstadoLaboral? estado, int pagina, int tamanoPagina,
        CancellationToken cancellationToken)
    {
        var query = context.Empleados.AsNoTracking().AsQueryable();
        if (!string.IsNullOrWhiteSpace(busqueda))
        {
            var texto = busqueda.Trim();
            query = query.Where(x => x.Identificacion.Contains(texto)
                || x.Nombres.Contains(texto) || x.Apellidos.Contains(texto));
        }
        if (estado.HasValue)
            query = query.Where(x => x.Estado == estado);

        var total = await query.CountAsync(cancellationToken);
        var elementos = await query.OrderBy(x => x.Apellidos).ThenBy(x => x.Nombres)
            .Skip((pagina - 1) * tamanoPagina).Take(tamanoPagina)
            .ToListAsync(cancellationToken);
        return new Pagina<Empleado>(elementos, pagina, tamanoPagina, total);
    }
}

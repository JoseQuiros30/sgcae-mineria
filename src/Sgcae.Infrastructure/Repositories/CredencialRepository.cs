using Microsoft.EntityFrameworkCore;
using Sgcae.Application.Abstractions;
using Sgcae.Application.Common;
using Sgcae.Domain.Entities;
using Sgcae.Domain.Enums;
using Sgcae.Infrastructure.Persistence;

namespace Sgcae.Infrastructure.Repositories;

public sealed class CredencialRepository(SgcaeDbContext context) : ICredencialRepository
{
    public async Task AgregarAsync(Credencial credencial, CancellationToken cancellationToken) =>
        await context.Credenciales.AddAsync(credencial, cancellationToken);

    public Task<Credencial?> ObtenerPorIdAsync(Guid id, CancellationToken cancellationToken) =>
        context.Credenciales.Include(x => x.Empleado)
            .FirstOrDefaultAsync(x => x.Id == id, cancellationToken);

    public Task<Credencial?> ObtenerPorCodigoConEmpleadoAsync(string codigo, CancellationToken cancellationToken)
    {
        var valor = codigo.Trim().ToUpperInvariant();
        return context.Credenciales.Include(x => x.Empleado)
            .FirstOrDefaultAsync(x => x.Codigo == valor, cancellationToken);
    }

    public Task<bool> ExisteCodigoAsync(string codigo, CancellationToken cancellationToken)
    {
        var valor = codigo.Trim().ToUpperInvariant();
        return context.Credenciales.AnyAsync(x => x.Codigo == valor, cancellationToken);
    }

    public Task<bool> EmpleadoTieneCredencialVigenteAsync(Guid empleadoId, CancellationToken cancellationToken) =>
        context.Credenciales.AnyAsync(
            x => x.EmpleadoId == empleadoId && x.Estado == EstadoCredencial.Vigente, cancellationToken);

    public async Task<Pagina<Credencial>> ConsultarAsync(
        Guid? empleadoId, EstadoCredencial? estado, int pagina, int tamanoPagina,
        CancellationToken cancellationToken)
    {
        var query = context.Credenciales.AsNoTracking().Include(x => x.Empleado).AsQueryable();
        if (empleadoId.HasValue)
            query = query.Where(x => x.EmpleadoId == empleadoId);
        if (estado.HasValue)
            query = query.Where(x => x.Estado == estado);

        var total = await query.CountAsync(cancellationToken);
        var elementos = await query.OrderBy(x => x.Codigo)
            .Skip((pagina - 1) * tamanoPagina).Take(tamanoPagina)
            .ToListAsync(cancellationToken);
        return new Pagina<Credencial>(elementos, pagina, tamanoPagina, total);
    }
}

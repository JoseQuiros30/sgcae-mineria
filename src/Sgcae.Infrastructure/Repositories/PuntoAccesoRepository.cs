using Microsoft.EntityFrameworkCore;
using Sgcae.Application.Abstractions;
using Sgcae.Domain.Entities;
using Sgcae.Infrastructure.Persistence;

namespace Sgcae.Infrastructure.Repositories;

public sealed class PuntoAccesoRepository(SgcaeDbContext context) : IPuntoAccesoRepository
{
    public async Task AgregarAsync(PuntoAcceso punto, CancellationToken cancellationToken) =>
        await context.PuntosAcceso.AddAsync(punto, cancellationToken);

    public Task<PuntoAcceso?> ObtenerPorIdAsync(Guid id, CancellationToken cancellationToken) =>
        context.PuntosAcceso.Include(x => x.Sede)
            .FirstOrDefaultAsync(x => x.Id == id, cancellationToken);

    public Task<PuntoAcceso?> ObtenerPorCodigoAsync(string codigo, CancellationToken cancellationToken)
    {
        var valor = codigo.Trim().ToUpperInvariant();
        return context.PuntosAcceso.Include(x => x.Sede)
            .FirstOrDefaultAsync(x => x.Codigo == valor, cancellationToken);
    }

    public Task<bool> ExisteCodigoAsync(string codigo, CancellationToken cancellationToken)
    {
        var valor = codigo.Trim().ToUpperInvariant();
        return context.PuntosAcceso.AnyAsync(x => x.Codigo == valor, cancellationToken);
    }

    public Task<Sede?> ObtenerSedeAsync(Guid sedeId, CancellationToken cancellationToken) =>
        context.Sedes.AsNoTracking().FirstOrDefaultAsync(x => x.Id == sedeId, cancellationToken);

    public async Task<IReadOnlyCollection<PuntoAcceso>> ConsultarAsync(
        Guid? sedeId, bool? habilitado, CancellationToken cancellationToken)
    {
        var query = context.PuntosAcceso.AsNoTracking().Include(x => x.Sede).AsQueryable();
        if (sedeId.HasValue)
            query = query.Where(x => x.SedeId == sedeId);
        if (habilitado.HasValue)
            query = query.Where(x => x.Habilitado == habilitado);
        return await query.OrderBy(x => x.Sede.Nombre).ThenBy(x => x.Nombre)
            .ToListAsync(cancellationToken);
    }

    public async Task<IReadOnlyCollection<Sede>> ConsultarSedesAsync(CancellationToken cancellationToken) =>
        await context.Sedes.AsNoTracking().OrderBy(x => x.Nombre).ToListAsync(cancellationToken);
}

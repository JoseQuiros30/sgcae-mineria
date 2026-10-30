using Microsoft.EntityFrameworkCore;
using Sgcae.Application.Abstractions;
using Sgcae.Application.Common;
using Sgcae.Domain.Entities;
using Sgcae.Domain.Enums;
using Sgcae.Infrastructure.Persistence;

namespace Sgcae.Infrastructure.Repositories;

public sealed class MovimientoAccesoRepository(SgcaeDbContext context) : IMovimientoAccesoRepository
{
    public async Task AgregarAsync(MovimientoAcceso movimiento, CancellationToken cancellationToken) =>
        await context.MovimientosAcceso.AddAsync(movimiento, cancellationToken);

    public async Task<Pagina<MovimientoAcceso>> ConsultarAsync(
        Guid? empleadoId,
        DateTime? desdeUtc,
        DateTime? hastaUtc,
        Guid? puntoAccesoId,
        ResultadoAcceso? resultado,
        int pagina,
        int tamanoPagina,
        CancellationToken cancellationToken)
    {
        var query = context.MovimientosAcceso.AsNoTracking()
            .Include(x => x.Empleado).Include(x => x.PuntoAcceso).AsQueryable();

        if (empleadoId.HasValue)
            query = query.Where(x => x.EmpleadoId == empleadoId);
        if (desdeUtc.HasValue)
            query = query.Where(x => x.RegistradoEnUtc >= desdeUtc);
        if (hastaUtc.HasValue)
            query = query.Where(x => x.RegistradoEnUtc <= hastaUtc);
        if (puntoAccesoId.HasValue)
            query = query.Where(x => x.PuntoAccesoId == puntoAccesoId);
        if (resultado.HasValue)
            query = query.Where(x => x.Resultado == resultado);

        var total = await query.CountAsync(cancellationToken);
        var elementos = await query.OrderByDescending(x => x.RegistradoEnUtc)
            .Skip((pagina - 1) * tamanoPagina).Take(tamanoPagina)
            .ToListAsync(cancellationToken);
        return new Pagina<MovimientoAcceso>(elementos, pagina, tamanoPagina, total);
    }
}

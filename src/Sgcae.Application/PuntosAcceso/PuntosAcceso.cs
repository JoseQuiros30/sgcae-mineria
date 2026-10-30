using Sgcae.Application.Abstractions;
using Sgcae.Application.Common;
using Sgcae.Domain.Entities;
using Sgcae.Domain.Enums;

namespace Sgcae.Application.PuntosAcceso;

public sealed record PuntoAccesoDto(
    Guid Id,
    string Codigo,
    string Nombre,
    Guid SedeId,
    string Sede,
    TipoPuntoAcceso Tipo,
    bool Habilitado)
{
    public static PuntoAccesoDto Desde(PuntoAcceso punto) => new(
        punto.Id, punto.Codigo, punto.Nombre, punto.SedeId,
        punto.Sede?.Nombre ?? string.Empty, punto.Tipo, punto.Habilitado);
}

public sealed record SedeDto(Guid Id, string Codigo, string Nombre)
{
    public static SedeDto Desde(Sede sede) => new(sede.Id, sede.Codigo, sede.Nombre);
}

public sealed record RegistrarPuntoAccesoCommand(
    string Codigo,
    string Nombre,
    Guid SedeId,
    TipoPuntoAcceso Tipo);

public sealed class RegistrarPuntoAccesoHandler(IPuntoAccesoRepository repository, IUnidadTrabajo unidadTrabajo)
{
    public async Task<PuntoAccesoDto> Handle(
        RegistrarPuntoAccesoCommand command, CancellationToken cancellationToken)
    {
        var sede = await repository.ObtenerSedeAsync(command.SedeId, cancellationToken)
            ?? throw new RecursoNoEncontradoException("Sede no encontrada.");
        var punto = PuntoAcceso.Crear(command.Codigo, command.Nombre, command.SedeId, command.Tipo);
        if (await repository.ExisteCodigoAsync(punto.Codigo, cancellationToken))
            throw new ConflictoException("Ya existe un punto de acceso con ese código.");

        await repository.AgregarAsync(punto, cancellationToken);
        await unidadTrabajo.GuardarCambiosAsync(cancellationToken);
        return new PuntoAccesoDto(
            punto.Id, punto.Codigo, punto.Nombre, punto.SedeId,
            sede.Nombre, punto.Tipo, punto.Habilitado);
    }
}

public sealed class CambiarEstadoPuntoAccesoHandler(IPuntoAccesoRepository repository, IUnidadTrabajo unidadTrabajo)
{
    public async Task Handle(Guid id, bool habilitado, CancellationToken cancellationToken)
    {
        var punto = await repository.ObtenerPorIdAsync(id, cancellationToken)
            ?? throw new RecursoNoEncontradoException("Punto de acceso no encontrado.");
        punto.CambiarEstado(habilitado);
        await unidadTrabajo.GuardarCambiosAsync(cancellationToken);
    }
}

public sealed class ConsultarPuntosAccesoHandler(IPuntoAccesoRepository repository)
{
    public async Task<IReadOnlyCollection<PuntoAccesoDto>> Handle(
        Guid? sedeId, bool? habilitado, CancellationToken cancellationToken)
    {
        var puntos = await repository.ConsultarAsync(sedeId, habilitado, cancellationToken);
        return puntos.Select(PuntoAccesoDto.Desde).ToArray();
    }
}

public sealed class ConsultarSedesHandler(IPuntoAccesoRepository repository)
{
    public async Task<IReadOnlyCollection<SedeDto>> Handle(CancellationToken cancellationToken)
    {
        var sedes = await repository.ConsultarSedesAsync(cancellationToken);
        return sedes.Select(SedeDto.Desde).ToArray();
    }
}

public sealed class ConsultarPuntoAccesoHandler(IPuntoAccesoRepository repository)
{
    public async Task<PuntoAccesoDto> Handle(Guid id, CancellationToken cancellationToken)
    {
        var punto = await repository.ObtenerPorIdAsync(id, cancellationToken)
            ?? throw new RecursoNoEncontradoException("Punto de acceso no encontrado.");
        return PuntoAccesoDto.Desde(punto);
    }
}

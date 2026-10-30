using Sgcae.Application.Abstractions;
using Sgcae.Application.Common;
using Sgcae.Domain.Entities;
using Sgcae.Domain.Enums;

namespace Sgcae.Application.Accesos;

public sealed record RegistrarAccesoCommand(
    string CodigoCredencial,
    string CodigoPuntoAcceso,
    TipoMovimiento Tipo);

public sealed record ResultadoAccesoDto(
    Guid MovimientoId,
    ResultadoAcceso Resultado,
    string Motivo,
    DateTime RegistradoEnUtc);

public sealed class RegistrarAccesoHandler(
    ICredencialRepository credenciales,
    IPuntoAccesoRepository puntos,
    IMovimientoAccesoRepository movimientos,
    IUnidadTrabajo unidadTrabajo)
{
    public async Task<ResultadoAccesoDto> Handle(
        RegistrarAccesoCommand command, CancellationToken cancellationToken)
    {
        var codigoCredencial = command.CodigoCredencial?.Trim().ToUpperInvariant() ?? string.Empty;
        var codigoPunto = command.CodigoPuntoAcceso?.Trim().ToUpperInvariant() ?? string.Empty;
        var credencial = string.IsNullOrEmpty(codigoCredencial)
            ? null
            : await credenciales.ObtenerPorCodigoConEmpleadoAsync(codigoCredencial, cancellationToken);
        var punto = string.IsNullOrEmpty(codigoPunto)
            ? null
            : await puntos.ObtenerPorCodigoAsync(codigoPunto, cancellationToken);

        var (resultado, motivo) = Evaluar(credencial, punto);
        var movimiento = MovimientoAcceso.Crear(
            codigoCredencial, codigoPunto, command.Tipo, resultado, motivo,
            credencial?.Id, credencial?.EmpleadoId, punto?.Id);

        await movimientos.AgregarAsync(movimiento, cancellationToken);
        await unidadTrabajo.GuardarCambiosAsync(cancellationToken);
        return new ResultadoAccesoDto(movimiento.Id, resultado, motivo, movimiento.RegistradoEnUtc);
    }

    private static (ResultadoAcceso Resultado, string Motivo) Evaluar(
        Credencial? credencial, PuntoAcceso? punto)
    {
        if (credencial is null)
            return (ResultadoAcceso.Rechazado, "Credencial inexistente.");
        if (punto is null)
            return (ResultadoAcceso.Rechazado, "Punto de acceso inexistente.");
        if (credencial.Estado != EstadoCredencial.Vigente)
            return (ResultadoAcceso.Rechazado, $"Credencial {credencial.Estado.ToString().ToLowerInvariant()}.");
        if (credencial.Empleado.Estado != EstadoLaboral.Activo)
            return (ResultadoAcceso.Rechazado, "El vínculo laboral del empleado está inactivo.");
        if (!punto.Habilitado)
            return (ResultadoAcceso.Rechazado, "El punto de acceso está deshabilitado.");

        return (ResultadoAcceso.Autorizado, "Acceso autorizado.");
    }
}

public sealed record MovimientoAccesoDto(
    Guid Id,
    string CodigoCredencial,
    string CodigoPuntoAcceso,
    Guid? EmpleadoId,
    string? Empleado,
    Guid? PuntoAccesoId,
    string? PuntoAcceso,
    TipoMovimiento Tipo,
    ResultadoAcceso Resultado,
    string Motivo,
    DateTime RegistradoEnUtc)
{
    public static MovimientoAccesoDto Desde(MovimientoAcceso movimiento) => new(
        movimiento.Id, movimiento.CodigoCredencial, movimiento.CodigoPuntoAcceso,
        movimiento.EmpleadoId,
        movimiento.Empleado is null ? null : $"{movimiento.Empleado.Nombres} {movimiento.Empleado.Apellidos}",
        movimiento.PuntoAccesoId, movimiento.PuntoAcceso?.Nombre,
        movimiento.Tipo, movimiento.Resultado, movimiento.Motivo, movimiento.RegistradoEnUtc);
}

public sealed class ConsultarHistorialAccesosHandler(IMovimientoAccesoRepository repository)
{
    public async Task<Pagina<MovimientoAccesoDto>> Handle(
        Guid? empleadoId,
        DateTime? desdeUtc,
        DateTime? hastaUtc,
        Guid? puntoAccesoId,
        ResultadoAcceso? resultado,
        int pagina,
        int tamanoPagina,
        CancellationToken cancellationToken)
    {
        if (desdeUtc.HasValue && hastaUtc.HasValue && desdeUtc > hastaUtc)
            throw new ArgumentException("La fecha inicial no puede ser posterior a la fecha final.");

        (pagina, tamanoPagina) = Paginacion.Normalizar(pagina, tamanoPagina);
        var consulta = await repository.ConsultarAsync(
            empleadoId, desdeUtc, hastaUtc, puntoAccesoId, resultado,
            pagina, tamanoPagina, cancellationToken);
        return new Pagina<MovimientoAccesoDto>(
            consulta.Elementos.Select(MovimientoAccesoDto.Desde).ToArray(),
            pagina, tamanoPagina, consulta.TotalElementos);
    }
}

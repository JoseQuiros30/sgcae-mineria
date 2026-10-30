using Sgcae.Application.Abstractions;
using Sgcae.Application.Common;
using Sgcae.Domain.Entities;
using Sgcae.Domain.Enums;

namespace Sgcae.Application.Empleados;

public sealed record EmpleadoDto(
    Guid Id,
    string Identificacion,
    string Nombres,
    string Apellidos,
    string Correo,
    string Cargo,
    string AreaOrganizacional,
    EstadoLaboral Estado,
    DateTime CreadoEnUtc,
    DateTime? ActualizadoEnUtc)
{
    public static EmpleadoDto Desde(Empleado empleado) => new(
        empleado.Id, empleado.Identificacion, empleado.Nombres, empleado.Apellidos,
        empleado.Correo.Valor, empleado.Cargo, empleado.AreaOrganizacional,
        empleado.Estado, empleado.CreadoEnUtc, empleado.ActualizadoEnUtc);
}

public sealed record RegistrarEmpleadoCommand(
    string Identificacion,
    string Nombres,
    string Apellidos,
    string Correo,
    string Cargo,
    string AreaOrganizacional);

public sealed class RegistrarEmpleadoHandler(IEmpleadoRepository repository, IUnidadTrabajo unidadTrabajo)
{
    public async Task<EmpleadoDto> Handle(RegistrarEmpleadoCommand command, CancellationToken cancellationToken)
    {
        var empleado = Empleado.Crear(
            command.Identificacion, command.Nombres, command.Apellidos,
            command.Correo, command.Cargo, command.AreaOrganizacional);
        if (await repository.ExisteIdentificacionAsync(empleado.Identificacion, null, cancellationToken))
            throw new ConflictoException("Ya existe un empleado con esa identificación.");
        if (await repository.ExisteCorreoAsync(empleado.Correo.Valor, null, cancellationToken))
            throw new ConflictoException("Ya existe un empleado con ese correo.");

        await repository.AgregarAsync(empleado, cancellationToken);
        await unidadTrabajo.GuardarCambiosAsync(cancellationToken);
        return EmpleadoDto.Desde(empleado);
    }
}

public sealed record ActualizarEmpleadoCommand(
    Guid Id,
    string Nombres,
    string Apellidos,
    string Correo,
    string Cargo,
    string AreaOrganizacional);

public sealed class ActualizarEmpleadoHandler(IEmpleadoRepository repository, IUnidadTrabajo unidadTrabajo)
{
    public async Task<EmpleadoDto> Handle(ActualizarEmpleadoCommand command, CancellationToken cancellationToken)
    {
        var empleado = await repository.ObtenerPorIdAsync(command.Id, cancellationToken)
            ?? throw new RecursoNoEncontradoException("Empleado no encontrado.");
        empleado.Actualizar(command.Nombres, command.Apellidos, command.Correo,
            command.Cargo, command.AreaOrganizacional);
        if (await repository.ExisteCorreoAsync(empleado.Correo.Valor, command.Id, cancellationToken))
            throw new ConflictoException("Ya existe otro empleado con ese correo.");

        await unidadTrabajo.GuardarCambiosAsync(cancellationToken);
        return EmpleadoDto.Desde(empleado);
    }
}

public sealed class DesactivarEmpleadoHandler(IEmpleadoRepository repository, IUnidadTrabajo unidadTrabajo)
{
    public async Task Handle(Guid id, CancellationToken cancellationToken)
    {
        var empleado = await repository.ObtenerPorIdAsync(id, cancellationToken)
            ?? throw new RecursoNoEncontradoException("Empleado no encontrado.");
        empleado.Desactivar();
        await unidadTrabajo.GuardarCambiosAsync(cancellationToken);
    }
}

public sealed class ConsultarEmpleadoHandler(IEmpleadoRepository repository)
{
    public async Task<EmpleadoDto> Handle(Guid id, CancellationToken cancellationToken)
    {
        var empleado = await repository.ObtenerPorIdAsync(id, cancellationToken)
            ?? throw new RecursoNoEncontradoException("Empleado no encontrado.");
        return EmpleadoDto.Desde(empleado);
    }
}

public sealed class ConsultarEmpleadosHandler(IEmpleadoRepository repository)
{
    public async Task<Pagina<EmpleadoDto>> Handle(
        string? busqueda, EstadoLaboral? estado, int pagina, int tamanoPagina,
        CancellationToken cancellationToken)
    {
        (pagina, tamanoPagina) = Paginacion.Normalizar(pagina, tamanoPagina);
        var resultado = await repository.ConsultarAsync(busqueda, estado, pagina, tamanoPagina, cancellationToken);
        return new Pagina<EmpleadoDto>(resultado.Elementos.Select(EmpleadoDto.Desde).ToArray(),
            pagina, tamanoPagina, resultado.TotalElementos);
    }
}

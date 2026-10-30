using Sgcae.Application.Abstractions;
using Sgcae.Application.Common;
using Sgcae.Domain.Entities;
using Sgcae.Domain.Enums;

namespace Sgcae.Application.Credenciales;

public sealed record CredencialDto(
    Guid Id,
    string Codigo,
    Guid EmpleadoId,
    string Empleado,
    EstadoCredencial Estado,
    DateTime AsignadaEnUtc,
    DateTime? ModificadaEnUtc)
{
    public static CredencialDto Desde(Credencial credencial) => new(
        credencial.Id, credencial.Codigo, credencial.EmpleadoId,
        credencial.Empleado is null
            ? string.Empty
            : $"{credencial.Empleado.Nombres} {credencial.Empleado.Apellidos}",
        credencial.Estado, credencial.AsignadaEnUtc, credencial.ModificadaEnUtc);
}

public sealed record AsignarCredencialCommand(string Codigo, Guid EmpleadoId);

public sealed class AsignarCredencialHandler(
    ICredencialRepository credenciales,
    IEmpleadoRepository empleados,
    IUnidadTrabajo unidadTrabajo)
{
    public async Task<CredencialDto> Handle(AsignarCredencialCommand command, CancellationToken cancellationToken)
    {
        var empleado = await empleados.ObtenerPorIdAsync(command.EmpleadoId, cancellationToken)
            ?? throw new RecursoNoEncontradoException("Empleado no encontrado.");
        if (empleado.Estado != EstadoLaboral.Activo)
            throw new ConflictoException("No se puede asignar una credencial a un empleado inactivo.");
        var credencial = Credencial.Asignar(command.Codigo, command.EmpleadoId);
        if (await credenciales.ExisteCodigoAsync(credencial.Codigo, cancellationToken))
            throw new ConflictoException("Ya existe una credencial con ese código.");
        if (await credenciales.EmpleadoTieneCredencialVigenteAsync(command.EmpleadoId, cancellationToken))
            throw new ConflictoException("El empleado ya tiene una credencial vigente.");

        await credenciales.AgregarAsync(credencial, cancellationToken);
        await unidadTrabajo.GuardarCambiosAsync(cancellationToken);
        return new CredencialDto(
            credencial.Id, credencial.Codigo, credencial.EmpleadoId,
            $"{empleado.Nombres} {empleado.Apellidos}", credencial.Estado,
            credencial.AsignadaEnUtc, credencial.ModificadaEnUtc);
    }
}

public sealed class CambiarEstadoCredencialHandler(ICredencialRepository repository, IUnidadTrabajo unidadTrabajo)
{
    public async Task Handle(Guid id, bool revocar, CancellationToken cancellationToken)
    {
        var credencial = await repository.ObtenerPorIdAsync(id, cancellationToken)
            ?? throw new RecursoNoEncontradoException("Credencial no encontrada.");
        if (revocar)
            credencial.Revocar();
        else
            credencial.Bloquear();
        await unidadTrabajo.GuardarCambiosAsync(cancellationToken);
    }
}

public sealed class ConsultarCredencialesHandler(ICredencialRepository repository)
{
    public async Task<Pagina<CredencialDto>> Handle(
        Guid? empleadoId, EstadoCredencial? estado, int pagina, int tamanoPagina,
        CancellationToken cancellationToken)
    {
        (pagina, tamanoPagina) = Paginacion.Normalizar(pagina, tamanoPagina);
        var resultado = await repository.ConsultarAsync(
            empleadoId, estado, pagina, tamanoPagina, cancellationToken);
        return new Pagina<CredencialDto>(resultado.Elementos.Select(CredencialDto.Desde).ToArray(),
            pagina, tamanoPagina, resultado.TotalElementos);
    }
}

public sealed class ConsultarCredencialHandler(ICredencialRepository repository)
{
    public async Task<CredencialDto> Handle(Guid id, CancellationToken cancellationToken)
    {
        var credencial = await repository.ObtenerPorIdAsync(id, cancellationToken)
            ?? throw new RecursoNoEncontradoException("Credencial no encontrada.");
        return CredencialDto.Desde(credencial);
    }
}

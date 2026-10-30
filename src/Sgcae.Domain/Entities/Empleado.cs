using Sgcae.Domain.Common;
using Sgcae.Domain.Enums;
using Sgcae.Domain.ValueObjects;

namespace Sgcae.Domain.Entities;

public sealed class Empleado : Entity
{
    private Empleado() { }

    private Empleado(
        string identificacion,
        string nombres,
        string apellidos,
        CorreoElectronico correo,
        string cargo,
        string areaOrganizacional)
    {
        Id = Guid.NewGuid();
        ActualizarDatos(identificacion, nombres, apellidos, correo, cargo, areaOrganizacional);
        Estado = EstadoLaboral.Activo;
        CreadoEnUtc = DateTime.UtcNow;
    }

    public string Identificacion { get; private set; } = string.Empty;
    public string Nombres { get; private set; } = string.Empty;
    public string Apellidos { get; private set; } = string.Empty;
    public CorreoElectronico Correo { get; private set; } = null!;
    public string Cargo { get; private set; } = string.Empty;
    public string AreaOrganizacional { get; private set; } = string.Empty;
    public EstadoLaboral Estado { get; private set; }
    public DateTime CreadoEnUtc { get; private set; }
    public DateTime? ActualizadoEnUtc { get; private set; }

    public static Empleado Crear(
        string identificacion,
        string nombres,
        string apellidos,
        string correo,
        string cargo,
        string areaOrganizacional) =>
        new(identificacion, nombres, apellidos, CorreoElectronico.Crear(correo), cargo, areaOrganizacional);

    public void Actualizar(
        string nombres,
        string apellidos,
        string correo,
        string cargo,
        string areaOrganizacional)
    {
        ActualizarDatos(Identificacion, nombres, apellidos, CorreoElectronico.Crear(correo), cargo, areaOrganizacional);
        ActualizadoEnUtc = DateTime.UtcNow;
    }

    public void Desactivar()
    {
        if (Estado == EstadoLaboral.Inactivo)
            throw new DomainException("El empleado ya se encuentra inactivo.");

        Estado = EstadoLaboral.Inactivo;
        ActualizadoEnUtc = DateTime.UtcNow;
    }

    private void ActualizarDatos(
        string identificacion,
        string nombres,
        string apellidos,
        CorreoElectronico correo,
        string cargo,
        string areaOrganizacional)
    {
        Identificacion = Requerido(identificacion, "identificación", 30);
        Nombres = Requerido(nombres, "nombres", 100);
        Apellidos = Requerido(apellidos, "apellidos", 100);
        Correo = correo;
        Cargo = Requerido(cargo, "cargo", 100);
        AreaOrganizacional = Requerido(areaOrganizacional, "área organizacional", 100);
    }

    private static string Requerido(string valor, string campo, int maximo)
    {
        var limpio = valor?.Trim() ?? string.Empty;
        if (string.IsNullOrWhiteSpace(limpio))
            throw new DomainException($"El campo {campo} es obligatorio.");
        if (limpio.Length > maximo)
            throw new DomainException($"El campo {campo} no puede superar {maximo} caracteres.");
        return limpio;
    }
}

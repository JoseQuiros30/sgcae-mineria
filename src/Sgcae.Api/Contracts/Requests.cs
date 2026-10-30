using System.ComponentModel.DataAnnotations;
using Sgcae.Domain.Enums;

namespace Sgcae.Api.Contracts;

public sealed class RegistrarEmpleadoRequest
{
    [Required, MaxLength(30)] public string Identificacion { get; init; } = string.Empty;
    [Required, MaxLength(100)] public string Nombres { get; init; } = string.Empty;
    [Required, MaxLength(100)] public string Apellidos { get; init; } = string.Empty;
    [Required, EmailAddress, MaxLength(150)] public string Correo { get; init; } = string.Empty;
    [Required, MaxLength(100)] public string Cargo { get; init; } = string.Empty;
    [Required, MaxLength(100)] public string AreaOrganizacional { get; init; } = string.Empty;
}

public sealed class ActualizarEmpleadoRequest
{
    [Required, MaxLength(100)] public string Nombres { get; init; } = string.Empty;
    [Required, MaxLength(100)] public string Apellidos { get; init; } = string.Empty;
    [Required, EmailAddress, MaxLength(150)] public string Correo { get; init; } = string.Empty;
    [Required, MaxLength(100)] public string Cargo { get; init; } = string.Empty;
    [Required, MaxLength(100)] public string AreaOrganizacional { get; init; } = string.Empty;
}

public sealed class AsignarCredencialRequest
{
    [Required, MaxLength(50)] public string Codigo { get; init; } = string.Empty;
    public Guid EmpleadoId { get; init; }
}

public sealed class RegistrarPuntoAccesoRequest
{
    [Required, MaxLength(30)] public string Codigo { get; init; } = string.Empty;
    [Required, MaxLength(100)] public string Nombre { get; init; } = string.Empty;
    public Guid SedeId { get; init; }
    [EnumDataType(typeof(TipoPuntoAcceso))] public TipoPuntoAcceso Tipo { get; init; }
}

public sealed class CambiarEstadoPuntoRequest
{
    public bool Habilitado { get; init; }
}

public sealed class RegistrarAccesoRequest
{
    [Required, MaxLength(50)] public string CodigoCredencial { get; init; } = string.Empty;
    [Required, MaxLength(30)] public string CodigoPuntoAcceso { get; init; } = string.Empty;
    [EnumDataType(typeof(TipoMovimiento))] public TipoMovimiento Tipo { get; init; }
}

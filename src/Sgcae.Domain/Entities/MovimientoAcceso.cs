using Sgcae.Domain.Common;
using Sgcae.Domain.Enums;

namespace Sgcae.Domain.Entities;

public sealed class MovimientoAcceso : Entity
{
    private MovimientoAcceso() { }

    private MovimientoAcceso(
        string codigoCredencial,
        string codigoPuntoAcceso,
        TipoMovimiento tipo,
        ResultadoAcceso resultado,
        string motivo,
        Guid? credencialId,
        Guid? empleadoId,
        Guid? puntoAccesoId)
    {
        if (!Enum.IsDefined(tipo))
            throw new DomainException("El tipo de movimiento debe ser Entrada o Salida.");

        Id = Guid.NewGuid();
        CodigoCredencial = codigoCredencial.Trim().ToUpperInvariant();
        CodigoPuntoAcceso = codigoPuntoAcceso.Trim().ToUpperInvariant();
        Tipo = tipo;
        Resultado = resultado;
        Motivo = motivo;
        CredencialId = credencialId;
        EmpleadoId = empleadoId;
        PuntoAccesoId = puntoAccesoId;
        RegistradoEnUtc = DateTime.UtcNow;
    }

    public string CodigoCredencial { get; private set; } = string.Empty;
    public string CodigoPuntoAcceso { get; private set; } = string.Empty;
    public TipoMovimiento Tipo { get; private set; }
    public ResultadoAcceso Resultado { get; private set; }
    public string Motivo { get; private set; } = string.Empty;
    public DateTime RegistradoEnUtc { get; private set; }
    public Guid? CredencialId { get; private set; }
    public Guid? EmpleadoId { get; private set; }
    public Guid? PuntoAccesoId { get; private set; }
    public Credencial? Credencial { get; private set; }
    public Empleado? Empleado { get; private set; }
    public PuntoAcceso? PuntoAcceso { get; private set; }

    public static MovimientoAcceso Crear(
        string codigoCredencial,
        string codigoPuntoAcceso,
        TipoMovimiento tipo,
        ResultadoAcceso resultado,
        string motivo,
        Guid? credencialId = null,
        Guid? empleadoId = null,
        Guid? puntoAccesoId = null) =>
        new(codigoCredencial, codigoPuntoAcceso, tipo, resultado, motivo,
            credencialId, empleadoId, puntoAccesoId);
}

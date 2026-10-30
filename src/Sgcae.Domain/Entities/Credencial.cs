using Sgcae.Domain.Common;
using Sgcae.Domain.Enums;

namespace Sgcae.Domain.Entities;

public sealed class Credencial : Entity
{
    private Credencial() { }

    private Credencial(string codigo, Guid empleadoId)
    {
        var limpio = codigo?.Trim().ToUpperInvariant() ?? string.Empty;
        if (string.IsNullOrWhiteSpace(limpio) || limpio.Length > 50)
            throw new DomainException("El código de la credencial es obligatorio y no puede superar 50 caracteres.");
        if (empleadoId == Guid.Empty)
            throw new DomainException("La credencial debe pertenecer a un empleado.");

        Id = Guid.NewGuid();
        Codigo = limpio;
        EmpleadoId = empleadoId;
        Estado = EstadoCredencial.Vigente;
        AsignadaEnUtc = DateTime.UtcNow;
    }

    public string Codigo { get; private set; } = string.Empty;
    public Guid EmpleadoId { get; private set; }
    public Empleado Empleado { get; private set; } = null!;
    public EstadoCredencial Estado { get; private set; }
    public DateTime AsignadaEnUtc { get; private set; }
    public DateTime? ModificadaEnUtc { get; private set; }

    public static Credencial Asignar(string codigo, Guid empleadoId) => new(codigo, empleadoId);

    public void Bloquear()
    {
        if (Estado == EstadoCredencial.Revocada)
            throw new DomainException("Una credencial revocada no puede bloquearse.");
        if (Estado == EstadoCredencial.Bloqueada)
            throw new DomainException("La credencial ya se encuentra bloqueada.");

        Estado = EstadoCredencial.Bloqueada;
        ModificadaEnUtc = DateTime.UtcNow;
    }

    public void Revocar()
    {
        if (Estado == EstadoCredencial.Revocada)
            throw new DomainException("La credencial ya se encuentra revocada.");

        Estado = EstadoCredencial.Revocada;
        ModificadaEnUtc = DateTime.UtcNow;
    }
}

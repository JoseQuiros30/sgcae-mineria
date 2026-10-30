using Sgcae.Domain.Common;
using Sgcae.Domain.Enums;

namespace Sgcae.Domain.Entities;

public sealed class PuntoAcceso : Entity
{
    private PuntoAcceso() { }

    private PuntoAcceso(string codigo, string nombre, Guid sedeId, TipoPuntoAcceso tipo)
    {
        Codigo = Requerido(codigo, "código", 30).ToUpperInvariant();
        Nombre = Requerido(nombre, "nombre", 100);
        if (sedeId == Guid.Empty)
            throw new DomainException("El punto de acceso debe pertenecer a una sede.");
        if (!Enum.IsDefined(tipo))
            throw new DomainException("El tipo de punto de acceso no es válido.");

        Id = Guid.NewGuid();
        SedeId = sedeId;
        Tipo = tipo;
        Habilitado = true;
    }

    public string Codigo { get; private set; } = string.Empty;
    public string Nombre { get; private set; } = string.Empty;
    public Guid SedeId { get; private set; }
    public Sede Sede { get; private set; } = null!;
    public TipoPuntoAcceso Tipo { get; private set; }
    public bool Habilitado { get; private set; }

    public static PuntoAcceso Crear(string codigo, string nombre, Guid sedeId, TipoPuntoAcceso tipo) =>
        new(codigo, nombre, sedeId, tipo);

    public void CambiarEstado(bool habilitado) => Habilitado = habilitado;

    private static string Requerido(string valor, string campo, int maximo)
    {
        var limpio = valor?.Trim() ?? string.Empty;
        if (string.IsNullOrWhiteSpace(limpio) || limpio.Length > maximo)
            throw new DomainException($"El {campo} es obligatorio y no puede superar {maximo} caracteres.");
        return limpio;
    }
}

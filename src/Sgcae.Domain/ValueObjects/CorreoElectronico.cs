using System.Text.RegularExpressions;
using Sgcae.Domain.Common;

namespace Sgcae.Domain.ValueObjects;

public sealed record CorreoElectronico
{
    private static readonly Regex Formato = new(
        @"^[^\s@]+@[^\s@]+\.[^\s@]+$",
        RegexOptions.Compiled | RegexOptions.CultureInvariant);

    public string Valor { get; private set; } = string.Empty;

    private CorreoElectronico() { }

    private CorreoElectronico(string valor) => Valor = valor;

    public static CorreoElectronico Crear(string valor)
    {
        var normalizado = valor?.Trim().ToLowerInvariant() ?? string.Empty;
        if (!Formato.IsMatch(normalizado) || normalizado.Length > 150)
            throw new DomainException("El correo electrónico no tiene un formato válido.");

        return new CorreoElectronico(normalizado);
    }

    public override string ToString() => Valor;
}

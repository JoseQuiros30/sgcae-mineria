using Sgcae.Domain.Common;

namespace Sgcae.Domain.Entities;

public sealed class Sede : Entity
{
    private Sede() { }

    public Sede(Guid id, string codigo, string nombre)
    {
        Id = id;
        Codigo = codigo;
        Nombre = nombre;
    }

    public string Codigo { get; private set; } = string.Empty;
    public string Nombre { get; private set; } = string.Empty;
}

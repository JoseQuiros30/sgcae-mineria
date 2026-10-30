using Sgcae.Domain.Common;
using Sgcae.Domain.Entities;
using Sgcae.Domain.Enums;

namespace Sgcae.Domain.Tests;

public sealed class CredencialTests
{
    [Fact]
    public void Asignar_CreaCredencialVigente()
    {
        var credencial = Credencial.Asignar("  min-001 ", Guid.NewGuid());

        Assert.Equal("MIN-001", credencial.Codigo);
        Assert.Equal(EstadoCredencial.Vigente, credencial.Estado);
    }

    [Fact]
    public void Bloquear_CredencialVigente_CambiaEstado()
    {
        var credencial = Credencial.Asignar("MIN-001", Guid.NewGuid());

        credencial.Bloquear();

        Assert.Equal(EstadoCredencial.Bloqueada, credencial.Estado);
    }

    [Fact]
    public void Revocar_EsIrreversible()
    {
        var credencial = Credencial.Asignar("MIN-001", Guid.NewGuid());
        credencial.Revocar();

        Assert.Throws<DomainException>(() => credencial.Bloquear());
        Assert.Equal(EstadoCredencial.Revocada, credencial.Estado);
    }
}

using Sgcae.Domain.Entities;
using Sgcae.Domain.Enums;
using Sgcae.Domain.Common;

namespace Sgcae.Domain.Tests;

public sealed class PuntoAccesoTests
{
    [Fact]
    public void Crear_DejaPuntoHabilitadoYNormalizaCodigo()
    {
        var punto = PuntoAcceso.Crear(
            " gya-taller ", "Taller", Guid.NewGuid(), TipoPuntoAcceso.AreaOperativa);

        Assert.Equal("GYA-TALLER", punto.Codigo);
        Assert.True(punto.Habilitado);
    }

    [Fact]
    public void Crear_ConTipoNoDefinido_RechazaSolicitud()
    {
        var accion = () => PuntoAcceso.Crear(
            "GYA-TALLER", "Taller", Guid.NewGuid(), (TipoPuntoAcceso)999);

        Assert.Throws<DomainException>(accion);
    }
}

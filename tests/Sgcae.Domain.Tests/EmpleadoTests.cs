using Sgcae.Domain.Common;
using Sgcae.Domain.Entities;
using Sgcae.Domain.Enums;

namespace Sgcae.Domain.Tests;

public sealed class EmpleadoTests
{
    [Fact]
    public void Crear_ConDatosValidos_DejaEmpleadoActivo()
    {
        var empleado = CrearEmpleado();

        Assert.NotEqual(Guid.Empty, empleado.Id);
        Assert.Equal(EstadoLaboral.Activo, empleado.Estado);
        Assert.Equal("operador@minera.com", empleado.Correo.Valor);
    }

    [Fact]
    public void Crear_ConCorreoInvalido_LanzaExcepcionDeDominio()
    {
        var accion = () => Empleado.Crear(
            "102030", "Ana", "Gómez", "correo-invalido", "Operadora", "Operaciones");

        Assert.Throws<DomainException>(accion);
    }

    [Fact]
    public void Desactivar_CambiaEstadoAInactivo()
    {
        var empleado = CrearEmpleado();

        empleado.Desactivar();

        Assert.Equal(EstadoLaboral.Inactivo, empleado.Estado);
        Assert.NotNull(empleado.ActualizadoEnUtc);
    }

    [Fact]
    public void Desactivar_DosVeces_NoPermiteTransicionDuplicada()
    {
        var empleado = CrearEmpleado();
        empleado.Desactivar();

        Assert.Throws<DomainException>(() => empleado.Desactivar());
    }

    private static Empleado CrearEmpleado() => Empleado.Crear(
        "102030", "Ana", "Gómez", "Operador@Minera.com", "Operadora", "Operaciones");
}

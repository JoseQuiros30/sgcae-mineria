using Microsoft.EntityFrameworkCore;
using Sgcae.Domain.Entities;
using Sgcae.Infrastructure.Persistence;

namespace Sgcae.Domain.Tests;

public sealed class ModeloPersistenciaTests
{
    [Fact]
    public void Modelo_EfCore_ContieneTodasLasEntidades()
    {
        var options = new DbContextOptionsBuilder<SgcaeDbContext>()
            .UseSqlServer("Server=localhost;Database=SoloModelo;Trusted_Connection=True;TrustServerCertificate=True")
            .Options;
        using var context = new SgcaeDbContext(options);

        Assert.NotNull(context.Model.FindEntityType(typeof(Empleado)));
        Assert.NotNull(context.Model.FindEntityType(typeof(Credencial)));
        Assert.NotNull(context.Model.FindEntityType(typeof(Sede)));
        Assert.NotNull(context.Model.FindEntityType(typeof(PuntoAcceso)));
        Assert.NotNull(context.Model.FindEntityType(typeof(MovimientoAcceso)));
    }
}

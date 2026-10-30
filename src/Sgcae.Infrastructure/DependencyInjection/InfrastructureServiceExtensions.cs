using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Sgcae.Application.Abstractions;
using Sgcae.Infrastructure.Persistence;
using Sgcae.Infrastructure.Repositories;

namespace Sgcae.Infrastructure.DependencyInjection;

public static class InfrastructureServiceExtensions
{
    public static IServiceCollection AgregarInfraestructura(
        this IServiceCollection services, IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("Sgcae")
            ?? throw new InvalidOperationException("No se configuró la cadena de conexión 'Sgcae'.");

        services.AddDbContext<SgcaeDbContext>(options =>
            options.UseSqlServer(connectionString, sql => sql.EnableRetryOnFailure()));
        services.AddScoped<IUnidadTrabajo>(provider => provider.GetRequiredService<SgcaeDbContext>());
        services.AddScoped<IEmpleadoRepository, EmpleadoRepository>();
        services.AddScoped<ICredencialRepository, CredencialRepository>();
        services.AddScoped<IPuntoAccesoRepository, PuntoAccesoRepository>();
        services.AddScoped<IMovimientoAccesoRepository, MovimientoAccesoRepository>();
        return services;
    }

    public static async Task InicializarBaseDatosAsync(this IServiceProvider services)
    {
        await using var scope = services.CreateAsyncScope();
        var context = scope.ServiceProvider.GetRequiredService<SgcaeDbContext>();
        await context.Database.EnsureCreatedAsync();
    }
}

using Microsoft.Extensions.DependencyInjection;
using Sgcae.Application.Accesos;
using Sgcae.Application.Credenciales;
using Sgcae.Application.Empleados;
using Sgcae.Application.PuntosAcceso;

namespace Sgcae.Application.DependencyInjection;

public static class ApplicationServiceExtensions
{
    public static IServiceCollection AgregarAplicacion(this IServiceCollection services)
    {
        services.AddScoped<RegistrarEmpleadoHandler>();
        services.AddScoped<ActualizarEmpleadoHandler>();
        services.AddScoped<DesactivarEmpleadoHandler>();
        services.AddScoped<ConsultarEmpleadoHandler>();
        services.AddScoped<ConsultarEmpleadosHandler>();
        services.AddScoped<AsignarCredencialHandler>();
        services.AddScoped<CambiarEstadoCredencialHandler>();
        services.AddScoped<ConsultarCredencialesHandler>();
        services.AddScoped<ConsultarCredencialHandler>();
        services.AddScoped<RegistrarPuntoAccesoHandler>();
        services.AddScoped<CambiarEstadoPuntoAccesoHandler>();
        services.AddScoped<ConsultarPuntosAccesoHandler>();
        services.AddScoped<ConsultarPuntoAccesoHandler>();
        services.AddScoped<ConsultarSedesHandler>();
        services.AddScoped<RegistrarAccesoHandler>();
        services.AddScoped<ConsultarHistorialAccesosHandler>();
        return services;
    }
}

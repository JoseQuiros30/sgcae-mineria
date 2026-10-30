using System.Text.Json.Serialization;
using Microsoft.OpenApi.Models;
using Sgcae.Api.Middleware;
using Sgcae.Application.DependencyInjection;
using Sgcae.Infrastructure.DependencyInjection;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers().AddJsonOptions(options =>
    options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter()));
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "SGCAE API",
        Version = "v1",
        Description = "Sistema de Gestión y Control de Acceso para Mina Guayabito, Mina Guaico y Relavera."
    });
});
builder.Services.AddHealthChecks();
builder.Services.AgregarAplicacion();
builder.Services.AgregarInfraestructura(builder.Configuration);

var app = builder.Build();

app.UseMiddleware<ExceptionMiddleware>();
app.UseSwagger();
app.UseSwaggerUI();
app.UseHttpsRedirection();
app.MapControllers();
app.MapHealthChecks("/health");

await app.Services.InicializarBaseDatosAsync();
await app.RunAsync();

public partial class Program;

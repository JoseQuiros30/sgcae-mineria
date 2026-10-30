using System.Net;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Sgcae.Application.Common;
using Sgcae.Domain.Common;

namespace Sgcae.Api.Middleware;

public sealed class ExceptionMiddleware(RequestDelegate next, ILogger<ExceptionMiddleware> logger)
{
    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await next(context);
        }
        catch (Exception exception)
        {
            await EscribirErrorAsync(context, exception);
        }
    }

    private async Task EscribirErrorAsync(HttpContext context, Exception exception)
    {
        var (status, titulo) = exception switch
        {
            RecursoNoEncontradoException => (HttpStatusCode.NotFound, "Recurso no encontrado"),
            ConflictoException => (HttpStatusCode.Conflict, "Conflicto con el estado actual"),
            DbUpdateException => (HttpStatusCode.Conflict, "Conflicto de persistencia"),
            DomainException => (HttpStatusCode.BadRequest, "Regla de negocio no satisfecha"),
            ArgumentException => (HttpStatusCode.BadRequest, "Solicitud inválida"),
            _ => (HttpStatusCode.InternalServerError, "Error interno del servidor")
        };

        if (status == HttpStatusCode.InternalServerError)
            logger.LogError(exception, "Error no controlado al procesar {Metodo} {Ruta}",
                context.Request.Method, context.Request.Path);

        var problem = new ProblemDetails
        {
            Status = (int)status,
            Title = titulo,
            Detail = status == HttpStatusCode.InternalServerError
                ? "Ocurrió un error inesperado. Consulte los registros del servidor."
                : exception.Message,
            Instance = context.Request.Path
        };
        problem.Extensions["traceId"] = context.TraceIdentifier;

        context.Response.StatusCode = (int)status;
        context.Response.ContentType = "application/problem+json";
        await context.Response.WriteAsJsonAsync(problem);
    }
}

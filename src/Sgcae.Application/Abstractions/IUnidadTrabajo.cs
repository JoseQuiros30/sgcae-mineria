namespace Sgcae.Application.Abstractions;

public interface IUnidadTrabajo
{
    Task<int> GuardarCambiosAsync(CancellationToken cancellationToken = default);
}

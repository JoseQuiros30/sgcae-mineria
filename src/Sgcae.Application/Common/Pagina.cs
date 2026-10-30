namespace Sgcae.Application.Common;

public sealed record Pagina<T>(
    IReadOnlyCollection<T> Elementos,
    int PaginaActual,
    int TamanoPagina,
    int TotalElementos)
{
    public int TotalPaginas => TotalElementos == 0
        ? 0
        : (int)Math.Ceiling(TotalElementos / (double)TamanoPagina);
}

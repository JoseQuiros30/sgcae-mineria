namespace Sgcae.Application.Common;

public static class Paginacion
{
    public static (int Pagina, int Tamano) Normalizar(int pagina, int tamano) =>
        (Math.Max(1, pagina), tamano <= 0 ? 20 : Math.Clamp(tamano, 1, 100));
}

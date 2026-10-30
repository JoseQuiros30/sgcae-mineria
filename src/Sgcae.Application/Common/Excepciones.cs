namespace Sgcae.Application.Common;

public sealed class RecursoNoEncontradoException(string mensaje) : Exception(mensaje);
public sealed class ConflictoException(string mensaje) : Exception(mensaje);

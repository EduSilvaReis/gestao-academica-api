namespace GestaoAcademica.Application.Common;

public sealed class NotFoundException(string entidade, Guid id)
    : Exception($"{entidade} com id '{id}' não foi encontrado(a).");
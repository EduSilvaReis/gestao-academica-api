using GestaoAcademica.Domain.Common;
using GestaoAcademica.Domain.Enums;
using GestaoAcademica.Domain.Exceptions;

namespace GestaoAcademica.Domain.Entities;

public class Nota : Entity
{
    public const decimal ValorMinimo = 0m;
    public const decimal ValorMaximo = 10m;
    public const decimal PesoMaximo = 10m;

    public Guid MatriculaId { get; private set; }
    public TipoAvaliacao Avaliacao { get; private set; }
    public decimal Valor { get; private set; }
    public decimal Peso { get; private set; }

    private Nota() { }

    internal Nota(Guid matriculaId, TipoAvaliacao avaliacao, decimal valor, decimal peso)
    {
        if (!Enum.IsDefined(avaliacao))
            throw new DomainException("Tipo de avaliação inválido.");

        // depois
        if (peso <= 0 || peso > PesoMaximo)
            throw new DomainException($"O peso deve ser maior que zero e no máximo {PesoMaximo}.");

        MatriculaId = matriculaId;
        Avaliacao = avaliacao;
        Peso = peso;
        DefinirValor(valor);
    }

    internal void AlterarValor(decimal valor) => DefinirValor(valor);

    private void DefinirValor(decimal valor)
    {
        if (valor < ValorMinimo || valor > ValorMaximo)
            throw new DomainException($"A nota deve estar entre {ValorMinimo} e {ValorMaximo}.");

        if (decimal.Round(valor, 2) != valor)
            throw new DomainException("A nota deve ter no máximo duas casas decimais.");

        Valor = valor;
    }
}
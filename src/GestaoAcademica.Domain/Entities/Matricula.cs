using GestaoAcademica.Domain.Common;
using GestaoAcademica.Domain.Enums;
using GestaoAcademica.Domain.Exceptions;

namespace GestaoAcademica.Domain.Entities;

public class Matricula : Entity
{
    private readonly List<Nota> _notas = [];

    public Guid AlunoId { get; private set; }
    public Guid TurmaId { get; private set; }
    public DateOnly DataMatricula { get; private set; }
    public StatusMatricula Status { get; private set; }
    public IReadOnlyCollection<Nota> Notas => _notas.AsReadOnly();

    private Matricula() { }

    // internal: somente a Turma (mesmo projeto) pode criar uma matrícula.
    internal Matricula(Guid alunoId, Guid turmaId)
    {
        AlunoId = alunoId;
        TurmaId = turmaId;
        DataMatricula = DateOnly.FromDateTime(DateTime.Today);
        Status = StatusMatricula.Ativa;
    }

    public Nota LancarNota(TipoAvaliacao avaliacao, decimal valor, decimal peso)
    {
        GarantirAtiva();

        if (_notas.Any(n => n.Avaliacao == avaliacao))
            throw new DomainException($"Já existe nota lançada para a avaliação {avaliacao}.");

        var nota = new Nota(Id, avaliacao, valor, peso);
        _notas.Add(nota);
        return nota;
    }

    public void CorrigirNota(TipoAvaliacao avaliacao, decimal novoValor)
    {
        GarantirAtiva();

        var nota = _notas.SingleOrDefault(n => n.Avaliacao == avaliacao)
            ?? throw new DomainException($"Não há nota lançada para a avaliação {avaliacao}.");

        nota.AlterarValor(novoValor);
    }

    public decimal? CalcularMedia()
    {
        if (_notas.Count == 0)
            return null;

        var somaPesos = _notas.Sum(n => n.Peso);
        var somaPonderada = _notas.Sum(n => n.Valor * n.Peso);

        return Math.Round(somaPonderada / somaPesos, 2, MidpointRounding.AwayFromZero);
    }

    public void Cancelar()
    {
        if (Status == StatusMatricula.Cancelada)
            throw new DomainException("A matrícula já está cancelada.");

        Status = StatusMatricula.Cancelada;
    }

    private void GarantirAtiva()
    {
        if (Status != StatusMatricula.Ativa)
            throw new DomainException("Operação não permitida em matrícula cancelada.");
    }
}
using System.Text.RegularExpressions;

using GestaoAcademica.Domain.Common;
using GestaoAcademica.Domain.Enums;
using GestaoAcademica.Domain.Exceptions;

namespace GestaoAcademica.Domain.Entities;

public partial class Turma : Entity
{
    public const int CodigoTamanhoMaximo = 20;
    public const int DisciplinaTamanhoMaximo = 100;

    private readonly List<Matricula> _matriculas = [];

    public string Codigo { get; private set; } = string.Empty;
    public string Disciplina { get; private set; } = string.Empty;
    public string Semestre { get; private set; } = string.Empty;
    public int Vagas { get; private set; }
    public IReadOnlyCollection<Matricula> Matriculas => _matriculas.AsReadOnly();

    public int VagasDisponiveis => Vagas - ContarMatriculasAtivas();

    private Turma() { }

    public Turma(string codigo, string disciplina, string semestre, int vagas)
    {
        if (string.IsNullOrWhiteSpace(semestre) || !SemestreRegex().IsMatch(semestre.Trim()))
            throw new DomainException("O semestre deve estar no formato AAAA.1 ou AAAA.2.");

        Codigo = ValidarTexto(codigo, "código", CodigoTamanhoMaximo).ToUpperInvariant();
        Disciplina = ValidarTexto(disciplina, "disciplina", DisciplinaTamanhoMaximo);
        Semestre = semestre.Trim();
        DefinirVagas(vagas);
    }

    public void AlterarVagas(int vagas) => DefinirVagas(vagas);

    public Matricula Matricular(Aluno aluno)
    {
        if (!aluno.Ativo)
            throw new DomainException("Aluno inativo não pode ser matriculado.");

        if (_matriculas.Any(m => m.AlunoId == aluno.Id && m.Status == StatusMatricula.Ativa))
            throw new DomainException("O aluno já está matriculado nesta turma.");

        if (VagasDisponiveis <= 0)
            throw new DomainException("A turma não possui vagas disponíveis.");

        var matricula = new Matricula(aluno.Id, Id);
        _matriculas.Add(matricula);
        return matricula;
    }

    private void DefinirVagas(int vagas)
    {
        if (vagas <= 0)
            throw new DomainException("A turma deve ter pelo menos uma vaga.");

        var ativas = ContarMatriculasAtivas();
        if (vagas < ativas)
            throw new DomainException($"A turma já tem {ativas} matrículas ativas. As vagas não podem ser menores que isso.");

        Vagas = vagas;
    }

    private int ContarMatriculasAtivas() =>
        _matriculas.Count(m => m.Status == StatusMatricula.Ativa);

    private static string ValidarTexto(string valor, string campo, int tamanhoMaximo)
    {
        if (string.IsNullOrWhiteSpace(valor))
            throw new DomainException($"O campo {campo} é obrigatório.");

        valor = valor.Trim();

        if (valor.Length > tamanhoMaximo)
            throw new DomainException($"O campo {campo} deve ter no máximo {tamanhoMaximo} caracteres.");

        return valor;
    }

    [GeneratedRegex(@"^\d{4}\.[12]$")]
    private static partial Regex SemestreRegex();
}
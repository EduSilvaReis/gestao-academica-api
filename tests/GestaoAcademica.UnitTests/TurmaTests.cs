using GestaoAcademica.Domain.Entities;
using GestaoAcademica.Domain.Enums;
using GestaoAcademica.Domain.Exceptions;

namespace GestaoAcademica.UnitTests.Domain;

public class TurmaTests
{
    private static Aluno CriarAluno(string ra = "2026001") =>
        new("Maria Souza", $"aluno{ra}@email.com", ra, new DateOnly(2004, 5, 10));

    private static Turma CriarTurma(int vagas = 30) =>
        new("ES101", "Engenharia de Software", "2026.2", vagas);

    [Theory]
    [InlineData("2026")]
    [InlineData("2026.3")]
    [InlineData("26.1")]
    public void Criar_ComSemestreInvalido_DeveLancarDomainException(string semestre)
    {
        Assert.Throws<DomainException>(() =>
            new Turma("ES101", "Engenharia de Software", semestre, 30));
    }

    [Fact]
    public void Criar_ComZeroVagas_DeveLancarDomainException()
    {
        Assert.Throws<DomainException>(() => CriarTurma(vagas: 0));
    }

    [Fact]
    public void Matricular_AlunoAtivo_DeveCriarMatriculaAtivaEReduzirVagas()
    {
        var turma = CriarTurma(vagas: 30);
        var aluno = CriarAluno();

        var matricula = turma.Matricular(aluno);

        Assert.Equal(StatusMatricula.Ativa, matricula.Status);
        Assert.Equal(aluno.Id, matricula.AlunoId);
        Assert.Equal(29, turma.VagasDisponiveis);
    }

    [Fact]
    public void Matricular_AlunoInativo_DeveLancarDomainException()
    {
        var turma = CriarTurma();
        var aluno = CriarAluno();
        aluno.Desativar();

        Assert.Throws<DomainException>(() => turma.Matricular(aluno));
    }

    [Fact]
    public void Matricular_AlunoJaMatriculado_DeveLancarDomainException()
    {
        var turma = CriarTurma();
        var aluno = CriarAluno();
        turma.Matricular(aluno);

        Assert.Throws<DomainException>(() => turma.Matricular(aluno));
    }

    [Fact]
    public void Matricular_TurmaSemVagas_DeveLancarDomainException()
    {
        var turma = CriarTurma(vagas: 1);
        turma.Matricular(CriarAluno("2026001"));

        Assert.Throws<DomainException>(() => turma.Matricular(CriarAluno("2026002")));
    }

    [Fact]
    public void Matricular_AposCancelamento_DevePermitirNovaMatricula()
    {
        var turma = CriarTurma(vagas: 1);
        var aluno = CriarAluno();
        turma.Matricular(aluno).Cancelar();

        var novaMatricula = turma.Matricular(aluno);

        Assert.Equal(StatusMatricula.Ativa, novaMatricula.Status);
        Assert.Equal(0, turma.VagasDisponiveis);
    }

    [Fact]
    public void AlterarVagas_MenorQueMatriculasAtivas_DeveLancarDomainException()
    {
        var turma = CriarTurma(vagas: 5);
        turma.Matricular(CriarAluno("2026001"));
        turma.Matricular(CriarAluno("2026002"));

        Assert.Throws<DomainException>(() => turma.AlterarVagas(1));
    }
}
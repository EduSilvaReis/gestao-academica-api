using GestaoAcademica.Domain.Entities;
using GestaoAcademica.Domain.Enums;
using GestaoAcademica.Domain.Exceptions;

namespace GestaoAcademica.UnitTests.Domain;

public class MatriculaTests
{
    private static Matricula CriarMatricula()
    {
        var turma = new Turma("ES101", "Engenharia de Software", "2026.2", 30);
        var aluno = new Aluno("Maria Souza", "maria@email.com", "2026001", new DateOnly(2004, 5, 10));
        return turma.Matricular(aluno);
    }

    [Theory]
    [InlineData(-1.0)]
    [InlineData(10.01)]
    public void LancarNota_ForaDoIntervalo_DeveLancarDomainException(double valor)
    {
        var matricula = CriarMatricula();

        Assert.Throws<DomainException>(() =>
            matricula.LancarNota(TipoAvaliacao.P1, (decimal)valor, 1m));
    }

    [Fact]
    public void LancarNota_ComMaisDeDuasCasasDecimais_DeveLancarDomainException()
    {
        var matricula = CriarMatricula();

        Assert.Throws<DomainException>(() =>
            matricula.LancarNota(TipoAvaliacao.P1, 7.555m, 1m));
    }

    [Fact]
    public void LancarNota_AvaliacaoRepetida_DeveLancarDomainException()
    {
        var matricula = CriarMatricula();
        matricula.LancarNota(TipoAvaliacao.P1, 7m, 1m);

        Assert.Throws<DomainException>(() =>
            matricula.LancarNota(TipoAvaliacao.P1, 8m, 1m));
    }

    [Fact]
    public void LancarNota_EmMatriculaCancelada_DeveLancarDomainException()
    {
        var matricula = CriarMatricula();
        matricula.Cancelar();

        Assert.Throws<DomainException>(() =>
            matricula.LancarNota(TipoAvaliacao.P1, 7m, 1m));
    }

    [Fact]
    public void CalcularMedia_SemNotas_DeveRetornarNull()
    {
        var matricula = CriarMatricula();

        Assert.Null(matricula.CalcularMedia());
    }

    [Fact]
    public void CalcularMedia_ComPesos_DeveRetornarMediaPonderada()
    {
        var matricula = CriarMatricula();
        matricula.LancarNota(TipoAvaliacao.P1, 6m, 2m);
        matricula.LancarNota(TipoAvaliacao.P2, 8m, 3m);
        matricula.LancarNota(TipoAvaliacao.Trabalho, 9m, 1m);

        var media = matricula.CalcularMedia();

        // (6×2 + 8×3 + 9×1) / (2+3+1) = 45 / 6 = 7,5
        Assert.Equal(7.5m, media);
    }

    [Fact]
    public void CorrigirNota_DeveAtualizarValor()
    {
        var matricula = CriarMatricula();
        matricula.LancarNota(TipoAvaliacao.P1, 5m, 1m);

        matricula.CorrigirNota(TipoAvaliacao.P1, 6.5m);

        Assert.Equal(6.5m, matricula.Notas.Single().Valor);
    }

    [Fact]
    public void Cancelar_MatriculaJaCancelada_DeveLancarDomainException()
    {
        var matricula = CriarMatricula();
        matricula.Cancelar();

        Assert.Throws<DomainException>(() => matricula.Cancelar());
    }
}
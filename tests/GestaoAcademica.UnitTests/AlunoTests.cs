using GestaoAcademica.Domain.Entities;
using GestaoAcademica.Domain.Exceptions;

namespace GestaoAcademica.UnitTests.Domain;

public class AlunoTests
{
    private static readonly DateOnly NascimentoValido = new(2004, 5, 10);

    [Fact]
    public void Criar_ComDadosValidos_DeveFicarAtivoEComId()
    {
        var aluno = new Aluno("Maria Souza", "maria@email.com", "2026001", NascimentoValido);

        Assert.True(aluno.Ativo);
        Assert.NotEqual(Guid.Empty, aluno.Id);
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    public void Criar_ComNomeVazio_DeveLancarDomainException(string nome)
    {
        Assert.Throws<DomainException>(() =>
            new Aluno(nome, "maria@email.com", "2026001", NascimentoValido));
    }

    [Fact]
    public void Criar_ComNomeMaiorQueOLimite_DeveLancarDomainException()
    {
        var nomeLongo = new string('A', Aluno.NomeTamanhoMaximo + 1);

        Assert.Throws<DomainException>(() =>
            new Aluno(nomeLongo, "maria@email.com", "2026001", NascimentoValido));
    }

    [Theory]
    [InlineData("")]
    [InlineData("sem-arroba")]
    public void Criar_ComEmailInvalido_DeveLancarDomainException(string email)
    {
        Assert.Throws<DomainException>(() =>
            new Aluno("Maria Souza", email, "2026001", NascimentoValido));
    }

    [Fact]
    public void Criar_DeveNormalizarEmailEmMinusculasESemEspacos()
    {
        var aluno = new Aluno("Maria Souza", "  Maria@Email.COM ", "2026001", NascimentoValido);

        Assert.Equal("maria@email.com", aluno.Email);
    }

    [Fact]
    public void Criar_ComDataDeNascimentoFutura_DeveLancarDomainException()
    {
        Assert.Throws<DomainException>(() =>
            new Aluno("Maria Souza", "maria@email.com", "2026001", new DateOnly(2100, 1, 1)));
    }

    [Fact]
    public void Desativar_DeveMarcarAlunoComoInativo()
    {
        var aluno = new Aluno("Maria Souza", "maria@email.com", "2026001", NascimentoValido);

        aluno.Desativar();

        Assert.False(aluno.Ativo);
    }
}
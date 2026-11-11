using System.Net.Mail;

using GestaoAcademica.Domain.Common;
using GestaoAcademica.Domain.Exceptions;

namespace GestaoAcademica.Domain.Entities;

public class Aluno : Entity
{
    public const int NomeTamanhoMaximo = 150;

    public string Nome { get; private set; } = string.Empty;
    public string Email { get; private set; } = string.Empty;
    public string RA { get; private set; } = string.Empty;
    public DateOnly DataNascimento { get; private set; }
    public bool Ativo { get; private set; }

    // Usado pelo EF Core para montar a entidade a partir do banco.
    private Aluno() { }

    public Aluno(string nome, string email, string ra, DateOnly dataNascimento)
    {
        if (string.IsNullOrWhiteSpace(ra))
            throw new DomainException("O RA é obrigatório.");

        if (dataNascimento > DateOnly.FromDateTime(DateTime.Today))
            throw new DomainException("A data de nascimento não pode estar no futuro.");

        DefinirNome(nome);
        DefinirEmail(email);
        RA = ra.Trim();
        DataNascimento = dataNascimento;
        Ativo = true;
    }

    public void AtualizarDados(string nome, string email)
    {
        DefinirNome(nome);
        DefinirEmail(email);
    }

    public void Desativar() => Ativo = false;

    public void Ativar() => Ativo = true;

    private void DefinirNome(string nome)
    {
        if (string.IsNullOrWhiteSpace(nome))
            throw new DomainException("O nome é obrigatório.");

        nome = nome.Trim();

        if (nome.Length > NomeTamanhoMaximo)
            throw new DomainException($"O nome deve ter no máximo {NomeTamanhoMaximo} caracteres.");

        Nome = nome;
    }

    private void DefinirEmail(string email)
    {
        if (string.IsNullOrWhiteSpace(email) || !MailAddress.TryCreate(email.Trim(), out _))
            throw new DomainException("E-mail inválido.");

        Email = email.Trim().ToLowerInvariant();
    }
}
using System.ComponentModel.DataAnnotations;

using GestaoAcademica.Domain.Entities;

namespace GestaoAcademica.Application.Alunos;

public sealed record CriarAlunoRequest(
    [Required, MaxLength(Aluno.NomeTamanhoMaximo)] string Nome,
    [Required, EmailAddress, MaxLength(Aluno.EmailTamanhoMaximo)] string Email,
    [Required, MaxLength(Aluno.RATamanhoMaximo)] string RA,
    [Required] DateOnly DataNascimento);

public sealed record AtualizarAlunoRequest(
    [Required, MaxLength(Aluno.NomeTamanhoMaximo)] string Nome,
    [Required, EmailAddress, MaxLength(Aluno.EmailTamanhoMaximo)] string Email);

public sealed record AlunoResponse(
    Guid Id,
    string Nome,
    string Email,
    string RA,
    DateOnly DataNascimento,
    bool Ativo);
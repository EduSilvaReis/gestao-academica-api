using System.ComponentModel.DataAnnotations;

using GestaoAcademica.Domain.Entities;

namespace GestaoAcademica.Application.Turmas;

// O que o cliente envia para criar uma turma
public sealed record CriarTurmaRequest(
    [Required, MaxLength(Turma.CodigoTamanhoMaximo)] string Codigo,
    [Required, MaxLength(Turma.DisciplinaTamanhoMaximo)] string Disciplina,
    [Required, RegularExpression(@"^\d{4}\.[12]$", ErrorMessage = "Use o formato AAAA.1 ou AAAA.2")] string Semestre,
    [Range(1, 500)] int Vagas);

// Só as vagas podem mudar depois de criada; código, disciplina e semestre identificam a turma
public sealed record AlterarVagasRequest([Range(1, 500)] int Vagas);

// O que a API devolve. VagasDisponiveis é calculado, não vem de coluna.
public sealed record TurmaResponse(
    Guid Id,
    string Codigo,
    string Disciplina,
    string Semestre,
    int Vagas,
    int VagasDisponiveis);
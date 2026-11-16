using System.ComponentModel.DataAnnotations;

using GestaoAcademica.Domain.Enums;

namespace GestaoAcademica.Application.Matriculas;

public sealed record MatricularRequest(
    [Required] Guid AlunoId,
    [Required] Guid TurmaId);

// Traz os nomes de aluno e turma, obtidos por join na consulta, para a resposta ser útil sem chamadas extras
public sealed record MatriculaResponse(
    Guid Id,
    Guid AlunoId,
    string AlunoNome,
    Guid TurmaId,
    string TurmaCodigo,
    DateOnly DataMatricula,
    StatusMatricula Status);
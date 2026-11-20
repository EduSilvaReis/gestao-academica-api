namespace GestaoAcademica.Application.Relatorios;

// Um aluno que está abaixo da média mínima
public sealed record AlunoEmRiscoResponse(
    Guid AlunoId,
    string Nome,
    decimal Media);

// O relatório completo da turma
public sealed record DesempenhoTurmaResponse(
    Guid TurmaId,
    string Codigo,
    string Disciplina,
    string Semestre,
    decimal MediaMinimaAprovacao,   // devolvida para o cliente saber qual régua foi usada
    int TotalAlunos,                // matrículas ativas
    int AlunosSemNotas,             // ainda não avaliados: nem aprovados nem em risco
    decimal? MediaTurma,            // média das médias; null se ninguém tem nota
    int Aprovados,
    IReadOnlyList<AlunoEmRiscoResponse> AlunosEmRisco);
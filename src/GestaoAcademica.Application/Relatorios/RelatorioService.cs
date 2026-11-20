using GestaoAcademica.Application.Abstractions;
using GestaoAcademica.Application.Common;
using GestaoAcademica.Domain.Entities;
using GestaoAcademica.Domain.Enums;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

namespace GestaoAcademica.Application.Relatorios;

// Recebe o banco e a configuração do relatório pela injeção de dependência.
public sealed class RelatorioService(IAppDbContext db, IOptions<RelatorioOptions> options) : IRelatorioService
{
    public async Task<DesempenhoTurmaResponse> DesempenhoTurmaAsync(Guid turmaId, CancellationToken ct)
    {
        // Lê a nota mínima do appsettings.json.
        var mediaMinima = options.Value.MediaMinimaAprovacao;

        // 1ª consulta: só os dados de cabeçalho da turma. Se não existir, 404.
        var turma = await db.Turmas.AsNoTracking()
            .Where(t => t.Id == turmaId)
            .Select(t => new { t.Id, t.Codigo, t.Disciplina, t.Semestre })
            .FirstOrDefaultAsync(ct)
            ?? throw new NotFoundException(nameof(Turma), turmaId);

        // 2ª consulta: UMA linha por aluno ativo, com as somas já feitas pelo SQL Server.
        // O banco NÃO devolve as notas, só os totais: soma ponderada, soma dos pesos e quantidade.
        var linhas = await db.Matriculas.AsNoTracking()
            .Where(m => m.TurmaId == turmaId && m.Status == StatusMatricula.Ativa)
            .Select(m => new
            {
                m.AlunoId,
                // Nome do aluno por subconsulta (mesma técnica da Etapa 4)
                Nome = db.Alunos.Where(a => a.Id == m.AlunoId).Select(a => a.Nome).First(),
                // SUM(Valor * Peso) e SUM(Peso) das notas desta matrícula
                SomaPonderada = m.Notas.Sum(n => n.Valor * n.Peso),
                SomaPesos = m.Notas.Sum(n => n.Peso),
                QuantidadeNotas = m.Notas.Count()
            })
            .ToListAsync(ct);

        // A partir daqui é cálculo em memória, sobre poucas linhas (uma por aluno).

        // Calcula a média de cada aluno que tem pelo menos uma nota.
        // Mesmo arredondamento da entidade Matricula: 2 casas, AwayFromZero.
        var medias = linhas
            .Where(l => l.QuantidadeNotas > 0)
            .Select(l => new
            {
                l.AlunoId,
                l.Nome,
                Media = Math.Round(l.SomaPonderada / l.SomaPesos, 2, MidpointRounding.AwayFromZero)
            })
            .ToList();

        // Alunos abaixo da média mínima, do pior para o melhor.
        var emRisco = medias
            .Where(m => m.Media < mediaMinima)
            .OrderBy(m => m.Media)
            .Select(m => new AlunoEmRiscoResponse(m.AlunoId, m.Nome, m.Media))
            .ToList();

        // Média da turma = média das médias dos alunos avaliados. Null se ninguém tem nota.
        decimal? mediaTurma = medias.Count > 0
            ? Math.Round(medias.Average(m => m.Media), 2, MidpointRounding.AwayFromZero)
            : null;

        return new DesempenhoTurmaResponse(
            turma.Id, turma.Codigo, turma.Disciplina, turma.Semestre,
            mediaMinima,
            TotalAlunos: linhas.Count,
            AlunosSemNotas: linhas.Count - medias.Count,
            MediaTurma: mediaTurma,
            Aprovados: medias.Count(m => m.Media >= mediaMinima),
            AlunosEmRisco: emRisco);
    }
}
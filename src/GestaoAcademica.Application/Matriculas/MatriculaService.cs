using GestaoAcademica.Application.Abstractions;
using GestaoAcademica.Application.Common;
using GestaoAcademica.Domain.Entities;

using Microsoft.EntityFrameworkCore;

namespace GestaoAcademica.Application.Matriculas;

public sealed class MatriculaService(IAppDbContext db) : IMatriculaService
{
    public async Task<PagedResult<MatriculaResponse>> ListarPorTurmaAsync(Guid turmaId, int page, int pageSize, CancellationToken ct)
    {
        if (!await db.Turmas.AnyAsync(t => t.Id == turmaId, ct))
            throw new NotFoundException(nameof(Turma), turmaId);

        var query = db.Matriculas.AsNoTracking().Where(m => m.TurmaId == turmaId);

        var total = await query.CountAsync(ct);

        var items = await query
            .OrderBy(m => m.DataMatricula)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            // Projeção com join: as subconsultas em Alunos e Turmas viram SELECTs correlacionados no SQL.
            // Tudo roda numa consulta só.
            .Select(m => new MatriculaResponse(
                m.Id,
                m.AlunoId,
                db.Alunos.Where(a => a.Id == m.AlunoId).Select(a => a.Nome).First(),
                m.TurmaId,
                db.Turmas.Where(t => t.Id == m.TurmaId).Select(t => t.Codigo).First(),
                m.DataMatricula,
                m.Status))
            .ToListAsync(ct);

        return new PagedResult<MatriculaResponse>(items, page, pageSize, total);
    }

    public async Task<MatriculaResponse> ObterAsync(Guid id, CancellationToken ct) =>
        await db.Matriculas.AsNoTracking()
            .Where(m => m.Id == id)
            // A mesma projeção, para um registro só
            .Select(m => new MatriculaResponse(
                m.Id,
                m.AlunoId,
                db.Alunos.Where(a => a.Id == m.AlunoId).Select(a => a.Nome).First(),
                m.TurmaId,
                db.Turmas.Where(t => t.Id == m.TurmaId).Select(t => t.Codigo).First(),
                m.DataMatricula,
                m.Status))
            .FirstOrDefaultAsync(ct)
            ?? throw new NotFoundException(nameof(Matricula), id);

    public async Task<MatriculaResponse> MatricularAsync(MatricularRequest request, CancellationToken ct)
    {
        var aluno = await db.Alunos.FirstOrDefaultAsync(a => a.Id == request.AlunoId, ct)
            ?? throw new NotFoundException(nameof(Aluno), request.AlunoId);

        // PONTO-CHAVE: Include(Matriculas). A entidade Turma verifica vagas e duplicidade
        // olhando a própria lista. Sem o Include, a lista viria vazia e toda regra passaria.
        var turma = await db.Turmas
            .Include(t => t.Matriculas)
            .FirstOrDefaultAsync(t => t.Id == request.TurmaId, ct)
            ?? throw new NotFoundException(nameof(Turma), request.TurmaId);

        // A regra inteira (aluno ativo, vaga, duplicidade) está no domínio.
        // Qualquer violação lança DomainException → 422.
        var matricula = turma.Matricular(aluno);

        // Não precisa de db.Matriculas.Add: a matrícula entrou na coleção da turma rastreada,
        // e o EF detecta a entidade nova ao salvar. É aqui que o ValueGeneratedNever importa.
        await db.SaveChangesAsync(ct);

        return new MatriculaResponse(
            matricula.Id, aluno.Id, aluno.Nome, turma.Id, turma.Codigo,
            matricula.DataMatricula, matricula.Status);
    }

    public async Task CancelarAsync(Guid id, CancellationToken ct)
    {
        var matricula = await db.Matriculas.FirstOrDefaultAsync(m => m.Id == id, ct)
            ?? throw new NotFoundException(nameof(Matricula), id);

        matricula.Cancelar();
        await db.SaveChangesAsync(ct);
    }
}
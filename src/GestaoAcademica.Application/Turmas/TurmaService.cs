using GestaoAcademica.Application.Abstractions;
using GestaoAcademica.Application.Common;
using GestaoAcademica.Domain.Entities;
using GestaoAcademica.Domain.Enums;

using Microsoft.EntityFrameworkCore;

namespace GestaoAcademica.Application.Turmas;

public sealed class TurmaService(IAppDbContext db) : ITurmaService
{
    public async Task<PagedResult<TurmaResponse>> ListarAsync(int page, int pageSize, CancellationToken ct)
    {
        var query = db.Turmas.AsNoTracking().OrderByDescending(t => t.Semestre).ThenBy(t => t.Codigo);

        var total = await query.CountAsync(ct);

        // Projeção: o COUNT das matrículas ativas é calculado pelo SQL Server,
        // sem trazer as matrículas para a memória.
        var items = await query
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .Select(t => new TurmaResponse(
                t.Id, t.Codigo, t.Disciplina, t.Semestre, t.Vagas,
                t.Vagas - t.Matriculas.Count(m => m.Status == StatusMatricula.Ativa)))
            .ToListAsync(ct);

        return new PagedResult<TurmaResponse>(items, page, pageSize, total);
    }

    public async Task<TurmaResponse> ObterAsync(Guid id, CancellationToken ct)
    {
        // Mesma projeção, para um único registro
        var turma = await db.Turmas.AsNoTracking()
            .Where(t => t.Id == id)
            .Select(t => new TurmaResponse(
                t.Id, t.Codigo, t.Disciplina, t.Semestre, t.Vagas,
                t.Vagas - t.Matriculas.Count(m => m.Status == StatusMatricula.Ativa)))
            .FirstOrDefaultAsync(ct)
            ?? throw new NotFoundException(nameof(Turma), id);

        return turma;
    }

    public async Task<TurmaResponse> CriarAsync(CriarTurmaRequest request, CancellationToken ct)
    {
        var codigo = request.Codigo.Trim().ToUpperInvariant();
        var semestre = request.Semestre.Trim();

        // Código + semestre é único (índice composto no banco)
        if (await db.Turmas.AnyAsync(t => t.Codigo == codigo && t.Semestre == semestre, ct))
            throw new ConflictException($"Já existe a turma '{codigo}' no semestre {semestre}.");

        var turma = new Turma(request.Codigo, request.Disciplina, request.Semestre, request.Vagas);

        db.Turmas.Add(turma);
        await db.SaveChangesAsync(ct);

        // Turma nova não tem matrícula: VagasDisponiveis == Vagas
        return new TurmaResponse(turma.Id, turma.Codigo, turma.Disciplina, turma.Semestre, turma.Vagas, turma.Vagas);
    }

    public async Task<TurmaResponse> AlterarVagasAsync(Guid id, AlterarVagasRequest request, CancellationToken ct)
    {
        // Include: a regra "vagas não podem ser menores que as matrículas ativas"
        // precisa das matrículas carregadas para contar.
        var turma = await db.Turmas
            .Include(t => t.Matriculas)
            .FirstOrDefaultAsync(t => t.Id == id, ct)
            ?? throw new NotFoundException(nameof(Turma), id);

        turma.AlterarVagas(request.Vagas);
        await db.SaveChangesAsync(ct);

        return new TurmaResponse(turma.Id, turma.Codigo, turma.Disciplina, turma.Semestre, turma.Vagas, turma.VagasDisponiveis);
    }
}
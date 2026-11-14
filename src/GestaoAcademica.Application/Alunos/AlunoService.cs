using GestaoAcademica.Application.Abstractions;
using GestaoAcademica.Application.Common;
using GestaoAcademica.Domain.Entities;

using Microsoft.EntityFrameworkCore;

namespace GestaoAcademica.Application.Alunos;

public sealed class AlunoService(IAppDbContext db) : IAlunoService
{
    public async Task<PagedResult<AlunoResponse>> ListarAsync(int page, int pageSize, CancellationToken ct)
    {
        var query = db.Alunos.AsNoTracking().OrderBy(a => a.Nome);

        var total = await query.CountAsync(ct);

        var items = await query
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .Select(a => ToResponse(a))
            .ToListAsync(ct);

        return new PagedResult<AlunoResponse>(items, page, pageSize, total);
    }

    public async Task<AlunoResponse> ObterAsync(Guid id, CancellationToken ct)
    {
        var aluno = await db.Alunos.AsNoTracking().FirstOrDefaultAsync(a => a.Id == id, ct)
            ?? throw new NotFoundException(nameof(Aluno), id);

        return ToResponse(aluno);
    }

    public async Task<AlunoResponse> CriarAsync(CriarAlunoRequest request, CancellationToken ct)
    {
        var email = request.Email.Trim().ToLowerInvariant();
        var ra = request.RA.Trim();

        if (await db.Alunos.AnyAsync(a => a.Email == email, ct))
            throw new ConflictException($"Já existe um aluno com o e-mail '{email}'.");

        if (await db.Alunos.AnyAsync(a => a.RA == ra, ct))
            throw new ConflictException($"Já existe um aluno com o RA '{ra}'.");

        var aluno = new Aluno(request.Nome, request.Email, request.RA, request.DataNascimento);

        db.Alunos.Add(aluno);
        await db.SaveChangesAsync(ct);

        return ToResponse(aluno);
    }

    public async Task<AlunoResponse> AtualizarAsync(Guid id, AtualizarAlunoRequest request, CancellationToken ct)
    {
        var aluno = await ObterEntidadeAsync(id, ct);

        var email = request.Email.Trim().ToLowerInvariant();

        if (await db.Alunos.AnyAsync(a => a.Email == email && a.Id != id, ct))
            throw new ConflictException($"Já existe um aluno com o e-mail '{email}'.");

        aluno.AtualizarDados(request.Nome, request.Email);
        await db.SaveChangesAsync(ct);

        return ToResponse(aluno);
    }

    public async Task DesativarAsync(Guid id, CancellationToken ct)
    {
        var aluno = await ObterEntidadeAsync(id, ct);
        aluno.Desativar();
        await db.SaveChangesAsync(ct);
    }

    public async Task AtivarAsync(Guid id, CancellationToken ct)
    {
        var aluno = await ObterEntidadeAsync(id, ct);
        aluno.Ativar();
        await db.SaveChangesAsync(ct);
    }

    // Com rastreamento: a entidade será modificada e salva
    private async Task<Aluno> ObterEntidadeAsync(Guid id, CancellationToken ct) =>
        await db.Alunos.FirstOrDefaultAsync(a => a.Id == id, ct)
            ?? throw new NotFoundException(nameof(Aluno), id);

    private static AlunoResponse ToResponse(Aluno a) =>
        new(a.Id, a.Nome, a.Email, a.RA, a.DataNascimento, a.Ativo);
}
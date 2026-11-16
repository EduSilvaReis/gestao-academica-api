using GestaoAcademica.Application.Abstractions;
using GestaoAcademica.Application.Common;
using GestaoAcademica.Domain.Entities;

using Microsoft.EntityFrameworkCore;

namespace GestaoAcademica.Application.Alunos;

// Recebe o contexto do banco (IAppDbContext) pela injeção de dependência.
public sealed class AlunoService(IAppDbContext db) : IAlunoService
{
    public async Task<PagedResult<AlunoResponse>> ListarAsync(int page, int pageSize, CancellationToken ct)
    {
        // Monta a consulta ordenada por nome. Ainda não executa nada no banco.
        var query = db.Alunos.AsNoTracking().OrderBy(a => a.Nome);

        // 1ª ida ao banco: SELECT COUNT(*) para saber o total de alunos.
        var total = await query.CountAsync(ct);

        // 2ª ida ao banco: pega só a página pedida, já convertida em DTO.
        var items = await query
            .Skip((page - 1) * pageSize)   // pula as páginas anteriores
            .Take(pageSize)                // pega só esta página
            .Select(a => ToResponse(a))    // entidade → DTO
            .ToListAsync(ct);              // executa a consulta

        // Devolve a página dentro do envelope com os totais.
        return new PagedResult<AlunoResponse>(items, page, pageSize, total);
    }

    public async Task<AlunoResponse> ObterAsync(Guid id, CancellationToken ct)
    {
        // Busca o aluno pelo id. Se não achar, lança NotFoundException (vira 404).
        var aluno = await db.Alunos.AsNoTracking().FirstOrDefaultAsync(a => a.Id == id, ct)
            ?? throw new NotFoundException(nameof(Aluno), id);

        // Converte em DTO e devolve.
        return ToResponse(aluno);
    }

    public async Task<AlunoResponse> CriarAsync(CriarAlunoRequest request, CancellationToken ct)
    {
        // Normaliza igual ao domínio, para a comparação de duplicidade bater.
        var email = request.Email.Trim().ToLowerInvariant();
        var ra = request.RA.Trim();

        // Verifica no banco se o e-mail já existe. Se sim, lança ConflictException (vira 409).
        if (await db.Alunos.AnyAsync(a => a.Email == email, ct))
            throw new ConflictException($"Já existe um aluno com o e-mail '{email}'.");

        // Mesma verificação para o RA.
        if (await db.Alunos.AnyAsync(a => a.RA == ra, ct))
            throw new ConflictException($"Já existe um aluno com o RA '{ra}'.");

        // Cria a entidade. O construtor valida; se algo estiver errado, lança DomainException (vira 422).
        var aluno = new Aluno(request.Nome, request.Email, request.RA, request.DataNascimento);

        // Marca a entidade para inserção.
        db.Alunos.Add(aluno);

        // Executa o INSERT no banco.
        await db.SaveChangesAsync(ct);

        // Devolve o DTO, já com o Id.
        return ToResponse(aluno);
    }

    public async Task<AlunoResponse> AtualizarAsync(Guid id, AtualizarAlunoRequest request, CancellationToken ct)
    {
        // Busca o aluno COM rastreamento, porque vai ser alterado.
        var aluno = await ObterEntidadeAsync(id, ct);

        var email = request.Email.Trim().ToLowerInvariant();

        // Verifica se OUTRO aluno (a.Id != id) já usa este e-mail.
        if (await db.Alunos.AnyAsync(a => a.Email == email && a.Id != id, ct))
            throw new ConflictException($"Já existe um aluno com o e-mail '{email}'.");

        // A entidade valida e altera os próprios dados.
        aluno.AtualizarDados(request.Nome, request.Email);

        // O EF percebe o que mudou e executa o UPDATE.
        await db.SaveChangesAsync(ct);

        return ToResponse(aluno);
    }

    public async Task DesativarAsync(Guid id, CancellationToken ct)
    {
        var aluno = await ObterEntidadeAsync(id, ct);   // busca com rastreamento
        aluno.Desativar();                               // muda Ativo para false
        await db.SaveChangesAsync(ct);                   // UPDATE no banco
    }

    public async Task AtivarAsync(Guid id, CancellationToken ct)
    {
        var aluno = await ObterEntidadeAsync(id, ct);
        aluno.Ativar();
        await db.SaveChangesAsync(ct);
    }

    // Busca COM rastreamento (sem AsNoTracking): usado quando a entidade vai ser alterada.
    private async Task<Aluno> ObterEntidadeAsync(Guid id, CancellationToken ct) =>
        await db.Alunos.FirstOrDefaultAsync(a => a.Id == id, ct)
            ?? throw new NotFoundException(nameof(Aluno), id);

    // Converte entidade em DTO. É o único lugar onde essa conversão existe.
    private static AlunoResponse ToResponse(Aluno a) =>
        new(a.Id, a.Nome, a.Email, a.RA, a.DataNascimento, a.Ativo);
}
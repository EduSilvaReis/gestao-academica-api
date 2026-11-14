using GestaoAcademica.Application.Common;

namespace GestaoAcademica.Application.Alunos;

public interface IAlunoService
{
    Task<PagedResult<AlunoResponse>> ListarAsync(int page, int pageSize, CancellationToken ct);
    Task<AlunoResponse> ObterAsync(Guid id, CancellationToken ct);
    Task<AlunoResponse> CriarAsync(CriarAlunoRequest request, CancellationToken ct);
    Task<AlunoResponse> AtualizarAsync(Guid id, AtualizarAlunoRequest request, CancellationToken ct);
    Task DesativarAsync(Guid id, CancellationToken ct);
    Task AtivarAsync(Guid id, CancellationToken ct);
}
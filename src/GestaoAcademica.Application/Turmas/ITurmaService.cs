using GestaoAcademica.Application.Common;

namespace GestaoAcademica.Application.Turmas;

public interface ITurmaService
{
    Task<PagedResult<TurmaResponse>> ListarAsync(int page, int pageSize, CancellationToken ct);
    Task<TurmaResponse> ObterAsync(Guid id, CancellationToken ct);
    Task<TurmaResponse> CriarAsync(CriarTurmaRequest request, CancellationToken ct);
    Task<TurmaResponse> AlterarVagasAsync(Guid id, AlterarVagasRequest request, CancellationToken ct);
}
using GestaoAcademica.Application.Common;

namespace GestaoAcademica.Application.Matriculas;

public interface IMatriculaService
{
    Task<PagedResult<MatriculaResponse>> ListarPorTurmaAsync(Guid turmaId, int page, int pageSize, CancellationToken ct);
    Task<MatriculaResponse> ObterAsync(Guid id, CancellationToken ct);
    Task<MatriculaResponse> MatricularAsync(MatricularRequest request, CancellationToken ct);
    Task CancelarAsync(Guid id, CancellationToken ct);
}
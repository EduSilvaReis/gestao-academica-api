using GestaoAcademica.Domain.Enums;

namespace GestaoAcademica.Application.Notas;

public interface INotaService
{
    Task<BoletimResponse> ObterBoletimAsync(Guid matriculaId, CancellationToken ct);
    Task<NotaResponse> LancarAsync(Guid matriculaId, LancarNotaRequest request, CancellationToken ct);
    Task<NotaResponse> CorrigirAsync(Guid matriculaId, TipoAvaliacao avaliacao, CorrigirNotaRequest request, CancellationToken ct);
}
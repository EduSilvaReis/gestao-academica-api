namespace GestaoAcademica.Application.Relatorios;

public interface IRelatorioService
{
    Task<DesempenhoTurmaResponse> DesempenhoTurmaAsync(Guid turmaId, CancellationToken ct);
}
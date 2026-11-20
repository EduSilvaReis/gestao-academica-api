using GestaoAcademica.Application.Relatorios;

using Microsoft.AspNetCore.Mvc;

namespace GestaoAcademica.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Produces("application/json")]
public sealed class RelatoriosController(IRelatorioService service) : ControllerBase
{
    // GET /api/relatorios/turmas/{turmaId}/desempenho
    [HttpGet("turmas/{turmaId:guid}/desempenho")]
    [ProducesResponseType<DesempenhoTurmaResponse>(StatusCodes.Status200OK)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<DesempenhoTurmaResponse>> DesempenhoTurma(Guid turmaId, CancellationToken ct) =>
        Ok(await service.DesempenhoTurmaAsync(turmaId, ct));
}
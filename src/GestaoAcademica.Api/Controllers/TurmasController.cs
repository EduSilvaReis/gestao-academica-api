using GestaoAcademica.Application.Common;
using GestaoAcademica.Application.Turmas;

using Microsoft.AspNetCore.Mvc;

namespace GestaoAcademica.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Produces("application/json")]
public sealed class TurmasController(ITurmaService service) : ControllerBase
{
    [HttpGet]
    [ProducesResponseType<PagedResult<TurmaResponse>>(StatusCodes.Status200OK)]
    public async Task<ActionResult<PagedResult<TurmaResponse>>> Listar(
        [FromQuery] int page = 1, [FromQuery] int pageSize = 10, CancellationToken ct = default)
    {
        page = Math.Max(page, 1);
        pageSize = Math.Clamp(pageSize, 1, 100);
        return Ok(await service.ListarAsync(page, pageSize, ct));
    }

    [HttpGet("{id:guid}")]
    [ProducesResponseType<TurmaResponse>(StatusCodes.Status200OK)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<TurmaResponse>> Obter(Guid id, CancellationToken ct) =>
        Ok(await service.ObterAsync(id, ct));

    [HttpPost]
    [ProducesResponseType<TurmaResponse>(StatusCodes.Status201Created)]
    [ProducesResponseType<ValidationProblemDetails>(StatusCodes.Status400BadRequest)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status409Conflict)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status422UnprocessableEntity)]
    public async Task<ActionResult<TurmaResponse>> Criar(CriarTurmaRequest request, CancellationToken ct)
    {
        var turma = await service.CriarAsync(request, ct);
        return CreatedAtAction(nameof(Obter), new { id = turma.Id }, turma);
    }

    // PATCH: alteração parcial (só as vagas), diferente do PUT que substitui o recurso inteiro
    [HttpPatch("{id:guid}/vagas")]
    [ProducesResponseType<TurmaResponse>(StatusCodes.Status200OK)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status404NotFound)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status422UnprocessableEntity)]
    public async Task<ActionResult<TurmaResponse>> AlterarVagas(Guid id, AlterarVagasRequest request, CancellationToken ct) =>
        Ok(await service.AlterarVagasAsync(id, request, ct));
}
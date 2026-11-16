using GestaoAcademica.Application.Common;
using GestaoAcademica.Application.Matriculas;

using Microsoft.AspNetCore.Mvc;

namespace GestaoAcademica.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Produces("application/json")]
public sealed class MatriculasController(IMatriculaService service) : ControllerBase
{
    // GET /api/matriculas?turmaId=...  → lista as matrículas de uma turma
    [HttpGet]
    [ProducesResponseType<PagedResult<MatriculaResponse>>(StatusCodes.Status200OK)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<PagedResult<MatriculaResponse>>> ListarPorTurma(
        [FromQuery] Guid turmaId, [FromQuery] int page = 1, [FromQuery] int pageSize = 10, CancellationToken ct = default)
    {
        page = Math.Max(page, 1);
        pageSize = Math.Clamp(pageSize, 1, 100);
        return Ok(await service.ListarPorTurmaAsync(turmaId, page, pageSize, ct));
    }

    [HttpGet("{id:guid}")]
    [ProducesResponseType<MatriculaResponse>(StatusCodes.Status200OK)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<MatriculaResponse>> Obter(Guid id, CancellationToken ct) =>
        Ok(await service.ObterAsync(id, ct));

    [HttpPost]
    [ProducesResponseType<MatriculaResponse>(StatusCodes.Status201Created)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status404NotFound)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status409Conflict)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status422UnprocessableEntity)]
    public async Task<ActionResult<MatriculaResponse>> Matricular(MatricularRequest request, CancellationToken ct)
    {
        var matricula = await service.MatricularAsync(request, ct);
        return CreatedAtAction(nameof(Obter), new { id = matricula.Id }, matricula);
    }

    // DELETE cancela: a matrícula continua no banco com status Cancelada
    [HttpDelete("{id:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status404NotFound)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status422UnprocessableEntity)]
    public async Task<IActionResult> Cancelar(Guid id, CancellationToken ct)
    {
        await service.CancelarAsync(id, ct);
        return NoContent();
    }
}
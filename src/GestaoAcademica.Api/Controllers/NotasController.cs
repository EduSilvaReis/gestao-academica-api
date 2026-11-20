using GestaoAcademica.Application.Notas;
using GestaoAcademica.Domain.Enums;

using Microsoft.AspNetCore.Mvc;

namespace GestaoAcademica.Api.Controllers;

// Rota aninhada: a nota pertence a uma matrícula.
// {matriculaId:guid} na rota da classe vale para todos os métodos.
[ApiController]
[Route("api/matriculas/{matriculaId:guid}/notas")]
[Produces("application/json")]
public sealed class NotasController(INotaService service) : ControllerBase
{
    // GET /api/matriculas/{matriculaId}/notas → todas as notas e a média
    [HttpGet]
    [ProducesResponseType<BoletimResponse>(StatusCodes.Status200OK)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<BoletimResponse>> ObterBoletim(Guid matriculaId, CancellationToken ct) =>
        Ok(await service.ObterBoletimAsync(matriculaId, ct));

    // POST /api/matriculas/{matriculaId}/notas → lança uma nota
    [HttpPost]
    [ProducesResponseType<NotaResponse>(StatusCodes.Status201Created)]
    [ProducesResponseType<ValidationProblemDetails>(StatusCodes.Status400BadRequest)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status404NotFound)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status422UnprocessableEntity)]
    public async Task<ActionResult<NotaResponse>> Lancar(Guid matriculaId, LancarNotaRequest request, CancellationToken ct)
    {
        var nota = await service.LancarAsync(matriculaId, request, ct);

        // Location aponta para o boletim, que é onde a nota pode ser consultada
        return CreatedAtAction(nameof(ObterBoletim), new { matriculaId }, nota);
    }

    // PUT /api/matriculas/{matriculaId}/notas/{avaliacao} → corrige o valor
    // {avaliacao} aceita o nome (P1) ou o número (1) do enum
    [HttpPut("{avaliacao}")]
    [ProducesResponseType<NotaResponse>(StatusCodes.Status200OK)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status404NotFound)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status422UnprocessableEntity)]
    public async Task<ActionResult<NotaResponse>> Corrigir(Guid matriculaId, TipoAvaliacao avaliacao, CorrigirNotaRequest request, CancellationToken ct) =>
        Ok(await service.CorrigirAsync(matriculaId, avaliacao, request, ct));
}
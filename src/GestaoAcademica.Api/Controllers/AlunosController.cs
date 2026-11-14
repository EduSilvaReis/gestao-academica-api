using GestaoAcademica.Application.Alunos;
using GestaoAcademica.Application.Common;

using Microsoft.AspNetCore.Mvc;

namespace GestaoAcademica.Api.Controllers;

// [ApiController]: ativa validação automática dos DTOs (400) e outras convenções de API.
// [Route("api/[controller]")]: "[controller]" vira o nome da classe sem "Controller" → /api/alunos
[ApiController]
[Route("api/[controller]")]
[Produces("application/json")]
public sealed class AlunosController(IAlunoService service) : ControllerBase
{
    // GET /api/alunos?page=1&pageSize=10
    // [FromQuery]: os valores vêm da query string da URL
    [HttpGet]
    [ProducesResponseType<PagedResult<AlunoResponse>>(StatusCodes.Status200OK)]
    public async Task<ActionResult<PagedResult<AlunoResponse>>> Listar(
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 10,
        CancellationToken ct = default)
    {
        // Limites defensivos: evita página 0 e pedidos de 1 milhão de linhas
        page = Math.Max(page, 1);
        pageSize = Math.Clamp(pageSize, 1, 100);

        return Ok(await service.ListarAsync(page, pageSize, ct));
    }

    // GET /api/alunos/{id}
    // {id:guid}: só aceita Guid válido; qualquer outra coisa nem chega aqui (404 de rota)
    [HttpGet("{id:guid}")]
    [ProducesResponseType<AlunoResponse>(StatusCodes.Status200OK)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<AlunoResponse>> Obter(Guid id, CancellationToken ct) =>
        Ok(await service.ObterAsync(id, ct));

    // POST /api/alunos
    // O corpo JSON é desserializado no CriarAlunoRequest e validado ([Required] etc.) antes de entrar aqui
    [HttpPost]
    [ProducesResponseType<AlunoResponse>(StatusCodes.Status201Created)]
    [ProducesResponseType<ValidationProblemDetails>(StatusCodes.Status400BadRequest)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status409Conflict)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status422UnprocessableEntity)]
    public async Task<ActionResult<AlunoResponse>> Criar(CriarAlunoRequest request, CancellationToken ct)
    {
        var aluno = await service.CriarAsync(request, ct);

        // 201 Created + cabeçalho Location apontando para GET /api/alunos/{id}
        return CreatedAtAction(nameof(Obter), new { id = aluno.Id }, aluno);
    }

    // PUT /api/alunos/{id}
    [HttpPut("{id:guid}")]
    [ProducesResponseType<AlunoResponse>(StatusCodes.Status200OK)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status404NotFound)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status409Conflict)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status422UnprocessableEntity)]
    public async Task<ActionResult<AlunoResponse>> Atualizar(Guid id, AtualizarAlunoRequest request, CancellationToken ct) =>
        Ok(await service.AtualizarAsync(id, request, ct));

    // DELETE /api/alunos/{id}  → soft delete: desativa, não apaga
    [HttpDelete("{id:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Desativar(Guid id, CancellationToken ct)
    {
        await service.DesativarAsync(id, ct);
        return NoContent();   // 204: sucesso, sem corpo
    }

    // POST /api/alunos/{id}/ativar  → ação que não é CRUD puro, por isso tem um nome na rota
    [HttpPost("{id:guid}/ativar")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Ativar(Guid id, CancellationToken ct)
    {
        await service.AtivarAsync(id, ct);
        return NoContent();
    }
}
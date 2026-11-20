using GestaoAcademica.Application.Abstractions;
using GestaoAcademica.Application.Common;
using GestaoAcademica.Domain.Entities;
using GestaoAcademica.Domain.Enums;

using Microsoft.EntityFrameworkCore;

namespace GestaoAcademica.Application.Notas;

public sealed class NotaService(IAppDbContext db) : INotaService
{
    public async Task<BoletimResponse> ObterBoletimAsync(Guid matriculaId, CancellationToken ct)
    {
        // Carrega a matrícula com as notas, sem rastreamento: só leitura.
        var matricula = await db.Matriculas
            .AsNoTracking()
            .Include(m => m.Notas)
            .FirstOrDefaultAsync(m => m.Id == matriculaId, ct)
            ?? throw new NotFoundException(nameof(Matricula), matriculaId);

        // Converte cada nota em DTO, ordenando por tipo de avaliação (P1, P2, Trabalho).
        var notas = matricula.Notas
            .OrderBy(n => n.Avaliacao)
            .Select(ToResponse)
            .ToList();

        // A média é calculada pela entidade: o mesmo código que tem teste unitário.
        return new BoletimResponse(matricula.Id, notas, matricula.CalcularMedia());
    }

    public async Task<NotaResponse> LancarAsync(Guid matriculaId, LancarNotaRequest request, CancellationToken ct)
    {
        // Com rastreamento e com as notas: a entidade precisa delas para barrar avaliação repetida.
        var matricula = await ObterComNotasAsync(matriculaId, ct);

        // A entidade valida: matrícula ativa, avaliação sem nota, valor e peso dentro do limite.
        var nota = matricula.LancarNota(request.Avaliacao, request.Valor, request.Peso);

        // A nota entrou na coleção da matrícula rastreada. O EF gera o INSERT.
        await db.SaveChangesAsync(ct);

        return ToResponse(nota);
    }

    public async Task<NotaResponse> CorrigirAsync(Guid matriculaId, TipoAvaliacao avaliacao, CorrigirNotaRequest request, CancellationToken ct)
    {
        var matricula = await ObterComNotasAsync(matriculaId, ct);

        // A entidade localiza a nota pela avaliação e altera. Se não existir, lança DomainException.
        matricula.CorrigirNota(avaliacao, request.Valor);

        // O EF percebe a nota alterada e gera o UPDATE.
        await db.SaveChangesAsync(ct);

        // Busca a nota corrigida na coleção para devolver.
        var nota = matricula.Notas.Single(n => n.Avaliacao == avaliacao);
        return ToResponse(nota);
    }

    // Busca com rastreamento e com as notas carregadas: usada quando vai alterar.
    private async Task<Matricula> ObterComNotasAsync(Guid matriculaId, CancellationToken ct) =>
        await db.Matriculas
            .Include(m => m.Notas)
            .FirstOrDefaultAsync(m => m.Id == matriculaId, ct)
            ?? throw new NotFoundException(nameof(Matricula), matriculaId);

    // Converte entidade em DTO.
    private static NotaResponse ToResponse(Nota n) =>
        new(n.Id, n.Avaliacao, n.Valor, n.Peso);
}
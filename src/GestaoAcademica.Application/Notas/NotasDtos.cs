using System.ComponentModel.DataAnnotations;

using GestaoAcademica.Domain.Entities;
using GestaoAcademica.Domain.Enums;

namespace GestaoAcademica.Application.Notas;

// O que o cliente envia para lançar uma nota
public sealed record LancarNotaRequest(
    [Required] TipoAvaliacao Avaliacao,
    [Range(0, 10)] decimal Valor,
    [Range(0.01, 10)] decimal Peso);

// Para corrigir, só o valor muda. A avaliação vem na URL.
public sealed record CorrigirNotaRequest([Range(0, 10)] decimal Valor);

// Uma nota na resposta
public sealed record NotaResponse(
    Guid Id,
    TipoAvaliacao Avaliacao,
    decimal Valor,
    decimal Peso);

// O boletim da matrícula: todas as notas e a média ponderada
public sealed record BoletimResponse(
    Guid MatriculaId,
    IReadOnlyList<NotaResponse> Notas,
    decimal? Media);   // null quando ainda não há notas
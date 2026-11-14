using GestaoAcademica.Application.Common;
using GestaoAcademica.Domain.Exceptions;

using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace GestaoAcademica.Api.ErrorHandling;

// IExceptionHandler: interface do ASP.NET para interceptar qualquer exceção não tratada.
// O ASP.NET chama TryHandleAsync sempre que uma requisição termina com exceção.
public sealed class GlobalExceptionHandler(ILogger<GlobalExceptionHandler> logger) : IExceptionHandler
{
    public async ValueTask<bool> TryHandleAsync(
        HttpContext httpContext,
        Exception exception,
        CancellationToken cancellationToken)
    {
        // Decide o status HTTP e o título conforme o TIPO da exceção.
        // "switch expression": cada linha é "padrão => resultado".
        var (status, title) = exception switch
        {
            DomainException => (StatusCodes.Status422UnprocessableEntity, "Regra de negócio violada"),
            NotFoundException => (StatusCodes.Status404NotFound, "Recurso não encontrado"),
            ConflictException => (StatusCodes.Status409Conflict, "Conflito"),
            // Índice único violado no banco (duas requisições passaram juntas pela verificação do serviço)
            DbUpdateException => (StatusCodes.Status409Conflict, "Conflito ao salvar"),
            _ => (StatusCodes.Status500InternalServerError, "Erro interno")
        };

        // Erro inesperado: registra o stack trace completo no log.
        // Erros de negócio não precisam de log de erro: são comportamento esperado.
        if (status == StatusCodes.Status500InternalServerError)
            logger.LogError(exception, "Erro não tratado em {Metodo} {Rota}", httpContext.Request.Method, httpContext.Request.Path);

        // ProblemDetails: o formato padrão (RFC 7807) de erro em APIs.
        var problem = new ProblemDetails
        {
            Status = status,
            Title = title,
            // Para 500, não expõe a mensagem interna ao cliente: pode vazar detalhes do sistema.
            Detail = status == StatusCodes.Status500InternalServerError
                ? "Ocorreu um erro inesperado. Tente novamente mais tarde."
                : exception.Message,
            Instance = httpContext.Request.Path
        };

        httpContext.Response.StatusCode = status;
        await httpContext.Response.WriteAsJsonAsync(problem, cancellationToken);

        // true: "eu tratei, não precisa fazer mais nada".
        return true;
    }
}
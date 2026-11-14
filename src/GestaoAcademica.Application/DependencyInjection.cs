using GestaoAcademica.Application.Alunos;

using Microsoft.Extensions.DependencyInjection;

namespace GestaoAcademica.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        services.AddScoped<IAlunoService, AlunoService>();
        return services;
    }
}
using GestaoAcademica.Application.Alunos;
using GestaoAcademica.Application.Matriculas;
using GestaoAcademica.Application.Turmas;

using Microsoft.Extensions.DependencyInjection;

namespace GestaoAcademica.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        services.AddScoped<IAlunoService, AlunoService>();
        services.AddScoped<ITurmaService, TurmaService>();
        services.AddScoped<IMatriculaService, MatriculaService>();
        return services;
    }
}
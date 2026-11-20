using GestaoAcademica.Application.Abstractions;
using GestaoAcademica.Application.Notas;
using GestaoAcademica.Infrastructure.Data;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace GestaoAcademica.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, string connectionString)
    {
        // O AppDbContext usando SQL Server com a connection string recebida
        services.AddDbContext<AppDbContext>(options => options.UseSqlServer(connectionString));

        // Quem pedir IAppDbContext recebe o mesmo AppDbContext da requisição
        services.AddScoped<IAppDbContext>(provider => provider.GetRequiredService<AppDbContext>());

        return services;
    }
}
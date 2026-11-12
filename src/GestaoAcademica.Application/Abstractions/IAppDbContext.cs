using GestaoAcademica.Domain.Entities;

using Microsoft.EntityFrameworkCore;

namespace GestaoAcademica.Application.Abstractions;

public interface IAppDbContext
{
    DbSet<Aluno> Alunos { get; }
    DbSet<Turma> Turmas { get; }
    DbSet<Matricula> Matriculas { get; }
    DbSet<Nota> Notas { get; }

    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}
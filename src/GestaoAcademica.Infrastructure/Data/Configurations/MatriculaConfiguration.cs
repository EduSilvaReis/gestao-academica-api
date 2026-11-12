using GestaoAcademica.Domain.Entities;
using GestaoAcademica.Domain.Enums;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace GestaoAcademica.Infrastructure.Data.Configurations;

public class MatriculaConfiguration : IEntityTypeConfiguration<Matricula>
{
    public void Configure(EntityTypeBuilder<Matricula> builder)
    {
        builder.ToTable("Matriculas");

        builder.HasKey(m => m.Id);
        builder.Property(m => m.Id).ValueGeneratedNever();

        builder.HasOne<Aluno>()
            .WithMany()
            .HasForeignKey(m => m.AlunoId)
            .OnDelete(DeleteBehavior.Restrict);

        // Apenas UMA matrícula ATIVA por aluno e turma; canceladas não bloqueiam rematrícula
        builder.HasIndex(m => new { m.AlunoId, m.TurmaId })
            .IsUnique()
            .HasFilter($"[Status] = {(int)StatusMatricula.Ativa}");

        builder.HasMany(m => m.Notas)
            .WithOne()
            .HasForeignKey(n => n.MatriculaId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.Navigation(m => m.Notas).UsePropertyAccessMode(PropertyAccessMode.Field);
    }
}
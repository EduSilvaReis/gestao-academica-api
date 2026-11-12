using GestaoAcademica.Domain.Entities;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace GestaoAcademica.Infrastructure.Data.Configurations;

public class TurmaConfiguration : IEntityTypeConfiguration<Turma>
{
    public void Configure(EntityTypeBuilder<Turma> builder)
    {
        builder.ToTable("Turmas");

        builder.HasKey(t => t.Id);
        builder.Property(t => t.Id).ValueGeneratedNever();

        builder.Property(t => t.Codigo).HasMaxLength(Turma.CodigoTamanhoMaximo).IsRequired();
        builder.Property(t => t.Disciplina).HasMaxLength(Turma.DisciplinaTamanhoMaximo).IsRequired();

        // Formato fixo "AAAA.N", por exemplo "2026.2"
        builder.Property(t => t.Semestre).HasMaxLength(6).IsFixedLength().IsRequired();

        builder.HasIndex(t => new { t.Codigo, t.Semestre }).IsUnique();

        // Propriedade calculada: não vira coluna
        builder.Ignore(t => t.VagasDisponiveis);

        builder.HasMany(t => t.Matriculas)
            .WithOne()
            .HasForeignKey(m => m.TurmaId)
            .OnDelete(DeleteBehavior.Restrict);

        // O EF lê e grava a lista privada _matriculas, não a propriedade somente leitura
        builder.Navigation(t => t.Matriculas).UsePropertyAccessMode(PropertyAccessMode.Field);
    }
}
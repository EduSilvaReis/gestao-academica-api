using GestaoAcademica.Domain.Entities;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace GestaoAcademica.Infrastructure.Data.Configurations;

public class AlunoConfiguration : IEntityTypeConfiguration<Aluno>
{
    public void Configure(EntityTypeBuilder<Aluno> builder)
    {
        builder.ToTable("Alunos");

        builder.HasKey(a => a.Id);
        builder.Property(a => a.Id).ValueGeneratedNever();

        builder.Property(a => a.Nome).HasMaxLength(Aluno.NomeTamanhoMaximo).IsRequired();
        builder.Property(a => a.Email).HasMaxLength(Aluno.EmailTamanhoMaximo).IsRequired();
        builder.Property(a => a.RA).HasMaxLength(Aluno.RATamanhoMaximo).IsRequired();

        builder.HasIndex(a => a.Email).IsUnique();
        builder.HasIndex(a => a.RA).IsUnique();
    }
}
using GestaoAcademica.Domain.Entities;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace GestaoAcademica.Infrastructure.Data.Configurations;

public class NotaConfiguration : IEntityTypeConfiguration<Nota>
{
    public void Configure(EntityTypeBuilder<Nota> builder)
    {
        builder.ToTable("Notas", tabela =>
        {
            tabela.HasCheckConstraint("CK_Notas_Valor", "[Valor] >= 0 AND [Valor] <= 10");
            tabela.HasCheckConstraint("CK_Notas_Peso", "[Peso] > 0 AND [Peso] <= 10");
        });

        builder.HasKey(n => n.Id);
        builder.Property(n => n.Id).ValueGeneratedNever();

        // decimal(4,2): até 99,99 com duas casas, suficiente para 0 a 10
        builder.Property(n => n.Valor).HasPrecision(4, 2);
        builder.Property(n => n.Peso).HasPrecision(4, 2);

        builder.HasIndex(n => new { n.MatriculaId, n.Avaliacao }).IsUnique();
    }
}
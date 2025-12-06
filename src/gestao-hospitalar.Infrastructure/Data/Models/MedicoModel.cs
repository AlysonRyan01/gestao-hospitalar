using gestao_hospitalar.Domain.Medicos.Aggregates;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace gestao_hospitalar.Infrastructure.Data.Models;

public class MedicoModel : IEntityTypeConfiguration<Medico>
{
    public void Configure(EntityTypeBuilder<Medico> builder)
    {
        builder.ToTable("Medicos");

        builder.HasKey(m => m.Id);

        builder.Property(m => m.Nome)
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(m => m.Telefone)
            .IsRequired()
            .HasMaxLength(20);

        builder.Property(m => m.Especialidade)
            .IsRequired()
            .HasMaxLength(100);

        builder.HasMany(m => m.Consultas)
            .WithOne(c => c.Medico)
            .HasForeignKey(c => c.MedicoId)
            .OnDelete(DeleteBehavior.SetNull);
    }
}
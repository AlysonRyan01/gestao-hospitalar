using gestao_hospitalar.Domain.Pacientes.Aggregates;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace gestao_hospitalar.Infrastructure.Data.Models;

public class PacienteModel : IEntityTypeConfiguration<Paciente>
{
    public void Configure(EntityTypeBuilder<Paciente> builder)
    {
        builder.ToTable("Pacientes");

        builder.HasKey(p => p.Id);

        builder.Property(p => p.UserId)
            .IsRequired();

        builder.HasOne(p => p.User)
            .WithMany()
            .HasForeignKey(p => p.UserId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasMany(p => p.Consultas)
            .WithOne(c => c.Paciente)
            .HasForeignKey(c => c.PacienteId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
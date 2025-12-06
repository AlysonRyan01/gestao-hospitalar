using gestao_hospitalar.Domain.Consultas.Aggregates;
using gestao_hospitalar.Domain.Medicos.Aggregates;
using gestao_hospitalar.Domain.Pacientes.Aggregates;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace gestao_hospitalar.Infrastructure.Data.Models;

public class ConsultaModel : IEntityTypeConfiguration<Consulta>
{
    public void Configure(EntityTypeBuilder<Consulta> builder)
    {
        builder.ToTable("Consultas");

        builder.HasKey(c => c.Id);

        builder.Property(c => c.Sobre)
            .IsRequired()
            .HasMaxLength(500);

        builder.Property(c => c.MarcadoPara)
            .IsRequired();

        builder.Property(c => c.FinalConsultaPara);

        builder.Property(c => c.MotivoCancelamento)
            .HasMaxLength(500);

        builder.Property(c => c.Status)
            .IsRequired();

        builder.HasOne<Paciente>()
            .WithMany(p => p.Consultas)
            .HasForeignKey(c => c.PacienteId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne<Medico>()
            .WithMany(m => m.Consultas)
            .HasForeignKey(c => c.MedicoId)
            .OnDelete(DeleteBehavior.SetNull);
    }
}
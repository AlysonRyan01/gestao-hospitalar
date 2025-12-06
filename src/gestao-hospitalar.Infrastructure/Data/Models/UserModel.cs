using gestao_hospitalar.Domain.Pacientes.Aggregates;
using gestao_hospitalar.Domain.Users.Aggregates;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace gestao_hospitalar.Infrastructure.Data.Models;

public class UserModel : IEntityTypeConfiguration<User>
{
    public void Configure(EntityTypeBuilder<User> builder)
    {
        builder.ToTable("Users");

        builder.HasKey(u => u.Id);

        builder.Property(u => u.Name)
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(u => u.Email)
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(u => u.Password)
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(u => u.Phone)
            .IsRequired()
            .HasMaxLength(20);

        builder.HasOne<Paciente>()
            .WithOne(p => p.User)
            .HasForeignKey<Paciente>(p => p.UserId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
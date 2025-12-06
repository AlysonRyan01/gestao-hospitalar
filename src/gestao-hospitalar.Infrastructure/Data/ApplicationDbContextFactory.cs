using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace gestao_hospitalar.Infrastructure.Data;

public class ApplicationDbContextFactory: IDesignTimeDbContextFactory<ApplicationDbContext>
{
    public ApplicationDbContext CreateDbContext(string[] args)
    {
        var optionsBuilder = new DbContextOptionsBuilder<ApplicationDbContext>();
        optionsBuilder.UseNpgsql("Server=.;Database=GestaoHospitalar;Trusted_Connection=True;");

        return new ApplicationDbContext(optionsBuilder.Options);
    }
}
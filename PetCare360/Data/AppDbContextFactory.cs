using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace PetCare360.Data;

public class AppDbContextFactory : IDesignTimeDbContextFactory<AppDbContext>
{
    public AppDbContext CreateDbContext(string[] args)
    {
        var optionsBuilder = new DbContextOptionsBuilder<AppDbContext>();
        optionsBuilder.UseOracle("User Id=rm563806;Password=310806;Data Source=oracle.fiap.com.br:1521/ORCL;");

        return new AppDbContext(optionsBuilder.Options);
    }
}
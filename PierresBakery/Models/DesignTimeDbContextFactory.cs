using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;
using System.IO;

namespace PierresBakery.Models
{
  public class PierresBakeryContextFactory : IDesignTimeDbContextFactory<PierresBakeryContext>
  {

    PierresBakeryContext IDesignTimeDbContextFactory<PierresBakeryContext>.CreateDbContext(string[] args)
    {
      IConfigurationRoot configuration = new ConfigurationBuilder()
          .SetBasePath(Directory.GetCurrentDirectory())
          .AddJsonFile("appsettings.json")
          .Build();

      var builder = new DbContextOptionsBuilder<PierresBakeryContext>();

      builder.UseMySql(configuration["ConnectionStrings:DefaultConnection"], ServerVersion.AutoDetect(configuration["ConnectionStrings:DefaultConnection"]));

      return new PierresBakeryContext(builder.Options);
    }
  }
}
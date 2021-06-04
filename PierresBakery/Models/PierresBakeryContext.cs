using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;

namespace PierresBakery.Models
{
  public class PierresBakeryContext : IdentityDbContext<ApplicationUser>
  {
     public DbSet<Flavor> Flavors { get; set; }
     public DbSet<Treat> Treats { get; set; }
     public DbSet<FlavorTreat> FlavorTreats { get; set; }
    public PierresBakeryContext(DbContextOptions options) : base(options) { }
    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
      optionsBuilder.UseLazyLoadingProxies();
    }
  }
}
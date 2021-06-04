using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.EntityFrameworkCore;
using PierresBakery.Models;
using Microsoft.AspNetCore.Identity;

namespace PierresBakery
{
  public class Startup
  {
    public IConfigurationRoot Configuration { get; set; }
    public Startup(IWebHostEnvironment env)
    {
      var builder = new ConfigurationBuilder()
          .SetBasePath(env.ContentRootPath)
          .AddJsonFile("appsettings.json");
      Configuration = builder.Build();
    }


    public void ConfigureServices(IServiceCollection services)
    {
      services.AddMvc();
      services.AddEntityFrameworkMySql()
      .AddDbContext<PierresBakeryContext>(options => options
      .UseMySql(Configuration["ConnectionStrings:DefaultConnection"], ServerVersion.AutoDetect(Configuration["ConnectionStrings:DefaultConnection"])
      )
      );
      services.AddIdentity<ApplicationUser, IdentityRole>()
      .AddEntityFrameworkStores<PierresBakeryContext>()
      .AddDefaultTokenProviders();
      
       services.Configure<IdentityOptions>(options =>
    {
        options.Password.RequireDigit = false;
        options.Password.RequiredLength = 0;
        options.Password.RequireLowercase = false;
        options.Password.RequireNonAlphanumeric = false;
        options.Password.RequireUppercase = false;
        options.Password.RequiredUniqueChars = 0;
    });
    }

    public void Configure(IApplicationBuilder app)
    {
      app.UseDeveloperExceptionPage();

      app.UseAuthentication();

      app.UseRouting();

      app.UseEndpoints(routes =>
      {
        routes.MapControllerRoute("default", "{controller=Home}/{action=Index}/{id?}");
      });

      app.UseStaticFiles();

      app.Run(async (context) =>
      {
        await context.Response.WriteAsync("Hello World!");
      });
    }
  }

}


using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace EPSPlus.API.Presentation.Extensions;

public static class DbRegistration
{
    public static void AddDbServices(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddDbContext<AppDbContext>(options =>
            options.UseSqlServer(configuration.GetConnectionString("DefaultConnection"),
                 sqlOptions => sqlOptions.MigrationsAssembly(typeof(AppDbContext).Assembly.GetName().Name)));
        

        services.AddIdentity<ApplicationUser, IdentityRole>()
            .AddEntityFrameworkStores<AppDbContext>().AddDefaultTokenProviders();
    }
}

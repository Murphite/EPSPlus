using EPSPlus.Domain.Constants;
using EPSPlus.Domain.Entities;
using EPSPlus.Infrastructure.Persistence;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection; 

namespace EPSPlus.Infrastructure.DbInitializer;

public class DbInitializer
{   
    public static async Task Run(IApplicationBuilder app)
    {
        using var scope = app.ApplicationServices.CreateScope();
        var serviceProvider = scope.ServiceProvider;

        var dbContext = serviceProvider.GetRequiredService<AppDbContext>();
        var roleManager = serviceProvider.GetRequiredService<RoleManager<IdentityRole>>();
        var userManager = serviceProvider.GetRequiredService<UserManager<ApplicationUser>>();

        try
        {
            // Apply migrations if needed
            if (dbContext.Database.GetPendingMigrations().Any())
            {
                dbContext.Database.Migrate();
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Migration Error: {ex.Message}");
        }

        // Create roles if they do not exist
        if (!dbContext.Roles.Any())
        {
            var roles = new List<IdentityRole>
            {
                new() { Name = RolesConstant.Member, NormalizedName = RolesConstant.Member.ToUpper() },
                new() { Name = RolesConstant.Employer, NormalizedName = RolesConstant.Employer.ToUpper() },
                new() { Name = RolesConstant.Admin, NormalizedName = RolesConstant.Admin.ToUpper() }
            };

            await dbContext.Roles.AddRangeAsync(roles);
            await dbContext.SaveChangesAsync();
        }

        // Ensure ADMIN role exists
        var AdminRole = await roleManager.FindByNameAsync(RolesConstant.Admin);
        if (AdminRole == null)
        {
            AdminRole = new IdentityRole
            {
                Name = RolesConstant.Admin,
                NormalizedName = RolesConstant.Admin.ToUpper()
            };
            await roleManager.CreateAsync(AdminRole);
        }

        // Check if the Admin user already exists
        var existingAdmin = await userManager.FindByEmailAsync("ogbeidemurphy@gmail.com");

        if (existingAdmin == null)
        {
            var adminUser = new ApplicationUser
            {
                Id = "admin1",
                FullName = "Murphy Admin",
                Email = "ogbeidemurphy@gmail.com",
                UserName = "ogbeidemurphy@gmail.com",
                PhoneNumber = "080123456789",
                UserType = "Admin",
                CreatedAt = DateTime.Now
            };

            var createResult = await userManager.CreateAsync(adminUser, "Admin@123");

            if (createResult.Succeeded)
            {
                // Ensure the user is added before inserting into Admins
                var newAdminUser = await userManager.FindByEmailAsync("ogbeidemurphy@gmail.com");

                if (newAdminUser != null)
                {
                    await userManager.AddToRoleAsync(newAdminUser, RolesConstant.Admin);

                    var admin = new Admin
                    {
                        UserId = newAdminUser.Id,  // Ensure this ID exists before inserting
                        CreatedAt = DateTime.UtcNow
                    };

                    await dbContext.Admins.AddAsync(admin);
                    await dbContext.SaveChangesAsync();
                }
            }
            else
            {
                Console.WriteLine("Admin user creation failed: " +
                    string.Join(", ", createResult.Errors.Select(e => e.Description)));
            }
        }
    }
}

using Infrastructure.Database.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Infrastructure.Database.Identity;

public static class IdentityConfiguration
{
    
    // Creating the Super Admin User, ensuring only one super admin exists
    public static async Task CreateRoles(this IServiceProvider serviceProvider)
    {
        await using AppDbContext context = new();

        var roleManager = serviceProvider.GetRequiredService<RoleManager<IdentityRole<Guid>>>();
        var userManager = serviceProvider.GetRequiredService<UserManager<AppUser>>();
        var configuration = serviceProvider.GetRequiredService<IConfiguration>();
        
        var roleExists = await roleManager.RoleExistsAsync("SuperAdmin");
        if (!roleExists)
        {
            await roleManager.CreateAsync(new IdentityRole<Guid>("SuperAdmin"));
        }
        
        var adminUsername = configuration["SuperAdmin:Username"];
        var adminEmail = configuration["SuperAdminUser:Email"];
        var adminPassword = configuration["SuperAdminUser:Password"];

        if (adminUsername == null || adminEmail == null || adminPassword == null)
        {
            throw new Exception("Invalid Super Admin User configuration");
        }
        
        var adminUser = await userManager.FindByNameAsync(adminUsername);
        if (adminUser == null)
        {
            //To ensure only one Super Admin exists at the time
            var allUsers = userManager.Users.ToList();
            foreach (var user in allUsers)
            {
                var roles = await userManager.GetRolesAsync(user);
                if (roles.Contains("SuperAdmin"))
                {
                    await userManager.DeleteAsync(user);
                }
            }
            
            var newAdmin = new AppUser
            {
                UserName = adminUsername,
                Email = adminEmail,
                EmailConfirmed = true
            };
            
            var createAdminResult = await userManager.CreateAsync(newAdmin, adminPassword);
            if (createAdminResult.Succeeded)
            {
                await userManager.AddToRoleAsync(newAdmin, "Admin");
            }
        }
    }
}
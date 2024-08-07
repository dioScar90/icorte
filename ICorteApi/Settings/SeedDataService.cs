using ICorteApi.Domain.Entities;
using ICorteApi.Domain.Enums;
using Microsoft.AspNetCore.Identity;

namespace ICorteApi.Settings;

public static class SeedDataService
{
    public static async Task Initialize(IServiceProvider serviceProvider, UserManager<User> userManager)
    {
        var roleManager = serviceProvider.GetRequiredService<RoleManager<IdentityRole>>();

        // string[] userRolesAsArray = { "Admin", "BarberShop", "Client" };
        string[] userRolesAsArray = Enum.GetNames(typeof(UserRole));
        IdentityResult roleResult;

        foreach (var roleName in userRolesAsArray)
        {
            var roleExist = await roleManager.RoleExistsAsync(roleName);
            if (!roleExist)
            {
                roleResult = await roleManager.CreateAsync(new IdentityRole(roleName));
            }
        }

        // Create an admin user if not exists
        var adminUser = await userManager.FindByEmailAsync("admin@example.com");
        if (adminUser is null)
        {
            var user = new User
            {
                UserName = "admin@example.com",
                Email = "admin@example.com",
                EmailConfirmed = true,
            };
            var result = await userManager.CreateAsync(user, "Password123!");
            if (result.Succeeded)
            {
                await userManager.AddToRoleAsync(user, "Admin");
            }
        }
    }
}


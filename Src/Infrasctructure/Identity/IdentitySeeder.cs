using Microsoft.AspNetCore.Identity;
using Domain.Entities;
using Microsoft.Extensions.DependencyInjection;

namespace Infrastructure.Identity;

public static class IdentitySeeder
{
    public static async Task SeedAsync(IServiceProvider services)
    {
        var roleManager =
            services.GetRequiredService<RoleManager<IdentityRole>>();

        var userManager =
            services.GetRequiredService < UserManager<ApplicationUser>>();

        string[] roles = ["Admin", "Manager", "User"];

        foreach (var role in roles)
        {
            if (!await roleManager.RoleExistsAsync(role))
            {
                await roleManager.CreateAsync(
                    new IdentityRole(role));
            }
        }

        // Seed users
        await CreateUserAsync(
            userManager,
            "juancamiloavila92@spl.com",
            "Avila123+",
            "Admin");

        await CreateUserAsync(
            userManager,
            "manager@spl.com",
            "Manager123!",
            "User");
    }

    private static async Task CreateUserAsync(
        UserManager<ApplicationUser> userManager,
        string email,
        string password,
        string role)
    {
        var existingUser = await userManager.FindByEmailAsync(email);

        if (existingUser != null)
        {
            // Make sure the user has the required role
            if (!await userManager.IsInRoleAsync(existingUser, role))
            {
                var userRoleResult =
                    await userManager.AddToRoleAsync(existingUser, role);

                if (!userRoleResult.Succeeded)
                {
                    throw new Exception(
                        $"Failed to assign role {role} to {email}: " +
                        string.Join(", ",
                            userRoleResult.Errors.Select(e => e.Description)));
                }
            }

            return;
        }

        var user = new ApplicationUser
        {
            UserName = email,
            Email = email,
            EmailConfirmed = true
        };

        var result = await userManager.CreateAsync(user, password);

        if (!result.Succeeded)
        {
            throw new Exception(
                $"Failed to create user {email}: " +
                string.Join(", ",
                    result.Errors.Select(e => e.Description)));
        }

        var roleResult = await userManager.AddToRoleAsync(user, role);

        if (!roleResult.Succeeded)
        {
            throw new Exception(
                $"Failed to assign role {role} to {email}: " +
                string.Join(", ",
                    roleResult.Errors.Select(e => e.Description)));
        }
    }
}

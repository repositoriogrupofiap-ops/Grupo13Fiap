using Grupo13Fiap.Identity.Constants;
using Microsoft.AspNetCore.Identity;

namespace Grupo13Fiap.Identity.Data
{
    public static class IdentityDataSeeder
    {
        public static async Task SeedAsync(
            RoleManager<IdentityRole> roleManager,
            UserManager<IdentityUser> userManager)
        {
            // Seed roles
            foreach (var role in new[] { Roles.Admin, Roles.User })
            {
                if (!await roleManager.RoleExistsAsync(role))
                    await roleManager.CreateAsync(new IdentityRole(role));
            }

            // Seed admin user
            const string adminEmail    = "admin@grupo13.com";
            const string adminPassword = "Admin@123!";

            var adminUser = await userManager.FindByEmailAsync(adminEmail);

            if (adminUser is null)
            {
                adminUser = new IdentityUser
                {
                    UserName       = adminEmail,
                    Email          = adminEmail,
                    EmailConfirmed = true,
                    LockoutEnabled = false
                };

                var result = await userManager.CreateAsync(adminUser, adminPassword);
                if (!result.Succeeded)
                    return;
            }

            // Garante a role Admin mesmo se o usuário já existia
            if (!await userManager.IsInRoleAsync(adminUser, Roles.Admin))
                await userManager.AddToRoleAsync(adminUser, Roles.Admin);
        }
    }
}
using Grupo13Fiap.Domain.Enum;
using Grupo13Fiap.Identity.Constants;
using Microsoft.AspNetCore.Identity;

namespace Grupo13Fiap.Identity.Data
{
    public static class IdentityDataSeeder
    {
        // Todas as roles derivadas diretamente do enum de domínio
        private static readonly string[] AllRoles =
            Enum.GetNames<UserRoleEnum>();

        public static async Task SeedAsync(
            RoleManager<IdentityRole> roleManager,
            UserManager<IdentityUser> userManager)
        {
            foreach (var role in AllRoles)
            {
                if (!await roleManager.RoleExistsAsync(role))
                    await roleManager.CreateAsync(new IdentityRole(role));
            }

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
                if (!result.Succeeded) return;
            }

            if (!await userManager.IsInRoleAsync(adminUser, Roles.Admin))
                await userManager.AddToRoleAsync(adminUser, Roles.Admin);
        }
    }
}
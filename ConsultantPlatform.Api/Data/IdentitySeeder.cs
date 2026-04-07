using ConsultantPlatform.Api.Models;
using Microsoft.AspNetCore.Identity;

namespace ConsultantPlatform.Api.Data
{
    public static class IdentitySeeder
    {
        public static async Task SeedAsync(IServiceProvider services)
        {
            using var scope = services.CreateScope();

            var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();
            var userManager = scope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();

            // Roles
            string[] roles = ["Admin", "User"];
            foreach (var role in roles)
            {
                if (!await roleManager.RoleExistsAsync(role))
                    await roleManager.CreateAsync(new IdentityRole(role));
            }

            // Admin
            var adminEmail = "dasekan@gmail.com";
            var adminPassword = "Lara123321!";

            var admin = await userManager.FindByEmailAsync(adminEmail);
            if (admin == null)
            {
                admin = new ApplicationUser
                {
                    UserName = adminEmail,
                    Email = adminEmail,
                    EmailConfirmed = true
                };

                var created = await userManager.CreateAsync(admin, adminPassword);
                if (!created.Succeeded)
                    throw new Exception("Failed to create admin: " + string.Join(", ", created.Errors.Select(e => e.Description)));
            }

            if (!await userManager.IsInRoleAsync(admin, "Admin"))
                await userManager.AddToRoleAsync(admin, "Admin");

            // User
            var userEmail = "dasekan1@gmail.com";
            var userPassword = "lara123321!";

            var user = await userManager.FindByEmailAsync(userEmail);
            if (user == null)
            {
                user = new ApplicationUser
                {
                    UserName = userEmail,
                    Email = userEmail,
                    EmailConfirmed = true
                };

                var createdUser = await userManager.CreateAsync(user, userPassword);
                if (!createdUser.Succeeded)
                    throw new Exception("Failed to create user: " + string.Join(", ", createdUser.Errors.Select(e => e.Description)));
            }

            if (!await userManager.IsInRoleAsync(user, "User"))
                await userManager.AddToRoleAsync(user, "User");
        }
    }
}

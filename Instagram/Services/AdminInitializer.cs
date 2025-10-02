using Microsoft.AspNetCore.Identity;
using Instagram.Models;

namespace Instagram.Services
{
    public class AdminInitializer
    {
        public static async Task SeedAdminUser(RoleManager<IdentityRole<int>> roleManager, UserManager<User> userManager)
        {
            var roles = new[] { "admin", "user" };
            foreach (var role in roles)
            {
                if (!await roleManager.RoleExistsAsync(role))
                {
                    await roleManager.CreateAsync(new IdentityRole<int>(role));
                }
            }

            string adminEmail = "admin@admin.com";
            string adminPassword = "Admin123!";

            if (await userManager.FindByEmailAsync(adminEmail) == null)
            {
                var admin = new User
                {
                    UserName = "admin",
                    Email = adminEmail,
                    ProfilePicturePath = "/images/default-avatar.png",
                    Name = "Administrator"
                };

                var result = await userManager.CreateAsync(admin, adminPassword);
                if (result.Succeeded)
                {
                    await userManager.AddToRoleAsync(admin, "admin");
                }
            }
        }
    }
}
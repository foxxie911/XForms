using Microsoft.AspNetCore.Identity;
using XForms.Data;

namespace XForms.Seeder;

public static class AdminSeeder
{
    public static async Task SeedAdminUserAsync(IServiceProvider serviceProvider)
    {
        using var scope = serviceProvider.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
        var userManager = scope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();
        var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();

        await context.Database.EnsureCreatedAsync();

        if (!await roleManager.RoleExistsAsync("Admin"))
        {
            await roleManager.CreateAsync(new IdentityRole("Admin"));
        }

        var adminUser = await userManager.FindByEmailAsync("admin@gmail.com");
        if (adminUser == null)
        {
            adminUser = new ApplicationUser
            {
                DisplayName = "Admin",
                UserName = "admin@gmail.com",
                Email = "admin@gmail.com"
            };

            await userManager.CreateAsync(adminUser, "@Dmin123");
            await userManager.AddToRoleAsync(adminUser, "Admin");

        }
    }
}
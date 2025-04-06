using Microsoft.AspNetCore.Identity;
using XForms.Data;

namespace XForms.Seeder;

public class UserSeeder
{
   public static async Task SeedUserAsync(IServiceProvider serviceProvider)
   {
      using var scope = serviceProvider.CreateScope();
      var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
      var userManager = scope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();

      await context.Database.EnsureCreatedAsync();

      for (var i = 1; i <= 10; i++)
      {
         var user = await userManager.FindByEmailAsync($"user{i}@gmail.com");
         
         if (user != null) continue;
         
         user = new ApplicationUser
         {
            DisplayName = $"User{i}",
            Email = $"user{i}@gmail.com",
            UserName = $"user_{i}"
         };

         await userManager.CreateAsync(user, "U$er123");
      }
   }
}
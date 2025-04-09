using Microsoft.AspNetCore.Identity;
using XForms.Data;

namespace XForms.Seeder;

public class UserSeeder
{
   public static async Task SeedUserAsync(IServiceProvider serviceProvider)
   {
      const int TOTAL_USERS = 0;
      
      using var scope = serviceProvider.CreateScope();
      var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
      var userManager = scope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();

      await context.Database.EnsureCreatedAsync();

      for (var i = 1; i <= TOTAL_USERS; i++)
      {
         var user = await userManager.FindByEmailAsync($"user{i}@gmail.com");
         
         if (user != null) continue;
         
         user = new ApplicationUser
         {
            DisplayName = $"User{i}",
            Email = $"user{i}@gmail.com",
            UserName = $"user{i}@gmail.com"
         };

         await userManager.CreateAsync(user, "U$er123");
      }
   }
}
using Microsoft.EntityFrameworkCore;
using XForms.Data;
using XForms.Services.Interface;

namespace XForms.Services.Implementation;

public class UserService(IDbContextFactory<ApplicationDbContext> contextFactory) : IUserService
{
    public async Task<bool> ToggleDarkMode(bool isDarkMode, string userId)
    {
        await using var context = await contextFactory.CreateDbContextAsync();
        try
        {
            var user = await context.Users
                .Where(u => u.Id == userId)
                .FirstOrDefaultAsync();

            if (user != null)
            {
                user.IsDarkMode = isDarkMode;
                context.Update(user);
                await context.SaveChangesAsync();
                return true;
            }
        }
        catch (Exception e)
        {
            Console.WriteLine($"{e.Message}");
        }

        return false;
    }
}
using Microsoft.EntityFrameworkCore;
using XForms.Data;
using XForms.Services.Interface;
using Exception = System.Exception;

namespace XForms.Services.Implementation;

public class TemplateService(IDbContextFactory<ApplicationDbContext> contextFactory) : ITemplateService
{
    public async Task<int> CreateTemplateAsync(string? userId)
    {
        await using var context = await contextFactory.CreateDbContextAsync();
        try
        {
            var template = await context.Templates.AddAsync(new Template()
            {
                Title = "Untitled",
                CreatorId = userId!,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow,
                Version = Guid.NewGuid()
            });
            await context.SaveChangesAsync();
            return template.Entity.Id;
        }
        catch (Exception e)
        {
            Console.WriteLine($"{e.Message}");
        }

        return -1;
    }

    public async Task UpdateTemplateAsync(Template? template)
    {
        await using var context = await contextFactory.CreateDbContextAsync();
        var dbTemplate = await context.Templates.FirstOrDefaultAsync(t => t.Id == template!.Id);
        if (dbTemplate is null)
        {
            Console.WriteLine("Template not found");
            return;
        }

        if (dbTemplate.Version != template!.Version)
        {
            Console.WriteLine($"Database entry updated for this template");
            return;
        }

        await context.SaveChangesAsync();
    }

    public async Task<Template> GetTemplateIncludingQuestionAsync(int id)
    {
        await using var context = await contextFactory.CreateDbContextAsync();
        try
        {
            var dbTemplate = await context.Templates
                .Where(t => t.Id == id)
                .Include(t => t.Questions)
                .FirstOrDefaultAsync();
            return dbTemplate!;
        }
        catch (Exception e)
        {
            Console.WriteLine($"{e.Message}");
        }

        return null!;
    }

    public async Task<Template> GetTemplateAsync(int id)
    {
        await using var context = await contextFactory.CreateDbContextAsync();
        try
        {
            var template = await context.Templates
                .Where(t => t.Id == id)
                .FirstOrDefaultAsync();
            return template!;
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
        }

        return null!;
    }

    public IEnumerable<Template> GetAllTemplates()
    {
        using var context = contextFactory.CreateDbContext();
        try
        {
            var templates = context.Templates
                .Include(u => u.Creator)
                .AsEnumerable();
            return templates;
        }
        catch (Exception e)
        {
            Console.WriteLine($"{e.Message}");
        }

        return [];
    }

    public async Task<List<Template>> GetTemplatesByUserId(string userId)
    {
        await using var context = await contextFactory.CreateDbContextAsync();

        try
        {
            var result = await context.Templates
                .Where(u => u.CreatorId == userId)
                .ToListAsync();
            return result;
        }
        catch (Exception e)
        {
            Console.WriteLine($"{e.Message}");
        }

        return [];
    }

    public async Task DeleteTemplate(Template? template)
    {
        if (template is null)
        {
            Console.WriteLine("Template not found");
            return;
        }

        await using var context = await contextFactory.CreateDbContextAsync();

        try
        {
            context.Templates.Remove(template);
            await context.SaveChangesAsync();
        }
        catch (Exception e)
        {
            Console.WriteLine($"{e.Message}");
        }
    }

    public async Task<bool> DeleteTemplatesAsync(IEnumerable<Template> templates)
    {
        await using var context = await contextFactory.CreateDbContextAsync();
        try
        {
            context.Templates.RemoveRange(templates);
            await context.SaveChangesAsync();
            return true;
        }
        catch (Exception e)
        {
            Console.WriteLine($"{e.Message}");
        }

        return false;
    }

    public async Task<bool> MakePublicAsync(Template? template)
    {
        await using var context = await contextFactory.CreateDbContextAsync();
        if (template is null) return false;
        try
        {
            template.IsPublic = true;
            await context.SaveChangesAsync();
            return true;
        }
        catch (Exception e)
        {
            Console.WriteLine($"{e.Message}");
        }

        return false;
    }

    public async Task<List<Template>> GetTopTemplates(int count)
    {
        await using var context = await contextFactory.CreateDbContextAsync();
        try
        {
            var result = await context.Templates
                .Where(t => t.IsPublic)
                .Include(t => t.Likes)
                .OrderByDescending(t => t.Likes.Count)
                .Take(count)
                .ToListAsync();
            return result;
        }
        catch (Exception e)
        {
            Console.WriteLine($"{e.Message}");
        }

        return [];
    }
}
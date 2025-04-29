using Microsoft.EntityFrameworkCore;
using XForms.Data;

namespace XForms.Services;

public class TemplateService(ApplicationDbContext context)
{
    public async Task<int> CreateTemplate(string? userId)
    {
        if (userId is null)
            Console.WriteLine("User not authorized, template creation failed");
        var template = new Template
        {
            Title = "Untitled",
            CreatorId = userId!,
            ImageUrl = null,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow,
            Version = Guid.NewGuid()
        };
        await context.Templates.AddAsync(template);
        await context.SaveChangesAsync();

        return template.Id;
    }

    public async Task UpdateTemplate(Template? template)
    {
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

        dbTemplate.Title = template.Title;
        dbTemplate.Description = template.Description;
        dbTemplate.ImageUrl = template.ImageUrl;
        dbTemplate.IsPublic = template.IsPublic;
        dbTemplate.UpdatedAt = DateTime.UtcNow;
        dbTemplate.Version = Guid.NewGuid();

        // Don't make it async!!! Breaks application.
        context.SaveChanges();
    }

    public async Task<Template> GetTemplate(int id)
    {
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

    public IEnumerable<Template> GetAllTemplates()
    {
        try
        {
            var templates = context.Templates
                .Include(u => u.Creator);
            return templates;
        }
        catch (Exception e)
        {
            Console.WriteLine($"{e.Message}");
        }
        return [];
    }

    public IEnumerable<Template> GetTemplatesByUserId(string userId)
    {
        return context.Templates
            .Where(u => u.CreatorId == userId);
    }

    public async Task DeleteTemplate(Template? template)
    {
        if (template is null)
        {
            Console.WriteLine("Template not found");
            return;
        }

        context.Templates.Remove(template!);
        await context.SaveChangesAsync();
    }

    public bool DeleteTemplates(IEnumerable<Template> templates)
    {
        try
        {
            context.Templates.RemoveRange(templates);
            context.SaveChanges();
            return true;
        }
        catch (Exception e)
        {
            Console.WriteLine($"{e.Message}");
            return false;
        }
    }

    public async Task<bool> MakePublicAsync(Template? template)
    {
        if (template is null) return false;
        try
        {
            template!.IsPublic = true;
            await context.SaveChangesAsync();
            return true;
        }
        catch (Exception e)
        {
            Console.WriteLine($"{e.Message}");
        }

        return false;
    }

    public IEnumerable<Template> GetTopTemplates(int count)
    {
        try
        {
            var result = context.Templates
                .Where(t => t.IsPublic)
                .Include(t => t.Likes)
                .OrderByDescending(t => t.Likes.Count)
                .Take(count);
            return result;
        }
        catch (Exception e)
        {
            Console.WriteLine($"{e.Message}");
        }

        return [];
    }
}
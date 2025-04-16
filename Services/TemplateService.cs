using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using MudBlazor;
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

        dbTemplate.Title = template!.Title;
        dbTemplate.Description = template.Description;
        dbTemplate.ImageUrl = template.ImageUrl;
        dbTemplate.IsPublic = template.IsPublic;
        dbTemplate.UpdatedAt = DateTime.UtcNow;
        dbTemplate.Version = Guid.NewGuid();

        context.SaveChanges();
    }

    public async Task<Template?> GetTemplateAsync(int id)
    {
        return await context.Templates
            .Include(q => q.Questions)
            .Include(u => u.Creator)
            .FirstOrDefaultAsync(t => t.Id == id);
    }

    public async Task<IEnumerable<Template>> GetAllTemplatesAsync()
    {
        return await context.Templates
            .Include(q => q.Questions)
            .Include(u => u.Creator)
            .ToListAsync();
    }

    public async Task<IEnumerable<Template>> GetAllUserTemplatesAsync(string userId)
    {
        return await context.Templates
            .Where(u => u.CreatorId == userId)
            .Include(q => q.Questions)
            .ToListAsync();
    }
}
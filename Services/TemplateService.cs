using Microsoft.EntityFrameworkCore;
using XForms.Data;
using XForms.Data.Dtos;

namespace XForms.Services;

public class TemplateService(ApplicationDbContext context)
{
    public async Task CreateTemplate(CreateTemplateDto dto, string userId)
    {
        var template = new Template
        {
            Title = dto.Title,
            Description = dto.Description,
            ImageUrl = dto.ImageUrl,
            IsPublic = dto.IsPublic,
            CreatorId = userId,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        await context.Templates.AddAsync(template);
        await context.SaveChangesAsync();

        if (!dto.IsPublic && dto.AllowedUserIds.Any())
        {
            // Implement Template Access for defined users
        }
    }

    public async Task<Template?> GetTemplateAsync(int id)
    {
        return await context.Templates
            .Include(q => q.Questions)
            .Include(u => u.Creator)
            .FirstOrDefaultAsync(t => t.Id == id);
    }
}
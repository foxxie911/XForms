using Microsoft.EntityFrameworkCore;
using XForms.Data;

namespace XForms.Services;

public class SearchService(ApplicationDbContext context)
{
    public async Task<IEnumerable<Template>> SearchTemplateByTitleAsync(string searchString, int maxResult = 10)
    {
        if (string.IsNullOrWhiteSpace(searchString))
            return [];

        var templates = await context.Templates
            .Where(t => EF.Functions.ILike(t.Title, $"%{searchString}%") && t.IsPublic == true)
            .Include(t => t.Likes)
            .OrderByDescending(t => t.Likes.Count)
            .Take(maxResult)
            .ToListAsync();


        return templates;
    }

    public async Task<IEnumerable<string>> SearchTagsByNameAsync(string searchString, int maxResult = 5)
    {
        if (string.IsNullOrWhiteSpace(searchString)) return Array.Empty<string>();
        var tagNames = await context.Tags
            .Where(t => EF.Functions.ILike(t.Name, $"%{searchString}%"))
            .Select(t => t.Name)
            .Take(maxResult)
            .ToListAsync();
        return tagNames;
    }

}
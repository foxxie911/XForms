using Microsoft.EntityFrameworkCore;
using XForms.Data;

namespace XForms.Services;

public class SearchService(ApplicationDbContext context)
{

    public async Task<IEnumerable<Template>> SearchTemplateByTitleAsync(string searchString, int maxResult = 5)
    {
        if (string.IsNullOrWhiteSpace(searchString))
            return [];

        var templates = await context.Templates
            .Where(t =>
                EF.Functions.ILike(t.Title, $"%{searchString}%") && t.IsPublic == true)
            .Take(maxResult)
            .ToListAsync();

        return templates;
    }
}
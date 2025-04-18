using Microsoft.EntityFrameworkCore;
using XForms.Data;

namespace XForms.Services;

public class SearchService(ApplicationDbContext dbContext)
{
    private readonly ApplicationDbContext _context = dbContext;

    public async Task<IEnumerable<Template>> SearchTemplateByTitleAsync(string searchString, int maxResult = 5)
    {
        if (string.IsNullOrWhiteSpace(searchString))
            return [];

        var templates = await _context.Templates
            .Where(t =>
                EF.Functions.ILike(t.Title, $"%{searchString}%"))
            .Take(maxResult)
            .ToListAsync();

        return templates;
    }
}
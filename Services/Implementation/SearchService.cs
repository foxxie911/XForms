using Microsoft.EntityFrameworkCore;
using XForms.Data;
using XForms.Services.Interface;

namespace XForms.Services.Implementation;


public class SearchService(IDbContextFactory<ApplicationDbContext> contextFactory) : ISearchService
{
    public async Task<List<Template>> SearchTemplateByTitleAsync(string searchString, int maxResult = 10)
    {
        await using var context = await contextFactory.CreateDbContextAsync();
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

    public async Task<List<string>> SearchTagsByNameAsync(string searchString, int maxResult = 5)
    {
        await using var context = await contextFactory.CreateDbContextAsync();
        if (string.IsNullOrWhiteSpace(searchString)) return [];
        var tagNames = await context.Tags
            .Where(t => EF.Functions.ILike(t.Name, $"%{searchString}%"))
            .Select(t => t.Name)
            .Take(maxResult)
            .ToListAsync();
        return tagNames;
    }

}
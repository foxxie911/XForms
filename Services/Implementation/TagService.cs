using Microsoft.EntityFrameworkCore;
using XForms.Data;
using XForms.Services.Interface;

namespace XForms.Services.Implementation;

public class TagService(IDbContextFactory<ApplicationDbContext> contextFactory) : ITagService
{
    public async Task<List<Tag>> GetTagsAsync()
    {
        await using var context = await contextFactory.CreateDbContextAsync();
        try
        {
            var tags = await context.Tags
                .Include(t => t.Templates)
                .AsNoTracking()
                .ToListAsync();
            return tags;
        }
        catch (Exception e)
        {
            Console.WriteLine($"{e.Message}");
        }

        return null!;
    }

    public async Task<List<string>> FindTagsByTemplate(int templateId)
    {
        await using var context = await contextFactory.CreateDbContextAsync();
        try
        {
            var targetTemplate = await context.Templates
                .Where(t => t.Id == templateId)
                .Include(t => t.Tags)
                .AsNoTracking()
                .Select(t => t.Tags)
                .FirstOrDefaultAsync();
                
                var result = targetTemplate!.Select(t => t.Name)
                    .ToList();
                
            return result;
        }
        catch (Exception e)
        {
            Console.WriteLine($"{e.Message}");
        }

        return [];
    }

    public async Task<bool> CreateOrAddTagAsync(string tagName, int templateId)
    {
        if (string.IsNullOrWhiteSpace(tagName) && tagName.Contains(' ')) return false;
        if (tagName[0] != '#') tagName = '#' + tagName;
        tagName = tagName.ToLower();
        
        await using var context = await contextFactory.CreateDbContextAsync();
        
        try
        {
            var tag = await context.Tags
                          .FirstOrDefaultAsync(t => t.Name == tagName) ??
                      (
                          await context.Tags.AddAsync(
                              new Tag
                              {
                                  Name = tagName
                              })
                      ).Entity;

            await context.SaveChangesAsync();

            var templateTag = await context.TemplateTags
                .FirstOrDefaultAsync(tt => tt.TemplateId == templateId && tt.TagId == tag.Id);

            if (templateTag is null)
            {
                await context.TemplateTags.AddAsync(new TemplateTag
                {
                    TagId = tag.Id,
                    TemplateId = templateId
                });
            }

            await context.SaveChangesAsync();
            return true;
        }
        catch (Exception e)
        {
            Console.WriteLine($"{e.Message}");
        }

        return false;
    }

    public async Task<bool> DeleteTagAsync(string tagName, int templateId)
    {
        if (string.IsNullOrWhiteSpace(tagName)) return false;
        await using var context = await contextFactory.CreateDbContextAsync();
        try
        {
            var tagId = (await context.Tags.FirstOrDefaultAsync(t => t.Name == tagName))!.Id;
            var templateTag = await context.TemplateTags
                .FirstOrDefaultAsync(tt => tt.TemplateId == templateId && tt.TagId == tagId);
            context.TemplateTags.Remove(templateTag!);
            await context.SaveChangesAsync();
            return true;
        }
        catch (Exception e)
        {
            Console.WriteLine($"{e.Message}");
        }

        return false;
    }

    public async Task<List<Template>> FindTemplatesByTagAsync(string tagName)
    {
        await using var context = await contextFactory.CreateDbContextAsync();
        try
        {
            var templates = (await context.Tags
                .Where(t => t.Name == tagName)
                .Include(t => t.Templates)!
                .ThenInclude(t => t.Likes)
                .Select(t => t.Templates)
                .FirstOrDefaultAsync())!
                .Where(t => t.IsPublic)
                .ToList();
            return templates;
        }
        catch (Exception e)
        {
            Console.WriteLine($"{e.Message}");
        }

        return null!;
    }
}
using Microsoft.EntityFrameworkCore;
using XForms.Data;

namespace XForms.Services;

public class TagService(ApplicationDbContext context)
{
    public IEnumerable<Tag> GetTags()
    {
        return context.Tags
            .Include(t => t.Templates)
            .AsEnumerable();
    }

    public IEnumerable<string> FindTagsByTemplate(int templateId)
    {
        try
        {
            var result = context.Templates
                .Where(t => t.Id == templateId)
                .Include(t => t.Tags)
                .AsNoTracking()
                .Select(t => t.Tags)
                .FirstOrDefault()!
                .Select(t => t.Name)
                .ToList();
            return result;
        }
        catch (Exception e)
        {
            Console.WriteLine($"{e.Message}");
        }

        return [];
    }

    public bool CreateOrAddTag(string tagName, int templateId)
    {
        if (string.IsNullOrWhiteSpace(tagName) && tagName.Contains(' ')) return false;
        if (tagName[0] != '#') tagName = '#' + tagName;
        tagName = tagName.ToLower();
        try
        {
            var tag = context.Tags
                          .FirstOrDefault(t => t.Name == tagName) ??
                      (
                          context.Tags.Add(
                              new Tag
                              {
                                  Name = tagName
                              })
                      ).Entity;

            context.SaveChanges();

            var templateTag = context.TemplateTags
                .FirstOrDefault(tt => tt.TemplateId == templateId && tt.TagId == tag.Id);

            if (templateTag is null)
            {
                context.TemplateTags.Add(new TemplateTag
                {
                    TagId = tag.Id,
                    TemplateId = templateId
                });
            }

            context.SaveChanges();
            return true;
        }
        catch (Exception e)
        {
            Console.WriteLine($"{e.Message}");
        }

        return false;
    }

    public bool DeleteTag(string tagName, int templateId)
    {
        if (string.IsNullOrWhiteSpace(tagName)) return false;
        try
        {
            var tagId = context.Tags.FirstOrDefault(t => t.Name == tagName)!.Id;
            var templateTag = context.TemplateTags
                .FirstOrDefault(tt => tt.TemplateId == templateId && tt.TagId == tagId);
            context.TemplateTags.Remove(templateTag!);
            context.SaveChanges();
            return true;
        }
        catch (Exception e)
        {
            Console.WriteLine($"{e.Message}");
        }

        return false;
    }

    public IEnumerable<Template> FindTemplatesByTag(string tagName)
    {
        try
        {
            var templates = context.Tags
                .Where(t => t.Name == tagName)
                .Include(t => t.Templates)!
                .ThenInclude(t => t.Likes)
                .Select(t => t.Templates)
                .FirstOrDefault()!
                .Where(t => t.IsPublic == true);
            return templates;
        }
        catch (Exception e)
        {
            Console.WriteLine($"{e.Message}");
        }
        
        return null!;
    }
}
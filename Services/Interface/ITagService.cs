using XForms.Data;

namespace XForms.Services.Interface;

public interface ITagService
{
    Task<List<Tag>> GetTagsAsync();
    Task<List<string>> FindTagsByTemplate(int templateId);
    Task<bool> CreateOrAddTagAsync(string tagName, int templateId);
    Task<bool> DeleteTagAsync(string tagName, int templateId);
    Task<List<Template>> FindTemplatesByTagAsync(string tagName);
}

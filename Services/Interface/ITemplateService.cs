using XForms.Data;

namespace XForms.Services.Interface;


public interface ITemplateService
{
    Task<int> CreateTemplateAsync(string? userId);
    Task UpdateTemplateAsync(Template? template);
    Task<Template> GetTemplateIncludingQuestionAsync(int id);
    Task<Template> GetTemplateAsync(int id);
    IEnumerable<Template> GetAllTemplates();
    Task<List<Template>> GetTemplatesByUserId(string userId);
    Task DeleteTemplate(Template? template);
    Task<bool> DeleteTemplatesAsync(IEnumerable<Template> templates);
    Task<bool> MakePublicAsync(Template? template);
    Task<List<Template>> GetTopTemplates(int count);
}

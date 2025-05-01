using XForms.Data;

namespace XForms.Services.Interface;

public interface ISearchService
{
    Task<List<Template>> SearchTemplateByTitleAsync(string searchString, int maxResult = 10);
    Task<List<string>> SearchTagsByNameAsync(string searchString, int maxResult = 5);
}

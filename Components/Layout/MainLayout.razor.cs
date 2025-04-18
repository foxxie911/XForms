using Microsoft.AspNetCore.Components;
using XForms.Services;

namespace XForms.Components.Layout;

public partial class MainLayout : LayoutComponentBase
{
    // Dependency Injection
    [Inject] private SearchService? SearchService { get; set; }
    
    // Class variables
    private bool _drawerOpen = true;
    private string _searchString = string.Empty;
    private Data.Template? _targetTemplate;

    public async Task<IEnumerable<Data.Template>>? SearchTemplates(string? s, CancellationToken cancellationToken)
    {
        var  templates = await SearchService!.SearchTemplateByTitleAsync(_searchString);
        return templates;
    }

    public void CreateForm()
    {
        
    }
}
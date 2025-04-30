using Microsoft.AspNetCore.Components;
using XForms.Services;

namespace XForms.Components.Form;

public partial class ViewForm : ComponentBase
{
    [Parameter] public int Id { get; set; }
    
    // Dependency Injection
    [Inject] private TemplateService? TemplateService { get; set; }
    [Inject] private NavigationManager? NavigationManager { get; set; }
    [Inject] private LikeService? LikeService { get; set; }
    
    // Class Variable
    private Data.Template? _template;
    private int _totalLikeCount;
    
    protected override async Task OnParametersSetAsync()
    {
        await base.OnParametersSetAsync();
        _template = await TemplateService!.GetTemplate(Id);
        if (_template is null) NavigationManager!.NavigateTo($"/");
        _totalLikeCount = LikeService!.CountLikeByTemplateId(_template!.Id);
    }
}
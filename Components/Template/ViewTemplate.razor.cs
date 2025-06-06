using Microsoft.AspNetCore.Components;
using XForms.Services.Interface;

namespace XForms.Components.Template;

public partial class ViewTemplate : ComponentBase
{
    [Parameter] public int Id { get; set; }
    
    // Dependency Injection
    [Inject] private ITemplateService? TemplateService { get; set; }
    [Inject] private NavigationManager? NavigationManager { get; set; }
    [Inject] private ILikeService? LikeService { get; set; }
    
    // Class Variable
    private Data.Template? _template;
    private int _totalLikeCount;
    
    protected override async Task OnParametersSetAsync()
    {
        await base.OnParametersSetAsync();
        _template = await TemplateService!.GetTemplateIncludingQuestionAsync(Id);
        if (_template is null) NavigationManager!.NavigateTo($"/");
        _totalLikeCount = await LikeService!.CountLikeByTemplateIdAsync(_template!.Id);
    }
}
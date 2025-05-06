using Microsoft.AspNetCore.Components;
using XForms.Data;
using XForms.Services.Interface;

namespace XForms.Components.Pages;

public partial class TopPicks : ComponentBase
{
    // Parameter
    [Parameter] public required ApplicationUser CurrentUser { get; set; }

    // Dependency Injection
    [Inject] private ITemplateService? TemplateService { get; set; }

    // Class variable
    private List<Data.Template> _templates = [];

    protected override async Task OnInitializedAsync()
    {
        await base.OnInitializedAsync();
        _templates = await TemplateService!.GetTopTemplates(5);
    }
}
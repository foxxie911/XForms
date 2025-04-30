using Microsoft.AspNetCore.Components;
using MudBlazor;
using XForms.Data;
using XForms.Services;

namespace XForms.Components.Pages;

public partial class TopPicks : ComponentBase
{
    // Parameter
    [Parameter] public required ApplicationUser CurrentUser { get; set; }

    // Dependency Injection
    [Inject] private TemplateService? TemplateService { get; set; }
    [Inject] private FormService? FormService { get; set; }
    [Inject] private ISnackbar? Snackbar { get; set; }
    [Inject] private NavigationManager? NavigationManager { get; set; }

    // Class variable
    private IEnumerable<Data.Template> _templates = [];

    protected override void OnInitialized()
    {
        base.OnInitialized();
        _templates = TemplateService!.GetTopTemplates(5);
    }
}
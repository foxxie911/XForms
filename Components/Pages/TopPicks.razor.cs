using Microsoft.AspNetCore.Components;
using MudBlazor;
using XForms.Data;
using XForms.Services;
using XForms.Services.Implementation;

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
    private List<Data.Template> _templates = [];

    protected override async Task OnInitializedAsync()
    {
        await base.OnInitializedAsync();
        _templates = await TemplateService!.GetTopTemplates(5);
    }
}
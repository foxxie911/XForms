using Microsoft.AspNetCore.Components;
using MudBlazor;
using XForms.Data;
using XForms.Services.Implementation;

namespace XForms.Components.Pages;

public partial class TagCloud : ComponentBase
{
    [Parameter] public required ApplicationUser CurrentUser { get; set; }

    // Dependency Injection
    [Inject] private TagService? TagService { get; set; }
    [Inject] private FormService? FormService { get; set; }
    [Inject] private NavigationManager? NavigationManager { get; set; }
    [Inject] private ISnackbar? Snackbar { get; set; }

    // Class variables
    private List<Tag> _tags = [];
    private List<Data.Template> _templates = [];

    protected override async Task OnInitializedAsync()
    {
        await base.OnInitializedAsync();
        _tags = await TagService!.GetTagsAsync();
        if (_tags.Count != 0)
            _templates = (await TagService!.FindTemplatesByTagAsync(_tags.First().Name))
                .OrderByDescending(t => t.Likes.Count)
                .ToList();
    }

    private async Task ShowTemplates(string tagName)
    {
        _templates = (await TagService!.FindTemplatesByTagAsync(tagName))
            .OrderByDescending(t => t.Likes.Count)
            .ToList();
    }
}
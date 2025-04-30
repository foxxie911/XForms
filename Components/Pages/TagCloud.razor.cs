using Microsoft.AspNetCore.Components;
using MudBlazor;
using XForms.Data;
using XForms.Services;

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
    private IEnumerable<Tag>? _tags;
    private IEnumerable<Data.Template>? _templates;

    protected override void OnInitialized()
    {
        base.OnInitialized();
        _tags = TagService!.GetTags();
        if (_tags.Any())
            _templates = TagService!.FindTemplatesByTag(_tags.First().Name).OrderByDescending(t => t.Likes.Count);
    }

    private void ShowTemplates(string tagName)
    {
        _templates = TagService!.FindTemplatesByTag(tagName).OrderByDescending(t => t.Likes.Count);
    }
}
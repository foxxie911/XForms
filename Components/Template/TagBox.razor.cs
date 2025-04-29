using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using MudBlazor;
using XForms.Services;

namespace XForms.Components.Template;

public partial class TagBox : ComponentBase
{
    // Parameter
    [Parameter] public int TemplateId { get; set; }

    // Dependency Injection
    [Inject] private TagService? TagService { get; set; }
    [Inject] private SearchService? SearchService { get; set; }
    [Inject] private ISnackbar? Snackbar { get; set; }

    // Class Variable
    private string _searchString = string.Empty;
    private string _tagName = string.Empty;
    private IEnumerable<string> _tags = [];

    protected override void OnInitialized()
    {
        base.OnInitialized();
        _tags = TagService!.FindTagsByTemplate(TemplateId);
    }

    private async Task<IEnumerable<string>> SearchTag(string? searchString, CancellationToken cancellationToken)
    {
        var tags = await SearchService!.SearchTagsByNameAsync(searchString!);
        return tags;
    }

    private void AddTag(KeyboardEventArgs arg)
    {
        if (arg.Key == "Enter")
        {
            var succeed = TagService!.CreateOrAddTag(_tagName, TemplateId);
            if (!succeed) Snackbar!.Add("Tag creation failed", Severity.Error);
            _tags = TagService!.FindTagsByTemplate(TemplateId);
            StateHasChanged();
        }
    }

    private void DeleteTag(MudChip<string> tag)
    {
        TagService!.DeleteTag(tag.Value!, TemplateId);
        _tags = TagService!.FindTagsByTemplate(TemplateId);
        StateHasChanged();
    }
}
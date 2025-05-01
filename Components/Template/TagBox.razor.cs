using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using MudBlazor;
using XForms.Services;
using XForms.Services.Implementation;

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

    protected override async Task OnInitializedAsync()
    {
        await base.OnInitializedAsync();
        _tags = await TagService!.FindTagsByTemplate(TemplateId);
    }

    private async Task<IEnumerable<string>> SearchTag(string? searchString, CancellationToken cancellationToken)
    {
        var tags = await SearchService!.SearchTagsByNameAsync(searchString!);
        return tags;
    }

    private async Task AddTag(KeyboardEventArgs arg)
    {
        if (arg.Key == "Enter")
        {
            var succeed = await TagService!.CreateOrAddTagAsync(_tagName, TemplateId);
            if (!succeed) Snackbar!.Add("Tag creation failed", Severity.Error);
            _tags = await TagService!.FindTagsByTemplate(TemplateId);
            StateHasChanged();
        }
    }

    private async Task DeleteTag(MudChip<string> tag)
    {
        await TagService!.DeleteTagAsync(tag.Value!, TemplateId);
        _tags = await TagService!.FindTagsByTemplate(TemplateId);
        StateHasChanged();
    }
}
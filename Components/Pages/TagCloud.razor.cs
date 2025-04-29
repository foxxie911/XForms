using Microsoft.AspNetCore.Components;
using MudBlazor;
using XForms.Data;
using XForms.Services;

namespace XForms.Components.Pages;

public partial class TagCloud : ComponentBase
{
    [Parameter]
    public required ApplicationUser CurrentUser { get; set; }
    
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
        Task.Delay(100);
        _templates = TagService!.FindTemplatesByTag(_tags.First().Name).OrderByDescending(t => t.Likes.Count);
    }

    private void ShowTemplates(string tagName)
    {
        _templates = TagService!.FindTemplatesByTag(tagName).OrderByDescending(t => t.Likes.Count);
    }
    
    public void CreateForm(Data.Template? template)
    {
        if (template == null) return;
        var userForm = FormService!.FindFormByUserAndTemplateId(CurrentUser!.Id, template.Id);
        if (userForm is null)
        { 
            var formId = FormService!.CreateForm(CurrentUser!.Id, template.Id);
            if (formId > -1)
            {
                Snackbar!.Add("Form Successfully created", Severity.Success);
                NavigationManager!.NavigateTo($"/form/edit/{formId}", forceLoad: true);
            }
            Snackbar!.Add("Form Creation Failed", Severity.Error);
        }

        if (userForm is not null)
        {
            NavigationManager!.NavigateTo("/form/edit/" + userForm.Id, forceLoad: true);
        }
        
    }
}
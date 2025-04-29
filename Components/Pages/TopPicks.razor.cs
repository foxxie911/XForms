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
        if (!_templates.Any())
            Snackbar!.Add("Top template picks could not be loaded", Severity.Error);
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
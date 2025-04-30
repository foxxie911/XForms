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

    public void CreateForm(Data.Template? template)
    {
        if (template == null) return;

        var userForm = FormService!.FindFormByUserAndTemplateId(CurrentUser!.Id, template.Id);

        Task.Delay(100);

        if (userForm is not null)
        {
            NavigationManager!.NavigateTo("/form/edit/" + userForm.Id);
            return;
        }

        var formId = FormService!.CreateForm(CurrentUser!.Id, template.Id);

        if (formId == int.MinValue)
            Snackbar!.Add("Form Creation Failed", Severity.Error);

        Snackbar!.Add("Form Successfully created", Severity.Success);
        NavigationManager!.NavigateTo($"/form/edit/{formId}");
    }
}
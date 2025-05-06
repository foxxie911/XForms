using Microsoft.AspNetCore.Components;
using MudBlazor;
using XForms.Data;
using XForms.Services.Interface;

namespace XForms.Components.Pages;

public partial class FormCard : ComponentBase
{
    [Parameter]
    public required Data.Template ChosenTemplate { get; set; }
    [Parameter]
    public required ApplicationUser CurrentUser { get; set; }
    
    [Inject] private IFormService? FormService { get; set; }
    [Inject] private ISnackbar? Snackbar { get; set; }
    [Inject] private NavigationManager? NavigationManager { get; set; }
    
    public async Task CreateForm(Data.Template? template)
    {
        if (template == null) return;

        var userForm = await FormService!.FindFormByUserAndTemplateIdAsync(CurrentUser.Id, template.Id);

        if (userForm is not null)
        {
            NavigationManager!.NavigateTo("/form/edit/" + userForm.Id);
            return;
        }

        var formId = await FormService!.CreateFormAsync(CurrentUser.Id, template.Id);

        if (formId == int.MinValue)
            Snackbar!.Add("Form Creation Failed", Severity.Error);

        Snackbar!.Add("Form Successfully created", Severity.Success);
        NavigationManager!.NavigateTo($"/form/edit/{formId}");
    }

    private void ViewForm()
    {
        NavigationManager!.NavigateTo($"/template/view/{ChosenTemplate.Id}");
    }
}
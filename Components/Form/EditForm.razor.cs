using Microsoft.AspNetCore.Components;
using MudBlazor;
using XForms.Services;

namespace XForms.Components.Form;

public partial class EditForm : ComponentBase
{
    // Parameter
    [Parameter] public required int Id { get; set; }

    // Dependency Injection
    [Inject] private FormService? FormService { get; set; }
    [Inject] private ISnackbar? Snackbar { get; set; }
    [Inject] private NavigationManager? NavigationManager { get; set; }
    
    // Class Variable
    private Data.Form? _form;

    protected override async Task OnInitializedAsync()
    {
        await base.OnInitializedAsync();

        _form = await FormService!.FindFormByIdAsync(Id);
        if(_form is null) NavigationManager!.NavigateTo($"/");
    }


    private async Task SubmitForm()
    {
        var succeed = await FormService!.SubmitFormAsync(_form);
        if (succeed) Snackbar!.Add("Form Submitted", Severity.Success);
        if (!succeed) Snackbar!.Add("Form Submission Failed", Severity.Error);
    }
}
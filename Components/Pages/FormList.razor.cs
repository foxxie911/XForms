using Microsoft.AspNetCore.Components;
using MudBlazor;
using XForms.Services.Implementation;

namespace XForms.Components.Pages;

public partial class FormList : ComponentBase
{
    // Parameter
    [Parameter] public required string UserId { get; set; }

    // Dependency Injection
    [Inject] private FormService? FormService { get; set; }
    [Inject] private ISnackbar? Snackbar { get; set; }
    [Inject] private NavigationManager? NavigationManager { get; set; }

    // Class variables
    private HashSet<Data.Form> _selectedForms = [];
    private List<Data.Form> _forms = [];
    private MudDataGrid<Data.Form>? _formDataGrid;

    protected override async Task OnInitializedAsync()
    {
        await base.OnInitializedAsync();
        _forms = await FormService!.FindFormsByUserId(UserId);
    }

    private async Task DeleteSelectedForms()
    {
        Snackbar!.Configuration.PositionClass = Defaults.Classes.Position.BottomLeft;
        if (_selectedForms.Count == 0)
        {
            Snackbar!.Add("No form selected", Severity.Info);
            return;
        }

        var success = await FormService!.DeleteFormsAsync(_selectedForms);

        if (!success)
        {
            Snackbar!.Add("Forms failed to delete", Severity.Error);
            return;
        }

        _selectedForms.Clear();
        _forms = await FormService!.FindFormsByUserId(UserId);
        await _formDataGrid!.ReloadServerData();
        Snackbar!.Add("Forms successfully deleted", Severity.Success);
    }

    private void NavigateToForm(DataGridRowClickEventArgs<Data.Form> args)
    {
        _selectedForms.Clear();
        NavigationManager!.NavigateTo($"/form/edit/{args.Item.Id}");
    }
}
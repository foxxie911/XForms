using Microsoft.AspNetCore.Components;
using MudBlazor;
using XForms.Services;

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
    private HashSet<Data.Form>? _selectedForms = [];
    private IEnumerable<Data.Form>? _forms;
    private MudDataGrid<Data.Form>? _formDataGrid;

    protected override void OnInitialized()
    {
        base.OnInitialized();
        _forms = FormService!.FindFormsByUserId(UserId);
    }

    private void DeleteSelectedForms()
    {
        Snackbar!.Configuration.PositionClass = Defaults.Classes.Position.BottomLeft;
        if (_selectedForms!.Count == 0)
        {
            Snackbar!.Add("No form selected", Severity.Info);
            return;
        }

        var success = FormService!.DeleteForms(_selectedForms);

        if (success)
        {
            _selectedForms.Clear();
            _formDataGrid!.ReloadServerData();
            Snackbar!.Add("Forms successfully deleted", Severity.Success);
        }

        if (!success)
        {
            Snackbar!.Add("Forms failed to delete", Severity.Error);
        }
    }

    private void NavigateToForm(DataGridRowClickEventArgs<Data.Form> args)
    {
        _selectedForms!.Clear();
        NavigationManager!.NavigateTo($"/form/edit/{args.Item.Id}");
    }
}
using Microsoft.AspNetCore.Components;
using MudBlazor;
using XForms.Services;
using XForms.Services.Implementation;

namespace XForms.Components.Admin;

public partial class ManageTemplate : ComponentBase
{
    // Dependency Injection
    [Inject] private TemplateService? TemplateService { get; set; }
    [Inject] private ISnackbar? Snackbar { get; set; }
    [Inject] private NavigationManager? NavigationManager { get; set; }
    
    // Class variables
    private HashSet<Data.Template>? _selectedTemplates = [];
    private IEnumerable<Data.Template>? _templates;
    private MudDataGrid<Data.Template>? _templateDataGrid;

    protected override void OnInitialized()
    {
        base.OnInitialized();
        _templates = TemplateService!.GetAllTemplates();
    }
    
    private async Task DeleteSelectedTemplates()
    {
        Snackbar!.Configuration.PositionClass = Defaults.Classes.Position.BottomLeft;
        if (_selectedTemplates!.Count == 0)
        {
            Snackbar!.Add("No templates selected", Severity.Info);
            return;
        }

        var success = await TemplateService!.DeleteTemplatesAsync(_selectedTemplates);

        if (success)
        {
            _selectedTemplates.Clear();
            await _templateDataGrid!.ReloadServerData();
            Snackbar!.Add("Templates successfully deleted", Severity.Success);
        }

        if (!success)
        {
            Snackbar!.Add("Templates failed to delete", Severity.Error);
        }
    }
    
    private void NavigateToTemplate(DataGridRowClickEventArgs<Data.Template> args)
    {
        _selectedTemplates!.Clear();
        NavigationManager!.NavigateTo($"/template/edit/{args.Item.Id}");
    }
}
using Microsoft.AspNetCore.Components;
using MudBlazor;
using XForms.Services.Interface;

namespace XForms.Components.Pages;

public partial class TemplateList : ComponentBase
{
    // Parameter
    [Parameter] public required string UserId { get; set; }

    // Dependency Injection
    [Inject] private ITemplateService? TemplateService { get; set; }
    [Inject] private ISnackbar? Snackbar { get; set; }
    [Inject] private NavigationManager? NavigationManager { get; set; }

    // Class variables
    private HashSet<Data.Template> _selectedTemplates = [];
    private List<Data.Template> _templates = [];
    private MudDataGrid<Data.Template>? _templateDataGrid;

    protected override async Task OnInitializedAsync()
    {
        await base.OnInitializedAsync();
        _templates = await TemplateService!.GetTemplatesByUserId(UserId);
    }

    private async Task DeleteSelectedTemplates()
    {
        Snackbar!.Configuration.PositionClass = Defaults.Classes.Position.BottomLeft;
        if (_selectedTemplates.Count == 0)
        {
            Snackbar!.Add("No templates selected", Severity.Info);
            return;
        }

        var success = await TemplateService!.DeleteTemplatesAsync(_selectedTemplates);

        if (!success)
        {
            Snackbar!.Add("Templates failed to delete", Severity.Error);
            return;
        }

        _selectedTemplates.Clear();
        _templates = await TemplateService!.GetTemplatesByUserId(UserId);
        await _templateDataGrid!.ReloadServerData();
        Snackbar!.Add("Templates successfully deleted", Severity.Success);
    }
    
    private async Task NewTemplate()
    {
        var templateId = await TemplateService!.CreateTemplateAsync(UserId);
        NavigationManager!.NavigateTo($"/template/edit/{templateId}");
    }

    private void NavigateToTemplate(DataGridRowClickEventArgs<Data.Template> args)
    {
        _selectedTemplates.Clear();
        NavigationManager!.NavigateTo($"/template/edit/{args.Item.Id}");
    }
}
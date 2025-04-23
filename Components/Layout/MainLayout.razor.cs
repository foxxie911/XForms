using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Identity;
using MudBlazor;
using XForms.Data;
using XForms.Services;

namespace XForms.Components.Layout;

public partial class MainLayout : LayoutComponentBase
{
    // Dependency Injection
    [Inject] private SearchService? SearchService { get; set; }
    [Inject] private AuthenticationStateProvider? AuthenticationStateProvider { get; set; }
    [Inject] private UserManager<ApplicationUser>? UserManager { get; set; }
    [Inject] private ISnackbar? Snackbar { get; set; }
    [Inject] private NavigationManager? NavigationManager { get; set; }
    [Inject] private FormService? FormService { get; set; }
    
    
    // Class variables
    private ApplicationUser? _user;
    private bool _drawerOpen = true;
    private string _searchString = string.Empty;

    protected override async Task OnInitializedAsync()
    {
        await base.OnInitializedAsync();
        var authState = await AuthenticationStateProvider!.GetAuthenticationStateAsync();
        _user = await UserManager!.GetUserAsync(authState.User);
    }

    public async Task<IEnumerable<Data.Template>> SearchTemplates(string? s, CancellationToken cancellationToken)
    {
        var  templates = await SearchService!.SearchTemplateByTitleAsync(_searchString);
        return templates;
    }

    public async Task CreateForm(Data.Template? template)
    {
        if (template == null) return;
        var userForm = await FormService!.FindFormByUserAndTemplateId(_user!.Id, template.Id);
        if (userForm is null)
        { 
            var formId = await FormService!.CreateForm(_user!.Id, template.Id);
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
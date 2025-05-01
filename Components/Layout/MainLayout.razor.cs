using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Identity;
using MudBlazor;
using XForms.Data;
using XForms.Services.Implementation;

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
    private AuthenticationState? _authState;

    protected override async Task OnInitializedAsync()
    {
        await base.OnInitializedAsync();
        _authState = AuthenticationStateProvider!.GetAuthenticationStateAsync().Result;
        _user = UserManager!.GetUserAsync(_authState.User).Result;
    }

    private async Task<IEnumerable<Data.Template>> SearchTemplates(string? s, CancellationToken cancellationToken)
    {
        var templates = await SearchService!.SearchTemplateByTitleAsync(_searchString);
        return templates;
    }

    public async Task ManageSearchSelection(Data.Template? template)
    {
        if (template == null) return;
        
        var user = _authState!.User;
        if (user.Identity!.IsAuthenticated == false)
        {
            NavigationManager!.NavigateTo($"/form/view/{template.Id}");
            return;
        }

        var userForm = await FormService!.FindFormByUserAndTemplateIdAsync(_user!.Id, template.Id);
        if (userForm is not null)
        {
            NavigationManager!.NavigateTo("/form/edit/" + userForm.Id);
            return;
        }

        var formId = await FormService!.CreateFormAsync(_user!.Id, template.Id);

        if (formId == int.MinValue)
            Snackbar!.Add("Form Creation Failed", Severity.Error);

        Snackbar!.Add("Form Successfully created", Severity.Success);
        NavigationManager!.NavigateTo($"/form/edit/{formId}");
    }
}
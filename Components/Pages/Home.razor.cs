using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Identity;
using MudBlazor;
using XForms.Data;
using XForms.Services;

namespace XForms.Components.Pages;

public partial class Home : ComponentBase
{
    // Dependency Injection
    [Inject] private AuthenticationStateProvider? AuthenticationStateProvider { get; set; }
    [Inject] private UserManager<ApplicationUser>? UserManager { get; set; }
    [Inject] private NavigationManager? NavigationManager { get; set; }
    [Inject] private ISnackbar? Snackbar { get; set; }
    [Inject] private TemplateService? TemplateService { get; set; }
    
    // Class Variable
    private ApplicationUser? _currentUser;
    
    protected override async Task OnInitializedAsync()
    {
        await base.OnInitializedAsync();

        var authState = AuthenticationStateProvider!.GetAuthenticationStateAsync().Result;
        _currentUser = UserManager!.GetUserAsync(authState.User).Result;
    }

    private void NewTemplate()
    {
        if (_currentUser is null)
            Snackbar!.Add("No user logged in");
        var templateId = TemplateService!.CreateTemplate(_currentUser!.Id);
        NavigationManager!.NavigateTo($"/template/edit/{templateId}");
    }
}
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Identity;
using MudBlazor;
using XForms.Data;
using XForms.Services;
using XForms.Services.Implementation;

namespace XForms.Components.Pages;

public partial class Home : ComponentBase
{
    // Dependency Injection
    [Inject] private AuthenticationStateProvider? AuthenticationStateProvider { get; set; }
    [Inject] private UserManager<ApplicationUser>? UserManager { get; set; }
    
    // Class Variable
    private ApplicationUser? _currentUser;
    
    protected override async Task OnInitializedAsync()
    {
        await base.OnInitializedAsync();

        var authState = AuthenticationStateProvider!.GetAuthenticationStateAsync().Result;
        _currentUser = UserManager!.GetUserAsync(authState.User).Result;
    }
}
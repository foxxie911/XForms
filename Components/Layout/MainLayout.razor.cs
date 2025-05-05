using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Identity;
using MudBlazor;
using XForms.Data;
using XForms.Services.Implementation;
using XForms.Services.Interface;

namespace XForms.Components.Layout;

public partial class MainLayout : LayoutComponentBase
{
    // Dependency Injection
    [Inject] private SearchService? SearchService { get; set; }
    [Inject] private AuthenticationStateProvider? AuthenticationStateProvider { get; set; }
    [Inject] private UserManager<ApplicationUser>? UserManager { get; set; }
    [Inject] private IUserService? UserService { get; set; }
    [Inject] private ISnackbar? Snackbar { get; set; }
    [Inject] private NavigationManager? NavigationManager { get; set; }
    [Inject] private FormService? FormService { get; set; }


    // Class variables
    private ApplicationUser? _currentUser;
    private bool _isDarkMode;
    private MudTheme? _theme;
    private bool _drawerOpen = true;
    private string _searchString = string.Empty;
    private AuthenticationState? _authState;

    protected override async Task OnInitializedAsync()
    {
        await base.OnInitializedAsync();
        _theme = new MudTheme
        {
            PaletteLight = _lightPalette,
            PaletteDark = _darkPalette,
            LayoutProperties = new LayoutProperties()
        };
        _authState = AuthenticationStateProvider!.GetAuthenticationStateAsync().Result;
        _currentUser = UserManager!.GetUserAsync(_authState.User).Result;
        if(_currentUser != null) 
            _isDarkMode = _currentUser!.IsDarkMode;
    }

    private async Task DarkModeToggle()
    {
        _isDarkMode = !_isDarkMode;
        if (_currentUser != null)
        {
            _ = await UserService!.ToggleDarkMode(_isDarkMode, _currentUser.Id);
        }
    }

    private string DarkLightModeButtonIcon => _isDarkMode switch
    {
        true => Icons.Material.Rounded.LightMode,
        false => Icons.Material.Outlined.DarkMode,
    };
    
    private readonly PaletteLight _lightPalette = new()
    {
        Black = "#110e2d",
        AppbarText = "#424242",
        AppbarBackground = "rgba(255,255,255,0.8)",
        DrawerBackground = "#ffffff",
        GrayLight = "#e8e8e8",
        GrayLighter = "#f9f9f9",
    };

    private readonly PaletteDark _darkPalette = new()
    {
        Primary = "#7e6fff",
        Surface = "#1e1e2d",
        Background = "#1a1a27",
        BackgroundGray = "#151521",
        AppbarText = "#92929f",
        AppbarBackground = "rgba(26,26,39,0.8)",
        DrawerBackground = "#1a1a27",
        ActionDefault = "#74718e",
        ActionDisabled = "#9999994d",
        ActionDisabledBackground = "#605f6d4d",
        TextPrimary = "#b2b0bf",
        TextSecondary = "#92929f",
        TextDisabled = "#ffffff33",
        DrawerIcon = "#92929f",
        DrawerText = "#92929f",
        GrayLight = "#2a2833",
        GrayLighter = "#1e1e2d",
        Info = "#4a86ff",
        Success = " #30D158FF",
        Warning = "#ffb545",
        Error = "#ff3f5f",
        LinesDefault = "#33323e",
        TableLines = "#33323e",
        Divider = "#292838",
        OverlayLight = "#1e1e2d80",
    };

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
            NavigationManager!.NavigateTo($"/template/view/{template.Id}");
            return;
        }

        var userForm = await FormService!.FindFormByUserAndTemplateIdAsync(_currentUser!.Id, template.Id);
        if (userForm is not null)
        {
            NavigationManager!.NavigateTo("/form/edit/" + userForm.Id);
            return;
        }

        var formId = await FormService!.CreateFormAsync(_currentUser!.Id, template.Id);

        if (formId == int.MinValue)
            Snackbar!.Add("Form Creation Failed", Severity.Error);

        Snackbar!.Add("Form Successfully created", Severity.Success);
        NavigationManager!.NavigateTo($"/form/edit/{formId}");
    }
}
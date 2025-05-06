using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Identity;
using MudBlazor;
using XForms.Data;
using XForms.Services.Interface;

namespace XForms.Components.Form;

public partial class EditForm : ComponentBase
{
    // Parameter
    [Parameter] public required int Id { get; set; }

    // Dependency Injection
    [Inject] private IFormService? FormService { get; set; }
    [Inject] private AuthenticationStateProvider? AuthenticationStateProvider { get; set; }
    [Inject] private UserManager<ApplicationUser>? UserManager { get; set; }
    [Inject] private ILikeService? LikeService { get; set; }
    [Inject] private ISnackbar? Snackbar { get; set; }
    [Inject] private NavigationManager? NavigationManager { get; set; }

    // Class Variable
    private ApplicationUser? _currentUser;
    private Data.Form? _form;
    private int _totalLikesCount;
    private bool _isLiked;

    protected override async Task OnInitializedAsync()
    {
        await base.OnInitializedAsync();

        var authState = AuthenticationStateProvider!.GetAuthenticationStateAsync().Result;
        _currentUser = UserManager!.GetUserAsync(authState.User).Result;
    }

    protected override async Task OnParametersSetAsync()
    {
        await base.OnParametersSetAsync();
        _form = await FormService!.FindFormByIdAsync(Id);
        if (_form is null) NavigationManager!.NavigateTo($"/");
        _totalLikesCount = await LikeService!.CountLikeByTemplateIdAsync(_form!.Template!.Id);
        _isLiked = await LikeService!.IsLikedAsync(_currentUser!.Id, _form!.Template!.Id);
    }

    private async Task LikeOrUnlikeForm()
    {
        await LikeService!.LikeOrUnlikeTemplate(_currentUser!.Id, _form!.Template!.Id);
        _totalLikesCount = await LikeService!.CountLikeByTemplateIdAsync(_form.Template!.Id);
        _isLiked = await LikeService!.IsLikedAsync(_currentUser!.Id, _form!.Template!.Id);
        StateHasChanged();
    }
    
    private async Task SubmitForm()
    {
        var succeed = await FormService!.SubmitFormAsync(_form);
        if (succeed)
        {
            Snackbar!.Add("Form Submitted", Severity.Success);
            StateHasChanged();
            return;
        }
        Snackbar!.Add("Form Submission Failed", Severity.Error);
    }
}
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Identity;
using MudBlazor;
using XForms.Data;
using XForms.Services;

namespace XForms.Components.Form;

public partial class EditForm : ComponentBase
{
    // Parameter
    [Parameter] public required int Id { get; set; }

    // Dependency Injection
    [Inject] private FormService? FormService { get; set; }
    [Inject] private AuthenticationStateProvider? AuthenticationStateProvider { get; set; }
    [Inject] private UserManager<ApplicationUser>? UserManager { get; set; }
    [Inject] private LikeService? LikeService { get; set; }
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
        _totalLikesCount = LikeService!.CountLikeByTemplateId(_form!.Template!.Id);
        _isLiked = LikeService!.IsLikedAsync(_currentUser!.Id, _form!.Template!.Id);
    }

    private void LikeOrUnlikeForm()
    {
        LikeService!.LikeOrUnlikeTemplate(_currentUser!.Id, _form!.Template!.Id);
        _totalLikesCount = LikeService!.CountLikeByTemplateId(_form.Template!.Id);
        _isLiked = LikeService!.IsLikedAsync(_currentUser!.Id, _form!.Template!.Id);
        StateHasChanged();
    }
    
    private void SubmitForm()
    {
        var succeed = FormService!.SubmitFormAsync(_form);
        if (succeed)
        {
            Snackbar!.Add("Form Submitted", Severity.Success);
            StateHasChanged();
            return;
        }
        Snackbar!.Add("Form Submission Failed", Severity.Error);
    }
}
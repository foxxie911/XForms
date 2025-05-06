using CloudinaryDotNet;
using CloudinaryDotNet.Actions;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.AspNetCore.Identity;
using MudBlazor;
using XForms.Data;
using XForms.Services.Interface;

namespace XForms.Components.Template;

public partial class EditTemplate : ComponentBase
{
    // Parameter
    [Parameter] public int Id { get; set; }

    // Dependency Injection
    [Inject] private ITemplateService? TemplateService { get; set; }
    [Inject] private AuthenticationStateProvider? AuthenticationStateProvider { get; set; }
    [Inject] private UserManager<ApplicationUser>? UserManager { get; set; }
    [Inject] private ILikeService? LikeService { get; set; }
    [Inject] private Cloudinary? Cloudinary { get; set; }
    [Inject] private ISnackbar? Snackbar { get; set; }

    // Global Class Variable
    private ApplicationUser? _currentUser;
    private Data.Template? _template;
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
        _template = await TemplateService!.GetTemplateAsync(Id);
        _totalLikesCount = await LikeService!.CountLikeByTemplateIdAsync(_template.Id);
        _isLiked = await LikeService!.IsLikedAsync(_currentUser!.Id, _template.Id);
    }

    // Template Section Start
    private async Task UpdateTemplate()
    {
        var succeed = await TemplateService!.UpdateTemplateAsync(_template!);
        if (succeed)
            Snackbar!.Add("Template Updated", Severity.Info);
    }

    private async Task PublishTemplatePublic()
    {
        var success = await TemplateService!.MakePublicAsync(_template);
        if (success)
        {
            Snackbar!.Add("Template  published successfully", Severity.Success);
            return;
        }

        Snackbar!.Add("Template publish failed", Severity.Error);
    }

    private async Task LikeOrUnlikeTemplate()
    {
        await LikeService!.LikeOrUnlikeTemplate(_currentUser!.Id, _template!.Id);
        _totalLikesCount = await LikeService!.CountLikeByTemplateIdAsync(_template.Id);
        _isLiked = await LikeService!.IsLikedAsync(_currentUser!.Id, _template.Id);
        StateHasChanged();
    }

    // Photo Section Start
    private async Task UploadCoverPhoto(IBrowserFile? coverImageFile)
    {
        const int maxFileSize = 2 * 1024 * 1024;
        Snackbar!.Configuration.PositionClass = Defaults.Classes.Position.BottomLeft;
        if (coverImageFile is null)
            Snackbar!.Add("No file selected", Severity.Info);
        if (coverImageFile!.Size > maxFileSize)
            Snackbar!.Add("File too large. Max file size is 2MB", Severity.Error);
        try
        {
            var uploadParams = new ImageUploadParams()
            {
                File = new FileDescription
                (
                    coverImageFile.Name,
                    coverImageFile.OpenReadStream(maxFileSize)
                ),
                PublicId = $"coverImage/{coverImageFile.Name}",
                Transformation = new Transformation().Height(150).Width(912).Crop("auto")
            };
            var uploadResult = await Cloudinary!.UploadAsync(uploadParams);
            _template!.ImageUrl = uploadResult.SecureUrl.ToString();
            var succeed = await TemplateService!.UpdateTemplateAsync(_template);
            if (succeed)
                Snackbar!.Add("Avatar uploaded successfully!", Severity.Success);
        }
        catch (Exception e)
        {
            Snackbar!.Add("Upload failed!", Severity.Error);
            Console.WriteLine(e.Message);
        }
        finally
        {
            StateHasChanged();
        }
    }

    private async Task RemoveCoverPhoto()
    {
        var deleteParam = new DeletionParams(_template!.ImageUrl);
        var result = await Cloudinary!.DestroyAsync(deleteParam);
        if (result.Error is not null)
            Snackbar!.Add($"{result.Error.Message}", Severity.Error);
        _template!.ImageUrl = null!;
        var succeed = await TemplateService!.UpdateTemplateAsync(_template!);
        if (succeed)
        {
            Snackbar!.Add("Cover photo successfully removed", Severity.Success);
            StateHasChanged();
        }
    }

    private async Task ReplaceCoverPhoto(IBrowserFile? coverImageFile)
    {
        await RemoveCoverPhoto();
        await UploadCoverPhoto(coverImageFile);
    }
    // Photo Section End
}
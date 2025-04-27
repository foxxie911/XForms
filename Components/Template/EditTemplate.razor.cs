using CloudinaryDotNet;
using CloudinaryDotNet.Actions;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.AspNetCore.Identity;
using MudBlazor;
using XForms.Data;
using XForms.Services;

namespace XForms.Components.Template;

public partial class EditTemplate : ComponentBase
{
    // Parameter
    [Parameter] public int Id { get; set; }

    // Dependency Injection
    [Inject] private TemplateService? TemplateService { get; set; }
    [Inject] private AuthenticationStateProvider? AuthenticationStateProvider { get; set; }
    [Inject] private UserManager<ApplicationUser>? UserManager { get; set; }
    [Inject] private LikeService? LikeService { get; set; }
    [Inject] private Cloudinary? Cloudinary { get; set; }
    [Inject] private ISnackbar? Snackbar { get; set; }

    // Global Class Variable
    private ApplicationUser? _currentUser;
    private Data.Template? _template;
    private string? _coverImagePublicId;
    private int _totalLikesCount;
    private bool _isLiked;

    protected override async Task OnInitializedAsync()
    {
        await base.OnInitializedAsync();
        var authState = await AuthenticationStateProvider!.GetAuthenticationStateAsync();
        _currentUser = await UserManager!.GetUserAsync(authState.User);
        _template = await TemplateService!.GetTemplate(Id); 
        await Task.Delay(500);
        _totalLikesCount = await LikeService!.CountLikeByTemplateIdAsync(_template.Id);
        _isLiked = await LikeService!.IsLikedAsync(_currentUser!.Id, _template.Id);
    }

    // Template Section Start
    private async Task UpdateTemplate()
    {
        await TemplateService!.UpdateTemplate(_template);
    }

    private async Task PublishTemplatePublic()
    {
        var success = await TemplateService!.MakePublicAsync(_template);
        if (success) 
            Snackbar!.Add("Template  published successfully", Severity.Success);
        if (!success) 
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
        if (coverImageFile is null)
            Snackbar!.Add("No file selected", Severity.Info);
        if (coverImageFile!.Size > maxFileSize)
            Snackbar!.Add("File too large. Max file size is 2MB", Severity.Error);
        try
        {
            _coverImagePublicId = $"coverImage/{coverImageFile.Name}";
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
            Snackbar!.Add("Avatar uploaded successfully!", Severity.Success);
            Console.WriteLine(_template!.ImageUrl);
        }
        catch (Exception e)
        {
            Snackbar!.Add("Upload failed!", Severity.Error);
            Snackbar!.Add($"{e.Message}", Severity.Error);
        }
        finally
        {
            StateHasChanged();
        }
    }

    private async Task RemoveCoverPhoto()
    {
        var deleteParam = new DeletionParams(_coverImagePublicId);
        var result = await Cloudinary!.DestroyAsync(deleteParam);
        if (result.Error is not null)
            Snackbar!.Add($"{result.Error.Message}", Severity.Error);
        _coverImagePublicId = null;
        _template!.ImageUrl = null!;
        StateHasChanged();
    }

    private async Task ReplaceCoverPhoto(IBrowserFile? coverImageFile)
    {
        await RemoveCoverPhoto();
        await UploadCoverPhoto(coverImageFile);
    }
    // Photo Section End
}
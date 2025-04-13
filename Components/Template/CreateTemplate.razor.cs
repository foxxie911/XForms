using CloudinaryDotNet;
using CloudinaryDotNet.Actions;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.AspNetCore.Identity;
using MudBlazor;
using XForms.Data;
using XForms.Data.Dtos;
using XForms.Services;

namespace XForms.Components.Template;

public partial class CreateTemplate : ComponentBase
{
    // Dependency Injection
    [Inject] private TemplateService? TemplateService { get; set; }
    [Inject] private AuthenticationStateProvider? AuthenticationStateProvider { get; set; }
    [Inject] private UserManager<ApplicationUser>? UserManager { get; set; }
    [Inject] private Cloudinary? Cloudinary { get; set; }
    [Inject] private ISnackbar? Snackbar { get; set; }

    // Class Parameters
    private readonly CreateTemplateDto _createTemplateDto = new();
    private AuthenticationState? AuthState { get; set; }
    private string? CoverImagePublicId { get; set; }

    protected override async Task OnInitializedAsync()
    {
        await base.OnInitializedAsync();
        AuthState = await AuthenticationStateProvider!.GetAuthenticationStateAsync();
    }

    private async Task UploadCoverPhoto(IBrowserFile? coverImageFile)
    {
        const int maxFileSize = 2 * 1024 * 1024;
        if (coverImageFile is null)
            Snackbar!.Add("No file selected", Severity.Info);

        if (coverImageFile!.Size > maxFileSize)
            Snackbar!.Add("File too large. Max file size is 2MB", Severity.Error);

        try
        {
            CoverImagePublicId = $"coverImage/{coverImageFile.Name}";
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
            _createTemplateDto.ImageUrl = uploadResult.SecureUrl.ToString();
            Snackbar!.Add("Avatar uploaded successfully!", Severity.Success);
            Console.WriteLine(_createTemplateDto.ImageUrl);
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
        var deleteParam = new DeletionParams(CoverImagePublicId);
        var result = await Cloudinary!.DestroyAsync(deleteParam);
        if (result.Error is not null)
            Snackbar!.Add($"{result.Error.Message}", Severity.Error);
        CoverImagePublicId = null;
        _createTemplateDto.ImageUrl = null!;
        StateHasChanged();
    }
    private async Task ReplaceCoverPhoto(IBrowserFile? coverImageFile)
    {
        await RemoveCoverPhoto();
        await UploadCoverPhoto(coverImageFile);
    }
    
    private async Task SaveTemplate()
    {
        try
        {
            var user = await UserManager!.GetUserAsync(AuthState!.User);
            var userId = user!.Id;
            await TemplateService!.CreateTemplate(_createTemplateDto, userId);
        }
        catch (Exception e)
        {
            Snackbar!.Add($"{e.Message}", Severity.Error);
        }
    }
}
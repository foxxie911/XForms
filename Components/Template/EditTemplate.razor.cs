using CloudinaryDotNet;
using CloudinaryDotNet.Actions;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
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
    [Inject] private Cloudinary? Cloudinary { get; set; }
    [Inject] private ISnackbar? Snackbar { get; set; }
    [Inject] private QuestionService? QuestionService { get; set; }

    // Global Class Variable
    private Data.Template? _template;
    private MudDropContainer<Question>? _dragDropContainer;
    private string? _coverImagePublicId;

    protected override async Task OnInitializedAsync()
    {
        await base.OnInitializedAsync();
        _template = await TemplateService!.GetTemplateAsync(Id);
    }

    private async Task UpdateTemplate()
    {
        await TemplateService!.UpdateTemplate(_template);
    }

    // Question Section Start
    private void CreateQuestion()
    {
        Snackbar!.Configuration.PositionClass = Defaults.Classes.Position.BottomLeft;
        _ = QuestionService!.CreateQuestion(Id);
        Snackbar.Add("Question created", Severity.Success);
        _dragDropContainer?.Refresh();
        StateHasChanged();
    }

    private async Task DeleteQuestion(Question question)
    {
        Snackbar!.Configuration.PositionClass = Defaults.Classes.Position.BottomLeft;
        await QuestionService!.DeleteQuestion(question);
        Snackbar!.Add("Question Deleted", Severity.Success);
        _dragDropContainer?.Refresh();
    }

    private void ReorderQuestions(MudItemDropInfo<Question> droppedItem)
    {
        var droppedQuestion = droppedItem.Item;
        var dropIndex = droppedItem.IndexInZone;
        _ = QuestionService!.RearrangeQuestionUponDrag(droppedQuestion!, dropIndex);
        StateHasChanged();
    }
    // Question Section End

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
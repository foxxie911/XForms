using Microsoft.AspNetCore.Components;
using MudBlazor;
using XForms.Data;
using XForms.Services;

namespace XForms.Components.Template;

public partial class EditQuestions : ComponentBase
{
    // Parameter
    [Parameter]
    public required Data.Template Template { get; set; }
    
    // Dependency Injection
    [Inject] private ISnackbar? Snackbar { get; set; }
    [Inject] private QuestionService? QuestionService { get; set; }
    
    // Class variables
    private MudDropContainer<Question>? _dragDropContainer;
    
    private void CreateQuestion()
    {
        Snackbar!.Configuration.PositionClass = Defaults.Classes.Position.BottomLeft;
        _ = QuestionService!.CreateQuestion(Template.Id);
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
}
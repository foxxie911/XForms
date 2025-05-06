using Microsoft.AspNetCore.Components;
using MudBlazor;
using XForms.Data;
using XForms.Services.Interface;

namespace XForms.Components.Template;

public partial class EditQuestions : ComponentBase
{
    // Parameter
    [Parameter] public required int TemplateId { get; set; }

    // Dependency Injection
    [Inject] private ISnackbar? Snackbar { get; set; }
    [Inject] private IQuestionService? QuestionService { get; set; }

    // Class variables
    private MudDropContainer<Question>? _dragDropContainer;
    private List<Question> _questions = [];

    protected override async Task OnInitializedAsync()
    {
        await base.OnInitializedAsync();
        _questions = await QuestionService!.GetQuestionsByTemplateIdAsync(TemplateId);
    }

    private async Task CreateQuestion()
    {
        Snackbar!.Configuration.PositionClass = Defaults.Classes.Position.BottomLeft;
        var succeed = await QuestionService!.CreateQuestionAsync(TemplateId);
        if (!succeed)
        {
            Snackbar!.Add("Question failed to create", Severity.Error);
            return;
        }

        Snackbar.Add("Question created", Severity.Success);
        _questions = await QuestionService!.GetQuestionsByTemplateIdAsync(TemplateId);
        StateHasChanged();
        _dragDropContainer?.Refresh();
    }

    private async Task DeleteQuestion(Question question)
    {
        Snackbar!.Configuration.PositionClass = Defaults.Classes.Position.BottomLeft;
        var updatedQuestion = _questions.Where(q => q.Order > question.Order).ToList();
        foreach (var updateQuestion in updatedQuestion)
        {
            updateQuestion.Order -= 1;
        }

        var succeed = await QuestionService!.DeleteQuestionAsync(question, updatedQuestion);
        if (!succeed)
        {
            Snackbar!.Add("Question failed to delete", Severity.Error);
            return;
        }

        Snackbar!.Add("Question Deleted", Severity.Success);
        _questions = await QuestionService!.GetQuestionsByTemplateIdAsync(TemplateId);
        StateHasChanged();
        _dragDropContainer?.Refresh();
    }

    private async Task ReorderQuestions(MudItemDropInfo<Question> dropInfo)
    {
        var droppedQuestion = dropInfo.Item;
        var dropIndex = dropInfo.IndexInZone;


        var oldIndex = droppedQuestion!.Order;

        if (oldIndex == dropIndex)
            return;

        List<Question> questions = [];
        if (oldIndex < dropIndex)
        {
            questions = _questions
                .Where(q => q.Order > oldIndex && q.Order <= dropIndex).ToList();
            foreach (var question in questions)
            {
                question.Order -= 1;
            }
        }

        if (oldIndex > dropIndex)
        {
            questions = _questions
                .Where(q => q.Order < oldIndex && q.Order >= dropIndex).ToList();
            foreach (var question in questions)
            {
                question.Order += 1;
            }
        }

        droppedQuestion.Order = dropIndex;
        questions.Add(droppedQuestion);

        var succeed = await QuestionService!.RearrangeQuestionUponDrag(questions);
        if (!succeed)
        {
            _questions = await QuestionService!.GetQuestionsByTemplateIdAsync(TemplateId);
            StateHasChanged();
            _dragDropContainer?.Refresh();
        }
    }
}
using Microsoft.AspNetCore.Components;
using MudBlazor;
using XForms.Data;
using XForms.Services.Interface;

namespace XForms.Components.Template;

public partial class ResponseList : ComponentBase
{
    // Parameter
    [Parameter] public required int TemplateId { get; set; }

    // Dependency Injection
    [Inject] private IQuestionService? QuestionService { get; set; }
    [Inject] private IAnswerService? AnswerService { get; set; }
    [Inject] private ISnackbar? Snackbar { get; set; }
    [Inject] private NavigationManager? NavigationManager { get; set; }

    // Class variables
    private IEnumerable<Question> _questions = [];
    private IEnumerable<Answer> _answers = [];
    private MudDataGrid<Answer>? _answerDataGrid;

    protected override async Task OnInitializedAsync()
    {
        await base.OnInitializedAsync();

        _questions = await QuestionService!.GetQuestionsByTemplateIdAsync(TemplateId);
        _answers = (await AnswerService!
            .GetSubmittedAnswersByQuestionIds(_questions.Select(q => q.Id)))
            .OrderBy(a => a.Question.Order);
    }
}
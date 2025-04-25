using Microsoft.AspNetCore.Components;
using MudBlazor;
using XForms.Data;
using XForms.Services;

namespace XForms.Components.Template;

public partial class ResponseList : ComponentBase
{
    // Parameter
    [Parameter] public required int TemplateId { get; set; }

    // Dependency Injection
    [Inject] private QuestionService? QuestionService { get; set; }
    [Inject] private AnswerService? AnswerService { get; set; }
    [Inject] private ISnackbar? Snackbar { get; set; }
    [Inject] private NavigationManager? NavigationManager { get; set; }

    // Class variables
    private IEnumerable<Question> _questions = [];
    private IEnumerable<Answer> _answers = [];
    private MudDataGrid<Answer>? _answerDataGrid;

    protected override void OnInitialized()
    {
        base.OnInitialized();

        _questions = QuestionService!.GetTemplateQuestionById(TemplateId);
        _answers = AnswerService!
            .GetAnswersByQuestions(_questions.Select(q => q.Id))
            .OrderBy(a => a.Question.Order);
    }
}
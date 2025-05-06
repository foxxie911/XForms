using Microsoft.AspNetCore.Components;
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

    // Class variables
    private IEnumerable<Question> _questions = [];
    private IEnumerable<Answer> _answers = [];

    protected override async Task OnInitializedAsync()
    {
        await base.OnInitializedAsync();

        _questions = await QuestionService!.GetQuestionsByTemplateIdAsync(TemplateId);
        _answers = (await AnswerService!
            .GetSubmittedAnswersByQuestionIds(_questions.Select(q => q.Id)))
            .OrderBy(a => a.Question!.Order);
    }
}
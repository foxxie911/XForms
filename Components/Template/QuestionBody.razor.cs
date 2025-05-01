using Microsoft.AspNetCore.Components;
using XForms.Data;
using XForms.Services;
using XForms.Services.Implementation;

namespace XForms.Components.Template;

public partial class QuestionBody : ComponentBase
{
    [Inject] private QuestionService? QuestionService { get; set; }
    [Parameter]
    public Question? Question { get; set; }
    [Parameter]
    public EventCallback<Question> QuestionDeleted { get; set; }

    private void UpdateQuestion()
    {
        _ = QuestionService!.UpdateQuestionAsync(Question!);
    }

    private void OnQuestionDeleted()
    {
        QuestionDeleted.InvokeAsync(Question);
    }
}
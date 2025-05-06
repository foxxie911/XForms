using Microsoft.AspNetCore.Components;
using XForms.Data;
using XForms.Services;
using XForms.Services.Implementation;
using XForms.Services.Interface;

namespace XForms.Components.Template;

public partial class QuestionBody : ComponentBase
{
    [Parameter]
    public Question? Question { get; set; }
    [Parameter]
    public EventCallback<Question> QuestionDeleted { get; set; }
   
    // Dependency Injection
    [Inject] private IQuestionService? QuestionService { get; set; }
    
    private void UpdateQuestion()
    {
        _ = QuestionService!.UpdateQuestionAsync(Question!);
    }

    private void OnQuestionDeleted()
    {
        QuestionDeleted.InvokeAsync(Question);
    }
}
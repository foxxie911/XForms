using Microsoft.AspNetCore.Components;
using MudBlazor;
using XForms.Data;
using XForms.Services.Interface;

namespace XForms.Components.Form;

public partial class AnswerBody : ComponentBase
{
    // Parameter
    [Parameter] public required Question Question { get; set; }
    [Parameter] public required int FormId { get; set; }
    [Parameter] public bool IsSubmitted { get; set; }

    // Dependency Injection
    [Inject] public IAnswerService? AnswerService { get; set; }
    [Inject] public ISnackbar? Snackbar { get; set; }

    // Class Variable
    private Answer? _answer;

    protected override async Task OnParametersSetAsync()
    {
        await base.OnParametersSetAsync();
        _answer = await AnswerService!.CreateOrShowAnswer(Question.Id, FormId);
        if (_answer is null) Snackbar!.Add($"Answer creation failed for Question: {Question.Title}", Severity.Error);
    }

    private void UpdateAnswer()
    {
        _ = AnswerService!.UpdateAnswerAsync(_answer);
    }
}
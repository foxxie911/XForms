using XForms.Data;

namespace XForms.Services.Interface;

public interface IAnswerService
{
    Task<Answer> CreateOrShowAnswer(int questionId, int formId);
    Task<bool> UpdateAnswerAsync(Answer? answer);
    Task<List<Answer>> GetSubmittedAnswersByQuestions(IEnumerable<int> questionIds);
}

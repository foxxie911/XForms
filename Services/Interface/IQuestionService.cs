using XForms.Data;

namespace XForms.Services.Interface;

public interface IQuestionService
{
    Task<bool> CreateQuestionAsync(int templateId);
    Task<bool> RearrangeQuestionUponDrag(IEnumerable<Question> questions);
    Task<bool> UpdateQuestionAsync(Question question);
    Task<bool> DeleteQuestionAsync(Question question, List<Question> updatedQuestions);
    Task<List<Question>> GetQuestionsByTemplateIdAsync(int templateId);
}

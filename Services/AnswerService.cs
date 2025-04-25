using Microsoft.EntityFrameworkCore;
using XForms.Data;

namespace XForms.Services;

public class AnswerService(ApplicationDbContext context)
{
    public async Task<Answer> CreateOrShowAnswerAsync(int questionId, int formId)
    {
        var answer = context.Answers.FirstOrDefault(q => q.FormId == formId && q.QuestionId == questionId);
        if (answer is not null) return answer;
        try
        {
            var answerEntity = await context.AddAsync(new Answer
            {
                QuestionId = questionId,
                FormId = formId,
                Version = Guid.NewGuid()
            });
            context.SaveChanges();
            return answerEntity.Entity;
        }
        catch (Exception e)
        {
            Console.WriteLine($"{e.Message}");
        }

        return null!;
    }

    public async Task<bool> UpdateAnswerAsync(Answer? answer)
    {
        try
        {
            context.Answers.Update(answer!);
            context.SaveChanges();
            return true;
        }
        catch (Exception e)
        {
            Console.WriteLine($"{e.Message}");
        }

        return false;
    }

    public IEnumerable<Answer> GetAnswersByQuestions(IEnumerable<int> questionIds)
    {
        try
        {
            var answers = context.Answers
                .Where(a => questionIds.Contains(a.QuestionId))
                .Include(a => a.Question)
                .Include(a => a.Form.Creator)
                .AsNoTracking();
            return answers;
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
        }
        return [];
    }
}
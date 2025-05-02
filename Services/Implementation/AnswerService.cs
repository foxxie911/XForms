using Microsoft.EntityFrameworkCore;
using XForms.Data;
using XForms.Services.Interface;

namespace XForms.Services.Implementation;

public class AnswerService(IDbContextFactory<ApplicationDbContext> contextFactory) : IAnswerService
{
    public async Task<Answer> CreateOrShowAnswer(int questionId, int formId)
    {
        await using var context = await contextFactory.CreateDbContextAsync();
        var answer = await context.Answers.FirstOrDefaultAsync(q => q.FormId == formId && q.QuestionId == questionId);
        if (answer is not null) return answer;
        try
        {
            var answerEntity = await context.AddAsync(new Answer()
            {
                QuestionId = questionId,
                FormId = formId,
                Version = Guid.NewGuid()
            });
            await context.SaveChangesAsync();
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
        await using var context = await contextFactory.CreateDbContextAsync();
        try
        {
            context.Answers.Update(answer!);
            await context.SaveChangesAsync();
            return true;
        }
        catch (Exception e)
        {
            Console.WriteLine($"{e.Message}");
        }

        return false;
    }

    public async Task<List<Answer>> GetSubmittedAnswersByQuestions(IEnumerable<int> questionIds)
    {
        await using var context = await contextFactory.CreateDbContextAsync();
        try
        {
            var answers = await context.Answers
                .Where(a => questionIds.Contains(a.QuestionId))
                .Include(a => a.Question)
                .Include(a => a.Form)
                .ThenInclude(f => f.Creator)
                .Where(a => a.Form.IsSubmitted == true)
                .AsNoTracking()
                .ToListAsync();
            return answers;
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
        }
        return [];
    }
}
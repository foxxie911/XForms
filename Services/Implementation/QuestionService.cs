using Microsoft.EntityFrameworkCore;
using XForms.Data;
using XForms.Services.Interface;
using Exception = System.Exception;

namespace XForms.Services.Implementation;

public class QuestionService(IDbContextFactory<ApplicationDbContext> contextFactory) : IQuestionService
{
    public async Task<bool> CreateQuestionAsync(int templateId)
    {
        await using var context = await contextFactory.CreateDbContextAsync();

        try
        {
            await context.Questions.AddAsync(new Question()
            {
                Title = "Untitled",
                Type = QuestionType.Singleline,
                Order = await context.Questions.Where(q => q.TemplateId == templateId).CountAsync(),
                TemplateId = templateId,
                Version = Guid.NewGuid()
            });
            await context.SaveChangesAsync();
            return true;
        }

        catch (Exception e)
        {
            Console.WriteLine($@"{e.Message}");
        }

        return false;
    }

    public async Task<bool> RearrangeQuestionUponDrag(IEnumerable<Question> questions)
    {
        await using var context = await contextFactory.CreateDbContextAsync();
        try
        {
            context.UpdateRange(questions);
            await context.SaveChangesAsync();
            return true;
        }
        catch (Exception e)
        {
            Console.WriteLine($@"{e.Message}");
        }

        return false;
    }

    public async Task<bool> UpdateQuestionAsync(Question question)
    {
        await using var context = await contextFactory.CreateDbContextAsync();
        try
        {
            context.Questions.Update(question);
            await context.SaveChangesAsync();
            return true;
        }
        catch (Exception e)
        {
            Console.WriteLine($@"{e.Message}");
        }

        return false;
    }

    public async Task<bool> DeleteQuestionAsync(Question question, List<Question> updatedQuestions)
    {
        await using var context = await contextFactory.CreateDbContextAsync();

        try
        {
            context.Questions.Remove(question);
            context.UpdateRange(updatedQuestions);
            await context.SaveChangesAsync();
            return true;
        }
        catch (Exception e)
        {
            Console.WriteLine($@"{e.Message}");
        }

        return false;
    }

    public async Task<List<Question>> GetQuestionsByTemplateIdAsync(int templateId)
    {
        await using var context = await contextFactory.CreateDbContextAsync();
        try
        {
            var questions = await context.Questions
                .Where(q => q.TemplateId == templateId)
                .ToListAsync();
            return questions;
        }
        catch (Exception e)
        {
            Console.WriteLine($@"{e.Message}");
        }

        return [];
    }
}
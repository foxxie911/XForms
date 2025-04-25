using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using XForms.Data;

namespace XForms.Services;

public class QuestionService(ApplicationDbContext context)
{
    public async Task CreateQuestion(int templateId)
    {
        var question = new Question()
        {
            Title = "Untitled",
            Type = QuestionType.Singleline,
            Order = context.Questions.Count(),
            TemplateId = templateId,
            Version = Guid.NewGuid()
        };

        await context.Questions.AddAsync(question);
        await context.SaveChangesAsync();
    }

    public async Task RearrangeQuestionUponDrag(Question droppedQuestion, int dropIndex)
    {
        if (droppedQuestion.Order > dropIndex)
        {
            await context.Questions
                .Where(q => q.Order >= dropIndex && q.Order <= droppedQuestion.Order && q.Id != droppedQuestion.Id)
                .ForEachAsync(q => q.Order += 1);
        }

        if (droppedQuestion.Order < dropIndex)
        {
            await context.Questions
                .Where(q => q.Order <= dropIndex && q.Order >= droppedQuestion.Order && q.Id != droppedQuestion.Id)
                .ForEachAsync(q => q.Order -= 1);
        }

        droppedQuestion.Order = dropIndex;
        await context.SaveChangesAsync();
    }

    public async Task UpdateQuestion()
    {
        await context.SaveChangesAsync();
    }

    public async Task DeleteQuestion(Question question)
    {
        await context.Questions
            .Where(q => q.Order > question.Order)
            .ForEachAsync(q => q.Order -= 1);
        context.Questions.Remove(question);
        await context.SaveChangesAsync();
    }

    public IEnumerable<Question> GetTemplateQuestionById(int templateId)
    {
        try
        {
            var questions = context.Questions
                .Where(q => q.TemplateId == templateId)
                .AsNoTracking();
            return questions;
        }
        catch (Exception e)
        {
            Console.WriteLine($"{e.Message}");
        }
        return [];
    }
}
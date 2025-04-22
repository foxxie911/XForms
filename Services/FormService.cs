using Microsoft.EntityFrameworkCore;
using XForms.Data;

namespace XForms.Services;

public class FormService (ApplicationDbContext context)
{
    public async Task<int> CreateForm(string userId, int templateId)
    {
        try
        {
            var form = context.Forms.Add(new Form()
            {
                CreatorId = userId,
                TemplateId = templateId,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow,
                Version = Guid.NewGuid()
            });
            await context.SaveChangesAsync();
            return form.Entity.Id;
        }
        catch (Exception e)
        {
            Console.WriteLine($"{e.Message}");
        }

        return -1;
    }

    public async Task<Form> FindFormByIdAsync(int formId)
    {
        try
        {
            var form = await context.Forms.Where(f => f.Id == formId)
                .Include(f => f.Template)
                .Include(f => f.Template!.Questions)
                .FirstOrDefaultAsync();
            form!.UpdatedAt = DateTime.UtcNow;
            await context.SaveChangesAsync();
            return form;
        }
        catch (Exception e)
        {
            Console.WriteLine($"{e.Message}");
        }
        
        return null!;
    }

    public async Task<Form> FindFormByUserId(string userId)
    {
        try
        {
            var form = await context.Forms.FirstOrDefaultAsync(f => f.CreatorId == userId);
            return form!;
        }
        catch (Exception e)
        {
            Console.WriteLine($"{e.Message}");
        }

        return null!;
    }

    public async Task<bool> SubmitFormAsync(Form? form)
    {
        try
        {
            form!.IsSubmitted = true;
            await context.SaveChangesAsync();
            return true;
        }
        catch (Exception e)
        {
            Console.WriteLine($"{e.Message}");
        }

        return false;
    }
}
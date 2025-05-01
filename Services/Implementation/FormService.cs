using Microsoft.EntityFrameworkCore;
using XForms.Data;
using XForms.Services.Interface;

namespace XForms.Services.Implementation;

public class FormService(IDbContextFactory<ApplicationDbContext> contextFactory) : IFormService
{
    public async Task<int> CreateFormAsync(string userId, int templateId)
    {
        await using var context = await contextFactory.CreateDbContextAsync();
        try
        {
            var form = await context.Forms.AddAsync(new Form()
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

        return int.MinValue;
    }

    public async Task<Form> FindFormByIdAsync(int formId)
    {
        await using var context = await contextFactory.CreateDbContextAsync();
        try
        {
            var form = await context.Forms.Where(f => f.Id == formId)
                .Include(f => f.Template)
                .ThenInclude(t => t!.Questions)
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

    public async Task<Form> FindFormByUserAndTemplateIdAsync(string userId, int templateId)
    {
        await using var context = await contextFactory.CreateDbContextAsync();
        try
        {
            var form = await context.Forms
                .FirstOrDefaultAsync(f => f.CreatorId == userId && f.TemplateId == templateId);
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
        await using var context = await contextFactory.CreateDbContextAsync();
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

    public async Task<List<Form>> FindFormsByUserId(string userId)
    {
        await using var context = await contextFactory.CreateDbContextAsync();
        try
        {
            var forms = await context.Forms
                .Where(f => f.CreatorId == userId)
                .Include(f => f.Template)
                .ToListAsync();
            return forms;
        }
        catch (Exception e)
        {
            Console.WriteLine($"{e.Message}");
        }

        return [];
    }

    public async Task<bool> DeleteFormsAsync(HashSet<Form> selectedForms)
    {
        await using var context = await contextFactory.CreateDbContextAsync();
        try
        {
            context.Forms.RemoveRange(selectedForms);
            await context.SaveChangesAsync();
            return true;
        }
        catch (Exception e)
        {
            Console.WriteLine($"{e.Message}");
        }

        return false;
    }

    public async Task<List<Form>> FindFormsByTemplateId(int templateId)
    {
        await using var context = await contextFactory.CreateDbContextAsync();
        try
        {
            var forms = await context.Forms
                .Where(f => f.TemplateId == templateId && f.IsSubmitted == true)
                .Include(f => f.Creator)
                .AsNoTracking()
                .ToListAsync();
            return forms;
        }
        catch (Exception e)
        {
            Console.WriteLine($"{e.Message}");
        }

        return [];
    }
}
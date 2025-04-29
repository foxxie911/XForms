using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query;
using XForms.Data;

namespace XForms.Services;

public class FormService(ApplicationDbContext context)
{
    public int CreateForm(string userId, int templateId)
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
            context.SaveChanges();
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

    public Form FindFormByUserAndTemplateId(string userId, int templateId)
    {
        try
        {
            var form = context.Forms
                .FirstOrDefault(f => f.CreatorId == userId && f.TemplateId == templateId);
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

    public IEnumerable<Form>? FindFormsByUserId(string userId)
    {
        try
        {
            var forms = context.Forms
                .Where(f => f.CreatorId == userId)
                .AsNoTracking()
                .Include(f => f.Template);
            return forms;
        }
        catch (Exception e)
        {
            Console.WriteLine($"{e.Message}");
        }

        return [];
    }

    public IEnumerable<Form>? ListAllForms()
    {
        try
        {
            var forms = context.Forms
                .AsNoTracking()
                .Include(f => f.Template);
            return forms;
        }
        catch (Exception e)
        {
            Console.WriteLine($"{e.Message}");
        }

        return [];
    }

    public bool DeleteForms(HashSet<Form> selectedForms)
    {
        try
        {
            context.Forms.RemoveRange(selectedForms);
            context.SaveChanges();
            return true;
        }
        catch (Exception e)
        {
            Console.WriteLine($"{e.Message}");
        }

        return false;
    }

    public IEnumerable<Form> FindFormsByTemplateId(int templateId)
    {
        try
        {
            var forms = context.Forms
                .Where(f => f.TemplateId == templateId && f.IsSubmitted == true)
                .Include(f => f.Creator)
                .AsNoTracking();
            return forms;
        }
        catch (Exception e)
        {
            Console.WriteLine($"{e.Message}");
        }

        return [];
    }
}
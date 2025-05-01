using XForms.Data;

namespace XForms.Services.Interface;

public interface IFormService
{
    Task<int> CreateFormAsync(string userId, int templateId);
    Task<Form> FindFormByIdAsync(int formId);
    Task<Form> FindFormByUserAndTemplateIdAsync(string userId, int templateId);
    Task<bool> SubmitFormAsync(Form? form);
    Task<List<Form>> FindFormsByUserId(string userId);
    Task<bool> DeleteFormsAsync(HashSet<Form> selectedForms);
    Task<List<Form>> FindFormsByTemplateId(int templateId);
}

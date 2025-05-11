using XForms.Data;
using XForms.Data.ViewModels.Salesforce;

namespace XForms.Services.Interface;

public interface ISalesforceService
{
    Task<HttpRequestMessage> CreateRequestAsync(HttpMethod method, string endpoint);
    Task<string> CreateAccountAsync(SalesforceAccount account, ApplicationUser user);
    Task<string> CreateContactAsync(SalesforceContact contact);
}
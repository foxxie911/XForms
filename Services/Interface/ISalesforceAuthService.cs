namespace XForms.Services.Interface;

public interface ISalesforceAuthService
{
    Task<string> GetAccessTokenAsync();
    string GetInstanceUrl();
}
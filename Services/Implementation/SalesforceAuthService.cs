using System.Text.Json;
using System.Text.Json.Serialization;
using XForms.Services.Interface;

namespace XForms.Services.Implementation;

public class SalesforceAuthService(IConfiguration configuration, HttpClient httpClient) : ISalesforceAuthService
{
    private string _accessToken = string.Empty;
    private string _instanceUrl = string.Empty;
    
    public async Task<string> GetAccessTokenAsync()
    {
        var clientId = configuration["Salesforce:ClientId"];
        var clientSecret = configuration["Salesforce:ClientSecret"];
        var username = configuration["Salesforce:Username"];
        var password = configuration["Salesforce:Password"];
        var loginUrl = "https://login.salesforce.com/services/oauth2/token";
        
        var content = new FormUrlEncodedContent([
            new KeyValuePair<string,string>("grant_type", "password"),
            new KeyValuePair<string,string>("client_id", clientId!),
            new KeyValuePair<string, string>("client_secret", clientSecret!),
            new KeyValuePair<string, string>("username", username!),
            new KeyValuePair<string, string>("password", password!)
        ]);

        var response = await httpClient.PostAsync(loginUrl, content);
        response.EnsureSuccessStatusCode();

        var responseText = await response.Content.ReadAsStringAsync();
        var responseObject = JsonSerializer.Deserialize<SalesforceLoginResponse>(responseText);
        _accessToken = responseObject!.AccessToken;
        _instanceUrl = responseObject!.InstanceUrl;
        return _accessToken;
    }

    public string GetInstanceUrl()
    {
        return _instanceUrl;
    }
}

internal class SalesforceLoginResponse
{
    [JsonPropertyName("access_token")] public string AccessToken { get; init; } = string.Empty;
    [JsonPropertyName("instance_url")] public string InstanceUrl { get; init; } = string.Empty;
}
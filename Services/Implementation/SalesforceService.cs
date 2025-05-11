using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Internal;
using XForms.Data;
using XForms.Data.ViewModels.Salesforce;
using XForms.Services.Interface;

namespace XForms.Services.Implementation;

public class SalesforceService(HttpClient client, SalesforceAuthService authService, IDbContextFactory<ApplicationDbContext> contextFactory) : ISalesforceService
{
    public async Task<HttpRequestMessage> CreateRequestAsync(HttpMethod method, string endpoint)
    {
        var token = await authService.GetAccessTokenAsync();
        var instanceUrl = authService.GetInstanceUrl();

        var request = new HttpRequestMessage(method, $"{instanceUrl}/services/data/v62.0/{endpoint}");
        request.Headers.Add("Authorization", $"Bearer " + token);
        request.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

        return request;
    }

    public async Task<string> CreateAccountAsync(SalesforceAccount account, ApplicationUser user)
    {
        var content = JsonSerializer.Serialize(account);
        var request = await CreateRequestAsync(HttpMethod.Post, "sobjects/Account");
        request.Content = new StringContent(content, Encoding.UTF8, "application/json");
        var response = await client.SendAsync(request);
        response.EnsureSuccessStatusCode();
        var responseText = await response.Content.ReadAsStringAsync();
        var responseObject = JsonSerializer.Deserialize<SalesforceResponse>(responseText);

        await using var context = await contextFactory.CreateDbContextAsync();
        try
        {
            user.SalesforceId = responseObject!.Id;
            context.Users.Update(user);
            await context.SaveChangesAsync();
        }
        catch (Exception e)
        {
            Console.WriteLine(e.Message);
        }
        
        return responseObject!.Id;
    }

    public async Task<string> CreateContactAsync(SalesforceContact contact)
    {
        var content = JsonSerializer.Serialize(contact);
        var request = await CreateRequestAsync(HttpMethod.Post, "sobjects/Contact");
        request.Content = new StringContent(content, Encoding.UTF8, "application/json");
        var response = await client.SendAsync(request);
        response.EnsureSuccessStatusCode();
        var responseText = await response.Content.ReadAsStringAsync();
        var responseObject = JsonSerializer.Deserialize<SalesforceResponse>(responseText);
        return responseObject!.Id;
    }
}

internal class SalesforceResponse
{
    [JsonPropertyName("id")] public string Id { get; init; } = string.Empty;
    [JsonPropertyName("success")] public bool Success { get; init; }
}
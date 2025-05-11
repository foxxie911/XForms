using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Identity;
using MudBlazor;
using XForms.Data;
using XForms.Data.ViewModels.Salesforce;
using XForms.Services.Interface;

namespace XForms.Components.Form;

public partial class SalesforceForm : ComponentBase
{
    // Dependency Injection
    [Inject] private AuthenticationStateProvider? ApplicationStateProvider { get; set; }
    [Inject] private UserManager<ApplicationUser>? UserManager { get; set; }
    [Inject] private ISalesforceService? SalesforceService { get; set; }
    [Inject] private ISnackbar? Snackbar { get; set; }
    
    // Class variable
    private ApplicationUser? _currentUser;
    private MudForm? _salesforceForm;
    private bool _isValid;
    private bool _isSubmitting;
    private SalesforceAccountContactViewModel _accountContactViewModel = new();

    protected override async Task OnInitializedAsync()
    {
        await base.OnInitializedAsync();
        var authState = await ApplicationStateProvider!.GetAuthenticationStateAsync();
        _currentUser = await UserManager!.GetUserAsync(authState.User);
    }

    private async Task SubmitForm()
    {
        await _salesforceForm!.Validate();

        if (_isValid)
        {
            try
            {
                _isSubmitting = true;
                var accountId = await SalesforceService!.CreateAccountAsync(_accountContactViewModel.Account!, _currentUser!);
                _accountContactViewModel.Contact!.AccountId = accountId;
                await SalesforceService!.CreateContactAsync(_accountContactViewModel.Contact);
                Snackbar!.Add("Account and Contact created successfully", Severity.Success);
                _accountContactViewModel = new SalesforceAccountContactViewModel();
                await _salesforceForm.ResetAsync();
            }
            catch (Exception ex)
            {
                Snackbar!.Add($"Error creating records: {ex.Message}", Severity.Error);
            }
            finally
            {
                _isSubmitting = false;
            }
        }
    }
}
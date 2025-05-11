namespace XForms.Data.ViewModels.Salesforce;

public class SalesforceAccountContactViewModel
{
    public SalesforceAccount? Account { get; set; } = new SalesforceAccount();
    public SalesforceContact? Contact { get; set; } = new SalesforceContact();
}
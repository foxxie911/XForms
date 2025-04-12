using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using XForms.Data;

namespace XForms.Components.Account;

public class LogOutController(SignInManager<ApplicationUser> signInManager) : Controller
{
    [IgnoreAntiforgeryToken]
    [HttpGet("/Account/LogOut")]
    public async Task<IActionResult> LogOut()
    {
        const string returnUrl = "/account/login";
        await signInManager.SignOutAsync();
        await HttpContext.SignOutAsync(IdentityConstants.ApplicationScheme);
        await HttpContext.SignOutAsync(IdentityConstants.ExternalScheme);
        HttpContext.Response.Cookies.Delete(".AspNetCore.Identity.Application");
        return Redirect(returnUrl);
    }
}
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using XForms.Data;

namespace XForms.Authorization;

public class TemplateEditorHandler : AuthorizationHandler<TemplateEditorRequirement, Template>
{
    protected override Task HandleRequirementAsync(
        AuthorizationHandlerContext context, 
        TemplateEditorRequirement requirement,
        Template resource)
    {
        var currentUserId = context.User.FindFirst(ClaimTypes.NameIdentifier);
        if (currentUserId?.Value is null) return Task.CompletedTask;
        var templateCreatorId = resource.CreatorId;

        if (context.User.IsInRole("Admin"))
        {
            context.Succeed(requirement);
        }

        if (currentUserId.Value == templateCreatorId)
        {
            context.Succeed(requirement);
        }
        return Task.CompletedTask;
    }
}
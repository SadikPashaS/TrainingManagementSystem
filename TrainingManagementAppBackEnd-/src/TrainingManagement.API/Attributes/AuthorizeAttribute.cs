using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using TrainingManagement.Domain.Enums;

namespace TrainingManagement.API.Attributes;

[AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
public class AuthorizeAttribute : Attribute, IAuthorizationFilter
{
    private readonly UserRole? _requiredRole;

    public AuthorizeAttribute()
    {
    }

    public AuthorizeAttribute(UserRole requiredRole)
    {
        _requiredRole = requiredRole;
    }

    public void OnAuthorization(AuthorizationFilterContext context)
    {
        var user = context.HttpContext.User;

        if (!user.Identity?.IsAuthenticated ?? true)
        {
            context.Result = new UnauthorizedResult();
            return;
        }

        if (_requiredRole.HasValue)
        {
            var userRoleClaim = user.FindFirst(System.Security.Claims.ClaimTypes.Role);
            if (userRoleClaim == null || !Enum.TryParse<UserRole>(userRoleClaim.Value, out var userRole))
            {
                context.Result = new ForbidResult();
                return;
            }

            if (userRole != _requiredRole.Value)
            {
                context.Result = new ForbidResult();
                return;
            }
        }
    }
}
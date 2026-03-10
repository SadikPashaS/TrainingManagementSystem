using System.Security.Claims;
using TrainingManagement.Domain.Enums;

namespace TrainingManagement.API;

public static class HttpContextExtensions
{
    public static int GetUserId(this HttpContext httpContext)
    {
        var userIdClaim = httpContext.User.FindFirst(ClaimTypes.NameIdentifier);
        if (userIdClaim == null || !int.TryParse(userIdClaim.Value, out var userId))
        {
            throw new UnauthorizedAccessException("User not authenticated");
        }
        return userId;
    }

    public static UserRole GetUserRole(this HttpContext httpContext)
    {
        var roleClaim = httpContext.User.FindFirst(ClaimTypes.Role);
        if (roleClaim == null || !Enum.TryParse<UserRole>(roleClaim.Value, out var role))
        {
            throw new UnauthorizedAccessException("User role not found");
        }
        return role;
    }

    public static string GetUserName(this HttpContext httpContext)
    {
        var nameClaim = httpContext.User.FindFirst(ClaimTypes.Name);
        return nameClaim?.Value ?? string.Empty;
    }
}